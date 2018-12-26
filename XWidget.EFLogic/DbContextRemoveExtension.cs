using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace XWidget.EFLogic {
    /// <summary>
    /// 針對<see cref="DbContext"/>的擴充方法
    /// </summary>
    public static class DbContextRemoveExtensions {
        /// <summary>
        /// 取得所有關聯物件
        /// </summary>
        /// <param name="context">DbContext實例</param>
        /// <param name="entity">Model實例</param>
        /// <returns>關聯物件集合</returns>
        private static IEnumerable<object> GetRefEntities(this DbContext context, object entity) {
            List<object> result = new List<object>();

            var entitiesTypes = context.Model.GetEntityTypes()
                    .Select(x => x.ClrType);

            /// <summary>
            /// 檢查是否為EF模型類型
            /// </summary>
            /// <param name="entityType">檢查類型</param>
            /// <returns>是否符合 </returns>
            bool TypeCheck(Type entityType) {
                if (entityType.IsGenericType &&
                    entityType.GetGenericTypeDefinition() == typeof(ICollection<>)) {
                    entityType = entityType.GetGenericArguments()[0];
                }
                return entitiesTypes
                    .Contains(entityType);
            }

            // 取得基礎類型
            Type GetRawType(Type _type) {
                if (_type.IsGenericType &&
                    _type.GetGenericTypeDefinition() == typeof(ICollection<>)) {
                    _type = _type.GetGenericArguments()[0];
                }
                return _type;
            }

            // 取得預設值
            object GetDefault(Type _type) {
                if (_type.IsValueType) {
                    return Activator.CreateInstance(_type);
                }
                return null;
            }

            // 清除外來鍵
            void ClearFK(Type currentType, Type propertyOrFieldType, object value) {
                var entityType = context.Model.FindRuntimeEntityType(currentType);
                var propertyType = context.Model.FindRuntimeEntityType(GetRawType(propertyOrFieldType));

                var fks_Prop = entityType.GetReferencingForeignKeys()
                    .Where(x => x.DeclaringEntityType == propertyType)
                    .SelectMany(x => x.Properties)
                    .Select(x => x.PropertyInfo)
                    .Where(x => x != null);
                var fks_Field = entityType.GetReferencingForeignKeys()
                    .Where(x => x.DeclaringEntityType == propertyType)
                    .SelectMany(x => x.Properties)
                    .Select(x => x.FieldInfo)
                    .Where(x => x != null);

                if (value is IEnumerable _enumValue) {
                    foreach (var element in _enumValue) {
                        foreach (var fk in fks_Prop) {
                            fk.SetValue(element, GetDefault(fk.PropertyType));
                        }
                        foreach (var fk in fks_Field) {
                            fk.SetValue(element, GetDefault(fk.FieldType));
                        }
                    }
                } else {
                    foreach (var fk in fks_Prop) {
                        fk.SetValue(value, GetDefault(fk.PropertyType));
                    }
                    foreach (var fk in fks_Field) {
                        fk.SetValue(value, GetDefault(fk.FieldType));
                    }
                }
            }

            Type type = entity.GetType();

            foreach (var property in type.GetProperties()) {
                var value = property.GetValue(entity);

                #region 省略無需處理屬性
                if (value == null) {
                    continue;
                }

                // 略過無對應屬性
                if (property.GetCustomAttribute<NotMappedAttribute>() != null) {
                    continue;
                }

                if (!TypeCheck(property.PropertyType)) {
                    continue;
                }
                #endregion

                if (property.GetCustomAttribute<RemoveCascadeStopperAttribute>() != null) {
                    ClearFK(type, property.PropertyType, value);
                    continue;
                }

                var shouldRemoveCascade = type.GetMethod(
                    $"ShouldRemoveCascade{property.Name}",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

                if (shouldRemoveCascade != null && //如果存在連鎖刪除判斷方法且不允許連鎖刪除
                    false.Equals(shouldRemoveCascade.Invoke(entity, new object[0]))) {
                    ClearFK(type, property.PropertyType, value);
                    continue;
                }

                if (value is IEnumerable enumValue) {
                    foreach (var element in enumValue) {
                        result.AddRange(GetRefEntities(context, element));
                    }
                } else {
                    result.Add(value);
                }
            }

            foreach (var field in type.GetFields()) {
                var value = field.GetValue(entity);

                #region 省略無需處理欄位
                if (value == null) {
                    continue;
                }

                // 略過無對應欄位
                if (field.GetCustomAttribute<NotMappedAttribute>() != null) {
                    continue;
                }

                if (!TypeCheck(field.FieldType)) {
                    continue;
                }
                #endregion

                if (field.GetCustomAttribute<RemoveCascadeStopperAttribute>() != null) {
                    ClearFK(type, field.FieldType, value);
                    continue;
                }

                var shouldRemoveCascade = type.GetMethod(
                    $"ShouldRemoveCascade{field.Name}",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

                if (shouldRemoveCascade != null && //如果存在連鎖刪除判斷方法且不允許連鎖刪除
                    false.Equals(shouldRemoveCascade.Invoke(entity, new object[0]))) {
                    ClearFK(type, field.FieldType, value);
                    continue;
                }

                if (value is IEnumerable enumValue) {
                    foreach (var element in enumValue) {
                        result.AddRange(GetRefEntities(context, element));
                    }
                } else {
                    result.Add(value);
                }
            }

            return result.Concat(new object[] { entity });
        }

        /// <summary>
        /// 移除範圍內物件實例與相關聯的物件
        /// </summary>
        /// <param name="context">DbContext實例</param>
        /// <param name="entities">Model實例集合</param>
        public static void RemoveRangeCascade(this DbContext context, params object[] entities) {
            RemoveRangeCascade(context, (IEnumerable<object>)entities);
        }

        /// <summary>
        /// 移除範圍內物件實例與相關聯的物件
        /// </summary>
        /// <param name="context">DbContext實例</param>
        /// <param name="entities">Model實例集合</param>
        public static void RemoveRangeCascade(this DbContext context, IEnumerable<object> entities) {
            context.RemoveRange(entities.SelectMany(x => GetRefEntities(context, x)));
        }

        /// <summary>
        /// 移除物件實例與相關聯的物件
        /// </summary>
        /// <param name="context">DbContext實例</param>
        /// <param name="entity">Model實例</param>
        public static void RemoveCascade<T>(this DbContext context, T entity) {
            RemoveCascade(context, (object)entity);
        }

        /// <summary>
        /// 移除物件實例與相關聯的物件
        /// </summary>
        /// <param name="context">DbContext實例</param>
        /// <param name="entity">Model實例</param>
        public static void RemoveCascade(this DbContext context, object entity) {
            RemoveRangeCascade(context, entity);
        }

        /// <summary>
        /// 取得相關聯連鎖刪除類型清單
        /// </summary>
        /// <param name="context">DbContext實例</param>
        /// <param name="type">起始類型</param>
        /// <returns>連鎖刪除類型清單</returns>
        public static Type[] GetRemoveCascadeTypes(this DbContext context, Type type) {
            List<Type> result = new List<Type>();
            LoadRemoveCascadeTypes(context, type, result);
            return result.ToArray();
        }

        internal static void LoadRemoveCascadeTypes(
            DbContext context,
            Type type,
            List<Type> result = null) {

            if (result == null) {
                result = new List<Type>();
            }

            if (result.Contains(type)) {
                return;
            }

            var entitiesTypes = context.Model.GetEntityTypes()
                    .Select(x => x.ClrType);

            /// <summary>
            /// 檢查是否為EF模型類型
            /// </summary>
            /// <param name="entityType">檢查類型</param>
            /// <returns>是否符合 </returns>
            bool TypeCheck(Type entityType) {
                if (entityType.IsGenericType &&
                    entityType.GetGenericTypeDefinition() == typeof(ICollection<>)) {
                    entityType = entityType.GetGenericArguments()[0];
                }
                return entitiesTypes
                    .Contains(entityType);
            }


            if (!TypeCheck(type)) {
                return;
            }

            result.Add(type);

            var entity = FormatterServices.GetUninitializedObject(type);

            foreach (var property in type.GetProperties()) {
                if (!TypeCheck(property.PropertyType)) {
                    continue;
                }

                if (property.GetCustomAttribute<RemoveCascadeStopperAttribute>() != null) {
                    continue;
                }

                var shouldRemoveCascade = type.GetMethod(
                    $"ShouldRemoveCascade{property.Name}",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

                if (shouldRemoveCascade != null && //如果存在連鎖刪除判斷方法且不允許連鎖刪除
                    false.Equals(shouldRemoveCascade.Invoke(entity, new object[0]))) {
                    continue;
                }

                if (property.PropertyType.GetInterfaces()
                    .Where(x => x.IsGenericType)
                    .Select(x => x.GetGenericTypeDefinition())
                    .Contains(typeof(IEnumerable<>))) {
                    LoadRemoveCascadeTypes(
                        context,
                        property.PropertyType.GetGenericArguments().FirstOrDefault(),
                        result);
                } else {
                    LoadRemoveCascadeTypes(context, property.PropertyType, result);
                }
            }

            foreach (var field in type.GetFields()) {
                if (!TypeCheck(field.FieldType)) {
                    continue;
                }

                if (field.GetCustomAttribute<RemoveCascadeStopperAttribute>() != null) {
                    continue;
                }

                var shouldRemoveCascade = type.GetMethod(
                    $"ShouldRemoveCascade{field.Name}",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

                if (shouldRemoveCascade != null && //如果存在連鎖刪除判斷方法且不允許連鎖刪除
                    false.Equals(shouldRemoveCascade.Invoke(entity, new object[0]))) {
                    continue;
                }

                if (field.FieldType.GetInterfaces().Contains(typeof(IEnumerable<>))) {
                    result.AddRange(GetRemoveCascadeTypes(context, field.FieldType.GetGenericArguments().FirstOrDefault()));
                } else {
                    result.Add(field.FieldType);
                }
            }
        }
    }
}
