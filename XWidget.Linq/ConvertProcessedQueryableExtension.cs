using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XWidget.Linq {
    public static class ConvertProcessedQueryableExtension {
        /// <summary>
        /// 查詢轉換處理
        /// </summary>
        /// <typeparam name="Tin">輸入元素類型</typeparam>
        /// <typeparam name="Tout">輸出元素類型</typeparam>
        /// <param name="obj">查詢</param>
        /// <param name="process">處理程序</param>
        /// <returns>隱含處理的查詢物件</returns>
        public static ConvertProcessedQueryable<Tin, Tout> ConvertProcess<Tin, Tout>(this IQueryable<Tin> obj, Func<Tin, Tout> process) {
            return new ConvertProcessedQueryable<Tin, Tout>() {
                Source = obj,
                Process = process
            };
        }
    }
}
