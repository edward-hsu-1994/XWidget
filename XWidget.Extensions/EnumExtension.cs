using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System {
    /// <summary>
    /// 針對<see cref="Enum"/>的擴充方法
    /// </summary>
    public static class EnumExtension {
        /// <summary>
        /// 取得目標列舉值之名稱
        /// </summary>
        /// <typeparam name="TEnum">列舉類別</typeparam>
        /// <param name="value">列舉值</param>
        /// <returns>目標列舉值名稱</returns>
        public static string GetEnumName<TEnum>(this TEnum value) where TEnum : Enum {
            return Enum.GetName(typeof(TEnum), value);
        }

        /// <summary>
        /// 取得目標列舉值之<see cref="Attribute"/>集合
        /// </summary>
        /// <typeparam name="TEnum">列舉類別</typeparam>
        /// <param name="attributeType">目標<see cref="Attribute"/>類別</param>
        /// <param name="value">列舉值</param>
        /// <returns>Attribute集合</returns>
        public static IEnumerable<Attribute> GetCustomAttributes<TEnum>(this TEnum value, Type attributeType)
            where TEnum : Enum {
            var isEnum = value.GetType().GetTypeInfo().IsValueType;
            if (!isEnum) throw new NotSupportedException("value is not Enum");
            var typeinfo = value.GetType().GetTypeInfo();
            var fieldInfo = typeinfo.GetField(GetEnumName(value));
            return fieldInfo.GetCustomAttributes(attributeType);
        }

        /// <summary>
        /// 取得目標列舉值之<see cref="Attribute"/>
        /// </summary>
        /// <typeparam name="TEnum">列舉類別</typeparam>
        /// <param name="attributeType">目標<see cref="Attribute"/>類別</param>
        /// <param name="value">列舉值</param>
        /// <returns>Attribute</returns>
        public static Attribute GetCustomAttribute<TEnum>(this TEnum value, Type attributeType)
            where TEnum : Enum {
            var isEnum = value.GetType().GetTypeInfo().IsValueType;
            if (!isEnum) throw new NotSupportedException("value is not Enum");
            var typeinfo = value.GetType().GetTypeInfo();
            var fieldInfo = typeinfo.GetField(GetEnumName(value));
            return fieldInfo.GetCustomAttribute(attributeType);
        }

        /// <summary>
        ///  取得目標列舉值之<see cref="Attribute"/>集合
        /// </summary>
        /// <typeparam name="TEnum">列舉類別</typeparam>
        /// <typeparam name="TAttribute">目標<see cref="Attribute"/>類別</typeparam>
        /// <param name="value">列舉值</param>
        /// <returns>Attribute集合</returns>
        public static IEnumerable<TAttribute> GetCustomAttributes<TEnum, TAttribute>(this TEnum value)
            where TEnum : Enum
            where TAttribute : Attribute {
            return GetCustomAttributes(value, typeof(TAttribute)).Cast<TAttribute>();
        }

        /// <summary>
        /// 取得目標列舉值之<see cref="Attribute"/>
        /// </summary>
        /// <typeparam name="TEnum">列舉類別</typeparam>
        /// <typeparam name="TAttribute">目標<see cref="Attribute"/>類別</typeparam>
        /// <param name="value">列舉值</param>
        /// <returns>Attribute</returns>
        public static TAttribute GetCustomAttribute<TEnum, TAttribute>(this TEnum value)
            where TEnum : Enum
            where TAttribute : Attribute {
            return (TAttribute)GetCustomAttribute(value, typeof(TAttribute));
        }
    }
}
