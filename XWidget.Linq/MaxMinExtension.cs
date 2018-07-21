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
    public static class MaxMinExtension {
        /// <summary>
        /// 取得選擇屬性最大值或預設值
        /// </summary>
        /// <typeparam name="TSource">元素類別</typeparam>
        /// <typeparam name="TKey">排序主鍵類別</typeparam>
        /// <param name="source">目前實例</param>
        /// <param name="selector">選擇器</param>
        /// <returns>最大值或預設值</returns>
        public static TKey MaxOrDefault<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector) {
            return source.OrderByDescending(selector).Select(selector).FirstOrDefault();
        }

        /// <summary>
        /// 取得選擇屬性最小值或預設值
        /// </summary>
        /// <typeparam name="TSource">元素類別</typeparam>
        /// <typeparam name="TKey">排序主鍵類別</typeparam>
        /// <param name="source">目前實例</param>
        /// <param name="selector">選擇器</param>
        /// <returns>最小值或預設值</returns>
        public static TKey MinOrDefault<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector) {
            return source.OrderBy(selector).Select(selector).FirstOrDefault();
        }

        /// <summary>
        /// 取得選擇屬性最大值或預設值
        /// </summary>
        /// <typeparam name="TSource">元素類別</typeparam>
        /// <typeparam name="TKey">排序主鍵類別</typeparam>
        /// <param name="source">目前實例</param>
        /// <param name="selector">選擇器</param>
        /// <param name="defaultValue">預設值</param>
        /// <returns>最大值或預設值</returns>
        public static Nullable<TKey> MaxOrDefault<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> selector,
            Nullable<TKey> defaultValue)
            where TKey : struct {
            if (!source.Any()) return defaultValue;
            return source.MaxOrDefault(selector);
        }

        /// <summary>
        /// 取得選擇屬性最小值或預設值
        /// </summary>
        /// <typeparam name="TSource">元素類別</typeparam>
        /// <typeparam name="TKey">排序主鍵類別</typeparam>
        /// <param name="source">目前實例</param>
        /// <param name="selector">選擇器</param>
        /// <returns>最小值或預設值</returns>
        public static Nullable<TKey> MinOrDefault<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> selector,
            Nullable<TKey> defaultValue)
            where TKey : struct {
            if (!source.Any()) return defaultValue;
            return source.MaxOrDefault(selector);
        }
    }
}
