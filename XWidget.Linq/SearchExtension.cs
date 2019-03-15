using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XWidget.Linq {
    /// <summary>
    /// 搜尋擴充方法
    /// </summary>
    public static class SearchExtension {
        /// <summary>
        /// 在指定的屬性或表達式中搜尋符合條件的項目
        /// </summary>
        /// <typeparam name="TSource">元素類別</typeparam>
        /// <typeparam name="TSearch">搜尋屬性類別</typeparam>
        /// <param name="source">目前實例</param>
        /// <param name="cond">條件</param>
        /// <param name="keySelectors">主鍵選擇器</param>
        /// <returns>搜尋結果</returns>
        public static IEnumerable<TSource> Search<TSource, TSearch>(
            this IEnumerable<TSource> source,
            Func<TSearch, bool> cond,
            params Func<TSource, TSearch>[] keySelectors) {
            return source.Where(x =>
                keySelectors.Any(y => cond(y(x)))
            );
        }
    }
}
