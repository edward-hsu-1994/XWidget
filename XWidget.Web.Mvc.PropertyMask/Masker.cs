using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XWidget.Web.Mvc.PropertyMask;
using XWidget.Reflection;
using Castle.DynamicProxy;
using Newtonsoft.Json;

namespace Microsoft.AspNetCore.Mvc {
    /// <summary>
    /// 屬性屏蔽核心
    /// </summary>
    public static class Masker {
        /// <summary>
        /// 啟用Masker的類型檢查條件
        /// </summary>
        public static Func<Type, bool> MaskCondition { get; set; } =
            (Type type) => {
                /*if (type.Namespace.StartsWith("Microsoft.EntityFrameworkCore")) {
                    return false;
                }*/
                return true;
            };


        /// <summary>
        /// 取得屬性屏蔽後的結果
        /// </summary>
        /// <typeparam name="T">資料類型</typeparam>
        /// <param name="data">資料</param>
        /// <param name="patternName">模式名稱</param>
        /// <returns>屏蔽後的資料</returns>
        public static TData Mask<TData>(
            TData data,
            string patternName)
            where TData : class {
            // 引動內部屏蔽方法，並深層複製原始資料，中斷參考關係
            return InternalMask(null, data.GetType(), data, null, patternName, new List<object>());
        }

        /// <summary>
        /// 取得屬性屏蔽後的結果
        /// </summary>
        /// <typeparam name="T">資料類型</typeparam>
        /// <param name="data">資料</param>
        /// <param name="controller">控制實例</param>
        /// <param name="patternName">模式名稱</param>
        /// <returns>屏蔽後的資料</returns>
        public static TData Mask<TData, TController>(
            TData data,
            TController controller,
            string patternName = null)
            where TData : class
            where TController : Controller {
            // 引動內部屏蔽方法，並深層複製原始資料，中斷參考關係
            return InternalMask(null, data.GetType(), data, controller, patternName, new List<object>());
        }

        /// <summary>
        /// 取得屬性屏蔽後的結果
        /// </summary>
        /// <typeparam name="T">資料類型</typeparam>
        /// <param name="data">資料</param>
        /// <param name="patternName">模式名稱</param>
        /// <returns>屏蔽後的資料</returns>
        public static IEnumerable<TData> Mask<TData>(
            IEnumerable<TData> data,
            string patternName)
            where TData : class {
            // 引動內部屏蔽方法，並深層複製原始資料，中斷參考關係
            return InternalMask(null, data.GetType(), data, null, patternName, new List<object>());
        }

        /// <summary>
        /// 取得屬性屏蔽後的結果
        /// </summary>
        /// <typeparam name="T">資料類型</typeparam>
        /// <param name="data">資料</param>
        /// <param name="controller">控制實例</param>
        /// <param name="patternName">模式名稱</param>
        /// <returns>屏蔽後的資料</returns>
        public static IEnumerable<TData> Mask<TData, TController>(
            IEnumerable<TData> data,
            TController controller,
            string patternName = null)
            where TData : class
            where TController : Controller {
            // 引動內部屏蔽方法，並深層複製原始資料，中斷參考關係
            return InternalMask(null, data.GetType(), data, controller, patternName, new List<object>());
        }


        internal static IEnumerable<TData> InternalMask<TData>(
            Type declaringType,
            Type packageType,
            IEnumerable<TData> data,
            object controller,
            string patternName,
            List<object> refList)
            where TData : class {
            List<TData> result = new List<TData>();

            foreach (var ele in data) {
                result.Add(InternalMask(declaringType, packageType, ele, controller, patternName, refList));
            }

            return result.AsEnumerable();
        }

        internal static TData InternalMask<TData>(
            Type declaringType,
            Type packageType,
            TData data,
            object controller,
            string patternName,
            List<object> refList)
            where TData : class {

            var type = typeof(TData);

            if (!MaskCondition(type)) {
                return data;
            }

            // 檢查是否發生參考循環
            if (refList.Contains(data)) {
                // 發生參考循環則直接返回
                return data;
            }
            // 加入參考列表
            refList.Add(data);



            #region 可列舉型別處理
            if (data is IEnumerable && type.IsGenericType && type.GetGenericTypeDefinition().GetInterfaces().Contains(typeof(IEnumerable))) {
                var internalMaskMethods = typeof(Masker).GetMethods(BindingFlags.Static | BindingFlags.NonPublic);

                var elementType = type.GetGenericArguments().First();

                var tempData = internalMaskMethods.Single(x => x.ReturnType.GetInterfaces().Contains(typeof(IEnumerable))).InvokeGeneric(
                    new Type[] { elementType },
                    new object[] { declaringType, packageType, data, controller, patternName, refList });

                try {
                    return (TData)tempData;
                } catch {
                    if (tempData is IEnumerable enumerable && typeof(TData).GetInterfaces().Contains(typeof(IQueryable))) {
                        return (TData)enumerable.AsQueryable();
                    }
                    return (TData)Convert.ChangeType(tempData, typeof(TData));
                }
            }
            #endregion

            var interceptor = new PropertyMaskInterceptor();

            #region 取得該類型中非靜態的所有屬性
            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                // 取得該屬性的JsonPropertyMaskAttribute集合，如果未設定則應該為空集合
                var attrs = property.GetCustomAttributes<PropertyMaskAttribute>();

                // 在JsonMask設定集合中尋找是否有符合項目
                if (attrs.Any(x => x.IsMatch(controller as Controller, type, packageType, patternName))) {
                    interceptor.MaskedProperties.Add(property.Name);
                } else {
                    // 該屬性找不到屏蔽設定，檢查該屬性的屬性類型是否有屏蔽選項
                    var propertyType = property.PropertyType;
                    if (propertyType.IsValueType || propertyType.Namespace.StartsWith("System")) continue;

                    if (!MaskCondition(propertyType)) {
                        continue;
                    }

                    if (!property.GetMethod.IsVirtual) {
                        throw new MemberAccessException($"屬性{type.Name}.{property.Name}必須為Virtual");
                    }

                    var value = property.GetValue(data);
                    if (value == null) continue;

                    interceptor.ReplaceProperties[property.Name] =
                        InternalMask(
                            type,
                            propertyType,
                            value,
                            controller,
                            patternName,
                            refList
                        );
                }
            }
            #endregion

            if (interceptor.MaskedProperties.Count == 0 &&
                interceptor.ReplaceProperties.Count == 0) {
                return data;
            }

            return new Castle.DynamicProxy.ProxyGenerator()
                    .CreateClassProxyWithTarget<TData>(data, interceptor);
        }
    }
}