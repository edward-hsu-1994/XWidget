using XWidget.Reflection;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace System.Linq {
    /// <summary>
    /// 針對Linq群組之擴充方法
    /// </summary>
    public static class GroupByExtension {
        /// <summary>
        /// 使用指定屬性進行Group操作
        /// </summary>
        /// <typeparam name="TSource">元素類別</typeparam>
        /// <typeparam name="TKey">主鍵類別</typeparam>
        /// <param name="source">目前實例</param>
        /// <param name="key">主鍵屬性名稱</param>
        /// <returns>使用指定key進行Group結果</returns>
        public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, string key) {
            return source.GroupBy(AccessExpressionUtility.CreateAccessFunc<TSource, TKey>(key));
        }

        /// <summary>
        /// 使用指定屬性進行Group操作
        /// </summary>
        /// <typeparam name="TSource">元素類別</typeparam>
        /// <param name="source">目前實例</param>
        /// <param name="keys">主鍵屬性名稱</param>
        /// <returns>使用指定key進行Group結果</returns>
        public static IEnumerable<IGrouping<object, TSource>> GroupBy<TSource>(this IEnumerable<TSource> source, string[] keys) {
            if (keys.Length == 1) return source.GroupBy<TSource, object>(keys.First());

            dynamic obj = new ExpandoObject();
            var dobj = obj as IDictionary<string, object>;
            foreach (var key in keys) {
                dobj[key] = null;
            }
            var type = (obj as ExpandoObject).CreateAnonymousType();

            return source.GroupBy<TSource, object>(x => {
                var result = Activator.CreateInstance(type);
                foreach (var property in type.GetProperties()) {
                    property.SetValue(result, x.GetType().GetProperty(property.Name).GetValue(x));
                }
                return result;
            });
        }
    }
}
