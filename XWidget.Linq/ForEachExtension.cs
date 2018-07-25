using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Linq {
    /// <summary>
    /// 針對Linq物件之ForEach擴充方法
    /// </summary>
    public static class ForEachExtension {
        /// <summary>
        /// 對列舉實利使用ForEach列舉取出元素並帶入指定方法執行
        /// </summary>
        /// <typeparam name="TSource">元素類別</typeparam>
        /// <param name="source">目前實例</param>
        /// <param name="action">執行內容</param>
        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action) {
            source.Select(x => {
                action(x);
                return default(object);
            }).ToArray();
        }

        /// <summary>
        /// 對列舉實利使用ForEach列舉取出元素以及索引並帶入指定方法執行
        /// </summary>
        /// <typeparam name="TSource">元素類別</typeparam>
        /// <param name="source">目前實例</param>
        /// <param name="action">執行內容(參數1為元素實例、參數2為索引值)</param>
        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource, int> action) {
            source.Select((x, i) => {
                action(x, i);
                return default(object);
            }).ToArray();
        }
    }
}
