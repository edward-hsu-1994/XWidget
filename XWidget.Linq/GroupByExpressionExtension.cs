using XWidget.Reflection;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace System.Linq {
    /// <summary>
    /// 針對Linq群組之擴充方法
    /// </summary>
    public static class GroupByExpressionExtension {
        /// <summary>
        /// 使用指定屬性進行Group操作
        /// </summary>
        /// <typeparam name="TSource">元素類別</typeparam>
        /// <typeparam name="TKey">主鍵類別</typeparam>
        /// <param name="source">目前實例</param>
        /// <param name="key">主鍵屬性名稱</param>
        /// <returns>使用指定key進行Group結果</returns>
        public static IQueryable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IQueryable<TSource> source, string key) {
            return source.GroupBy(AccessExpressionUtility.CreateAccessExpressionFunc<TSource, TKey>(key));
        }

        /// <summary>
        /// 使用指定屬性進行Group操作
        /// </summary>
        /// <typeparam name="TSource">元素類別</typeparam>
        /// <param name="source">目前實例</param>
        /// <param name="keys">主鍵屬性名稱</param>
        /// <returns>使用指定key進行Group結果</returns>
        public static IQueryable<IGrouping<object, TSource>> GroupBy<TSource>(this IQueryable<TSource> source, string[] keys) {
            if (keys.Length == 1) return source.GroupBy<TSource, object>(keys.First());

            dynamic obj = new ExpandoObject();
            List<MemberBinding> memberBindings = new List<MemberBinding>();
            var p = Expression.Parameter(typeof(TSource), "x");

            var dobj = obj as IDictionary<string, object>;
            foreach (var key in keys) {
                dobj[key] = null;
            }
            var type = (obj as ExpandoObject).CreateAnonymousType();

            foreach (var prop in type.GetProperties()) {
                memberBindings.Add(Expression.Bind(prop, Expression.Property(p, prop.Name)));
            }

            var createGroupObject = Expression.MemberInit(Expression.New(type), memberBindings);

            return source.GroupBy<TSource, object>(Expression.Lambda<Func<TSource, object>>(createGroupObject, p));
        }
    }
}
