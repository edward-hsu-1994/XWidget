using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace System.Linq {
    /// <summary>
    /// 最大最小值查詢擴充
    /// </summary>
    public static class MaxMinExpressionExtension {
        /// <summary>
        /// 取得選擇屬性最大值或預設值
        /// </summary>
        /// <typeparam name="TSource">元素類別</typeparam>
        /// <typeparam name="TKey">排序主鍵類別</typeparam>
        /// <param name="source">目前實例</param>
        /// <returns>最大值或預設值</returns>
        public static TKey MaxOrDefault<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> selector) {
            return source.OrderByDescending(selector).Select(selector).FirstOrDefault();
        }

        /// <summary>
        /// 取得選擇屬性最小值或預設值
        /// </summary>
        /// <typeparam name="TSource">元素類別</typeparam>
        /// <typeparam name="TKey">排序主鍵類別</typeparam>
        /// <param name="source">目前實例</param>
        /// <returns>最小值或預設值</returns>
        public static TKey MinOrDefault<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> selector) {
            return source.OrderBy(selector).Select(selector).FirstOrDefault();
        }
    }
}
