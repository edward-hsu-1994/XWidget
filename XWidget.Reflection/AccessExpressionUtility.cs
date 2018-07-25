using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace XWidget.Reflection {
    /// <summary>
    /// 產生Access Expression的常用方法
    /// </summary>
    public static class AccessExpressionUtility {
        /// <summary>
        /// 產生存取<see cref="Func{T, TResult}"/>
        /// </summary>
        /// <typeparam name="T">存取目標類別</typeparam>
        /// <param name="name">屬性或欄位名稱</param>
        /// <returns>存取<see cref="Func{T, TResult}"/></returns>
        public static Expression<Func<T, object>> CreateAccessFunc<T>(string name) {
            return CreateAccessExpressionFunc<T>(name);
        }

        /// <summary>
        /// 產生存取<see cref="Func{T, TResult}"/>
        /// </summary>
        /// <typeparam name="T">存取目標類別</typeparam>
        /// <typeparam name="R">存取結果類別</typeparam>
        /// <param name="name">屬性或欄位名稱</param>
        /// <returns>存取<see cref="Func{T, TResult}"/></returns>
        public static Func<T, R> CreateAccessFunc<T, R>(string name) {
            return CreateAccessExpressionFunc<T, R>(name).Compile();
        }

        /// <summary>
        /// 產生存取Expression <see cref="Func{T, TResult}"/>
        /// </summary>
        /// <typeparam name="T">存取目標類別</typeparam>
        /// <param name="name">屬性或欄位名稱</param>
        /// <returns>存取Expression <see cref="Func{T, TResult}"/></returns>
        public static Expression<Func<T, object>> CreateAccessExpressionFunc<T>(string name) {
            var p = Expression.Parameter(typeof(T), "x");
            var member = typeof(T).GetMember(name,
               BindingFlags.Instance |
               BindingFlags.Static |
               BindingFlags.Public |
               BindingFlags.NonPublic).First();

            if (member.MemberType == MemberTypes.Property) {
                return Expression.Lambda<Func<T, object>>(Expression.TypeAs(Expression.Property(p, name), typeof(object)), p);
            } else if (member.MemberType == MemberTypes.Field) {
                return Expression.Lambda<Func<T, object>>(Expression.TypeAs(Expression.Field(p, name), typeof(object)), p);
            } else {
                throw new NotSupportedException();
            }

        }

        /// <summary>
        /// 產生存取Expression <see cref="Func{T, TResult}"/>
        /// </summary>
        /// <typeparam name="T">存取目標類別</typeparam>
        /// <typeparam name="R">存取結果類別</typeparam>
        /// <param name="name">屬性或欄位名稱</param>
        /// <returns>存取Expression <see cref="Func{T, TResult}"/></returns>
        public static Expression<Func<T, R>> CreateAccessExpressionFunc<T, R>(string name) {
            var p = Expression.Parameter(typeof(T), "x");
            var member = typeof(T).GetMember(name,
               BindingFlags.Instance |
               BindingFlags.Static |
               BindingFlags.Public |
               BindingFlags.NonPublic).First();

            if (member.MemberType == MemberTypes.Property) {
                return Expression.Lambda<Func<T, R>>(Expression.Property(p, name), p);
            } else if (member.MemberType == MemberTypes.Field) {
                return Expression.Lambda<Func<T, R>>(Expression.Field(p, name), p);
            } else {
                throw new NotSupportedException();
            }
        }
    }
}
