using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web.Mvc {
    /// <summary>
    /// 分頁結果
    /// </summary>
    public class PaginationResult<T> where T : IEnumerable {
        /// <summary>
        /// 起始索引
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// 取得筆數
        /// </summary>
        public int Take { get; set; }

        /// <summary>
        /// 資料總數
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 是否還有下一頁
        /// </summary>
        public bool HasNext => Skip + Take < TotalCount;

        /// <summary>
        /// 分頁結果
        /// </summary>
        public T Result { get; set; }
    }
}
