using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XWidget.Linq {
    public static class ProcessedQueryableExtension {
        /// <summary>
        /// 查詢處理
        /// </summary>
        /// <typeparam name="T">元素類型</typeparam>
        /// <param name="obj">查詢</param>
        /// <param name="process">處理程序</param>
        /// <returns>隱含處理的查詢物件</returns>
        public static ProcessedQueryable<T> Process<T>(this IQueryable<T> obj, Func<T, T> process) {
            return new ProcessedQueryable<T>() {
                Source = obj,
                Process = process
            };
        }
    }
}
