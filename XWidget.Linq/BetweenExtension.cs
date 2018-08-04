using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace System.Linq {
    /// <summary>
    /// 範圍查詢擴充
    /// </summary>
    public static class BetweenExtension {
        /// <summary>
        /// 針對指定屬性取得符合指定值區間的成員
        /// </summary>
        /// <typeparam name="TSource">列舉元素類型</typeparam>
        /// <typeparam name="TProperty">條件屬性類型</typeparam>
        /// <param name="source">列舉來源</param>
        /// <param name="selector">查詢屬性</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>查詢結果</returns>
        public static IEnumerable<TSource> Between<TSource, TProperty>(
            this IEnumerable<TSource> source,
            Func<TSource, TProperty> selector,
            Nullable<TProperty> min,
            Nullable<TProperty> max)
            where TProperty : struct, IComparable {
            var result = source;

            if (min.HasValue) {
                result = result.Where(x => {
                    var temp = selector(x) as IComparable;
                    return temp.CompareTo(min.Value) >= 0;
                });
            }

            if (max.HasValue) {
                result = result.Where(x => {
                    var temp = selector(x) as IComparable;
                    return temp.CompareTo(max.Value) <= 0;
                });
            }

            return result;
        }
    }
}
