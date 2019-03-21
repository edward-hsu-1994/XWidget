using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XWidget.Linq {
    /// <summary>
    /// 分頁模型
    /// </summary>
    /// <typeparam name="TSource">列舉成員類型</typeparam>
    public interface IPagingModel<TSource> {
        /// <summary>
        /// 起始索引
        /// </summary>
        int Skip { get; }

        /// <summary>
        /// 取得筆數
        /// </summary>
        int Take { get; }

        /// <summary>
        /// 資料總筆數
        /// </summary>
        int TotalCount { get; }

        /// <summary>
        /// 目前所在分頁索引
        /// </summary>
        int CurrentPageIndex { get; }

        /// <summary>
        /// 總頁數
        /// </summary>
        int TotalPageCount { get; }

        /// <summary>
        /// 是否有上一頁
        /// </summary>
        bool HasPreviousPage { get; }

        /// <summary>
        /// 是否有下一頁
        /// </summary>
        bool HasNextPage { get; }
    }
}
