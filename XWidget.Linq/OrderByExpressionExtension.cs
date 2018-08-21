using XWidget.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace System.Linq {
    /// <summary>
    /// 針對Linq排序之擴充方法
    /// </summary>
    public static class OrderByExpressionExtension {
        /// <summary>
        /// 使用元素指定排序主鍵進行遞增排列
        /// </summary>
        /// <typeparam name="TSource">元素類別</typeparam>
        /// <typeparam name="TKey">排序主鍵類別</typeparam>
        /// <param name="source">目前實例</param>
        /// <param name="keySelectors">主鍵選擇器</param>
        /// <returns>使用指定KeySelectors排序結果</returns>
        public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(this IQueryable<TSource> source, (bool isDec, Expression<Func<TSource, TKey>> selector)[] keySelectors) {
            if (keySelectors.Length == 0) throw new ArgumentNullException($"{nameof(keySelectors)}不該為空");
            IOrderedQueryable<TSource> result = default(IOrderedQueryable<TSource>);
            if (keySelectors[0].isDec) {
                result = source.OrderByDescending(keySelectors[0].selector);
            } else {
                result = source.OrderBy(keySelectors[0].selector);
            }
            for (int i = 1; i < keySelectors.Length; i++) {
                if (keySelectors[i].isDec) {
                    result = result.ThenByDescending(keySelectors[i].selector);
                } else {
                    result = result.ThenBy(keySelectors[i].selector);
                }
            }
            return result;
        }

        /// <summary>
        /// 使用元素指定排序主鍵進行遞增或遞減排列
        /// </summary>
        /// <typeparam name="TSource">元素類別</typeparam>
        /// <param name="source">目前實例</param>
        /// <param name="keyNames">主鍵選擇器與排序方式</param>
        /// <returns>使用指定KeySelectors排序結果</returns>
        public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, (bool isDec, string name)[] keyNames) {
            if (keyNames.Length == 0) throw new ArgumentNullException($"{nameof(keyNames)}不該為空");

            var keySelectors = keyNames.Select(x => (isDec: x.isDec, selector: AccessExpressionUtility.CreateAccessFunc<TSource>(x.name)));

            return source.OrderBy(keySelectors.ToArray());
        }

        /// <summary>
        /// 使用元素指定排序主鍵進行遞增排列
        /// </summary>
        /// <typeparam name="TSource">元素類別</typeparam>
        /// <typeparam name="TKey">排序主鍵類別</typeparam>
        /// <param name="source">目前實例</param>
        /// <param name="keyNames">主鍵屬性名稱</param>
        /// <returns>使用指定KeySelectors排序結果</returns>
        public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, params string[] keyNames) {
            if (keyNames.Length == 0) throw new ArgumentNullException($"{nameof(keyNames)}不該為空");

            var keySelectors = keyNames.Select(x => (isDec: false, selector: AccessExpressionUtility.CreateAccessFunc<TSource>(x)));

            return source.OrderBy(keySelectors.ToArray());
        }

        /// <summary>
        /// 使用元素指定排序主鍵進行遞減排列
        /// </summary>
        /// <typeparam name="TSource">元素類別</typeparam>
        /// <typeparam name="TKey">排序主鍵類別</typeparam>
        /// <param name="source">目前實例</param>
        /// <param name="keyNames">主鍵屬性名稱</param>
        /// <returns>使用指定KeySelectors排序結果</returns>
        public static IOrderedQueryable<TSource> OrderByDescending<TSource>(this IQueryable<TSource> source, params string[] keyNames) {
            if (keyNames.Length == 0) throw new ArgumentNullException($"{nameof(keyNames)}不該為空");

            var keySelectors = keyNames.Select(x => (isDec: true, selector: AccessExpressionUtility.CreateAccessFunc<TSource>(x)));

            return source.OrderBy(keySelectors.ToArray());
        }

        /// <summary>
        /// 使用元素指定排序主鍵進行遞增排列
        /// </summary>
        /// <typeparam name="TSource">元素類別</typeparam>
        /// <typeparam name="TKey">排序主鍵類別</typeparam>
        /// <param name="source">目前實例</param>
        /// <param name="keySelectors">主鍵選擇器</param>
        /// <returns>使用指定KeySelectors排序結果</returns>
        public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, params Expression<Func<TSource, object>>[] keySelectors) {
            return source.OrderBy(keySelectors.Select(x => (isDec: false, selector: x)).ToArray());
        }

        /// <summary>
        /// 使用元素指定排序主鍵進行遞減排列
        /// </summary>
        /// <typeparam name="TSource">元素類別</typeparam>
        /// <typeparam name="TKey">排序主鍵類別</typeparam>
        /// <param name="source">目前實例</param>
        /// <param name="keySelectors">主鍵選擇器</param>
        /// <returns>使用指定KeySelectors排序結果</returns>
        public static IOrderedQueryable<TSource> OrderByDescending<TSource>(this IQueryable<TSource> source, params Expression<Func<TSource, object>>[] keySelectors) {
            return source.OrderBy(keySelectors.Select(x => (isDec: true, selector: x)).ToArray());
        }
    }
}
