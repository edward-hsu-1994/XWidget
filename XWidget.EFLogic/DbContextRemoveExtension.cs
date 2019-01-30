using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using XWidget.Utilities;
using Microsoft.EntityFrameworkCore.Metadata;

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

            void SetNavigationToDefault(IEntityType _currentType, IEntityType _navTargetType, object _navTargetValue) {
                //找出目標類型相依於目前類型的外來鍵屬性
                foreach (var targets in _navTargetType.GetForeignKeys().Where(
                    // 這個外來鍵由導覽類別所定義
                    x => x.DeclaringEntityType == _navTargetType &&
                    // 並且目標為目前類別
                         x.PrincipalEntityType == _currentType)) {
                    if (_navTargetValue is IEnumerable _enumValue) {
                        foreach (var element in _enumValue) {
                            foreach (var fk in targets.Properties) {
                                fk.PropertyInfo.SetValue(element, TypeUtility.GetDefault(fk.PropertyInfo.PropertyType));
                            }
                        }
                    } else {
                        foreach (var fk in targets.Properties) {
                            fk.PropertyInfo.SetValue(_navTargetValue, TypeUtility.GetDefault(fk.PropertyInfo.PropertyType));
                        }
                    }
                }
            }

            Type type = entity.GetType();

            // 獲取類型中的連鎖刪除方案設定
            RemoveCascadeAttribute removeCascadeAttribute = type.GetCustomAttribute<RemoveCascadeAttribute>();
            if (removeCascadeAttribute == null) {
                removeCascadeAttribute = new RemoveCascadeAttribute() {
                    Mode = RemoveCascadeMode.OptOut
                };
            }


            var entityType = context.Model.FindRuntimeEntityType(type);
            var navProperties = entityType.GetNavigations();

            foreach (var property in navProperties) {
                var value = property.PropertyInfo.GetValue(entity);

                #region 省略無需處理屬性
                if (value == null) {
                    continue;
                }

                // 略過無對應屬性
                if (property.PropertyInfo.GetCustomAttribute<NotMappedAttribute>() != null) {
                    continue;
                }
                #endregion

                var shouldRemoveCascade = type.GetMethod(
                    $"ShouldRemoveCascade{property.Name}",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

                if (removeCascadeAttribute.Mode == RemoveCascadeMode.OptOut) {
                    if (shouldRemoveCascade != null && //如果存在連鎖刪除判斷方法且不允許連鎖刪除
                        false.Equals(shouldRemoveCascade.Invoke(entity, new object[0]))) {
                        SetNavigationToDefault(entityType, property.GetTargetType(), value);
                        continue;
                    }

                    if (property.PropertyInfo.GetCustomAttribute<RemoveCascadeStopperAttribute>() != null) {
                        SetNavigationToDefault(entityType, property.GetTargetType(), value);
                        continue;
                    }

                    if (value is IEnumerable enumValue) {
                        foreach (var element in enumValue) {
                            result.AddRange(GetRefEntities(context, element));
                        }
                    } else {
                        result.Add(value);
                    }
                } else if (removeCascadeAttribute.Mode == RemoveCascadeMode.OptIn) {
                    if (shouldRemoveCascade != null && //如果存在連鎖刪除判斷方法且不允許連鎖刪除
                        false.Equals(shouldRemoveCascade.Invoke(entity, new object[0]))) {
                        SetNavigationToDefault(entityType, property.GetTargetType(), value);
                        continue;
                    }

                    // 未明確標記可連鎖刪除
                    if (property.PropertyInfo.GetCustomAttribute<RemoveCascadePropertyAttribute>() == null &&
                        shouldRemoveCascade == null) {
                        SetNavigationToDefault(entityType, property.GetTargetType(), value);
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
            /// <param name="runtimeType">檢查類型</param>
            /// <returns>是否符合 </returns>
            bool TypeCheck(Type runtimeType) {
                if (runtimeType.IsGenericType &&
                    runtimeType.GetGenericTypeDefinition() == typeof(ICollection<>)) {
                    runtimeType = runtimeType.GetGenericArguments()[0];
                }
                return entitiesTypes
                    .Contains(runtimeType);
            }


            if (!TypeCheck(type)) {
                return;
            }

            result.Add(type);

            var entity = FormatterServices.GetUninitializedObject(type);
            var entityType = context.Model.FindRuntimeEntityType(type);

            // 獲取類型中的連鎖刪除方案設定
            RemoveCascadeAttribute removeCascadeAttribute = type.GetCustomAttribute<RemoveCascadeAttribute>();
            if (removeCascadeAttribute == null) {
                removeCascadeAttribute = new RemoveCascadeAttribute() {
                    Mode = RemoveCascadeMode.OptOut
                };
            }

            var navs = entityType.GetNavigations();

            foreach (var nav in navs) {
                if (!TypeCheck(nav.PropertyInfo.PropertyType)) {
                    continue;
                }
                if (removeCascadeAttribute.Mode == RemoveCascadeMode.OptOut) {
                    if (nav.PropertyInfo.GetCustomAttribute<RemoveCascadeStopperAttribute>() != null) {
                        continue;
                    }

                    var shouldRemoveCascade = type.GetMethod(
                        $"ShouldRemoveCascade{nav.Name}",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

                    if (shouldRemoveCascade != null && //如果存在連鎖刪除判斷方法且不允許連鎖刪除
                        false.Equals(shouldRemoveCascade.Invoke(entity, new object[0]))) {
                        continue;
                    }

                    LoadRemoveCascadeTypes(
                            context,
                            nav.GetTargetType().ClrType,
                            result);
                } else if (removeCascadeAttribute.Mode == RemoveCascadeMode.OptIn) {
                    if (nav.PropertyInfo.GetCustomAttribute<RemoveCascadePropertyAttribute>() == null) {
                        continue;
                    }

                    var shouldRemoveCascade = type.GetMethod(
                        $"ShouldRemoveCascade{nav.Name}",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

                    if (shouldRemoveCascade != null && //如果存在連鎖刪除判斷方法且不允許連鎖刪除
                        false.Equals(shouldRemoveCascade.Invoke(entity, new object[0]))) {
                        continue;
                    }

                    LoadRemoveCascadeTypes(
                            context,
                            nav.GetTargetType().ClrType,
                            result);
                }
            }
        }
    }
}
