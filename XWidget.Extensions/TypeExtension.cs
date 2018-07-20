using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace System {
    /// <summary>
    /// 類型擴充方法
    /// </summary>
    public static class TypeExtension {
        /// <summary>
        /// 取得目前類型的為初始化物件實例
        /// </summary>
        /// <param name="type">類型</param>
        /// <returns>物件實例</returns>
        public static object GetUninitializedObject(this Type type) {
            return FormatterServices.GetUninitializedObject(type);
        }

        /// <summary>
        /// 取得指定類別繼承鏈中所有類別
        /// </summary>
        /// <param name="type">類別實例</param>
        /// <returns>類別繼承鏈所有類別陣列</returns>
        public static Type[] GetAllBaseTypes(this Type type) {
            if (type == typeof(object)) {
                return typeof(object).BoxingToArray();
            }
            return type.BaseType.GetAllBaseTypes().Concat(type.BoxingToArray()).ToArray();
        }

        /// <summary>
        /// 取得指定類別繼承鏈中所有類別資訊
        /// </summary>
        /// <param name="typeInfo">類別實例</param>
        /// <returns>類別繼承鏈所有類別</returns>
        public static TypeInfo[] GetAllBaseTypeInfos(this TypeInfo typeInfo) {
            if (typeInfo == typeof(object).GetTypeInfo()) {
                return typeof(object).GetTypeInfo().BoxingToArray();
            }
            return typeInfo.BaseType.GetTypeInfo().GetAllBaseTypeInfos().Concat(typeInfo.BoxingToArray()).ToArray();
        }

        /// <summary>
        /// 確認目前類別是否為匿名類別
        /// </summary>
        /// <param name="type">類別實例</param>
        /// <returns>是否為匿名類別</returns>
        public static bool IsAnonymousType(this Type type) {
            if (type == null) return false;
            return type.Namespace == null;
        }

        /// <summary>
        /// 取得目前實例類別的預設值
        /// </summary>
        /// <typeparam name="T">實例類別</typeparam>
        /// <param name="obj">目前實例</param>
        /// <returns>目前實例類別的預設值</returns>
        public static T GetDefault<T>(this T obj) {
            var type = obj.GetType();
            if (type.IsValueType) {
                return (T)Activator.CreateInstance(type);
            } else {
                return default(T);
            }
        }
    }
}
