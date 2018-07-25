using System;
using System.Collections.Generic;
using System.Text;
using XWidget.Linq;

namespace System.Linq {
    /// <summary>
    /// 針對IEnumerable轉換為Paging的擴充
    /// </summary>
    public static class PagingExtension {
        /// <summary>
        /// 將列舉項目轉換為分頁類型
        /// </summary>
        /// <typeparam name="TSource">元素類型</typeparam>
        /// <param name="source">分頁資料來源</param>
        /// <param name="skip">起始索引</param>
        /// <param name="take">取得筆數</param>
        /// <returns>分頁結果</returns>
        public static Paging<TSource> AsPaging<TSource>(this IEnumerable<TSource> source, int skip = 0, int take = 10) {
            return new Paging<TSource>(source, skip, take);
        }
    }
}
