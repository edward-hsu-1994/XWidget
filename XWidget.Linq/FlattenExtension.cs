using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XWidget.Linq {
    /// <summary>
    /// 扁平化擴充
    /// </summary>
    public static class FlattenExtension {
        /// <summary>
        /// 將列舉依照制定方式扁平化
        /// </summary>
        /// <typeparam name="TSource">列舉元素類型</typeparam>
        /// <param name="source">列舉來源</param>
        /// <param name="selector">查詢屬性</param>
        /// <returns>扁平化樹狀結構路徑</returns>
        public static IEnumerable<IEnumerable<TSource>> Flatten<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TSource>> selector) {
            var result = source;

            return result.SelectMany(x => {
                var paths = selector(x);

                if (paths == null || paths.Count() == 0) {
                    return new TSource[][]{
                        new TSource[]{ x }
                    };
                }

                return paths.Flatten(selector).Select(y => new TSource[] { x }.Concat(y));
            });
        }

        /// <summary>
        /// 將列舉依照制定方式扁平化取出所有元素
        /// </summary>
        /// <typeparam name="TSource">列舉元素類型</typeparam>
        /// <param name="source">列舉來源</param>
        /// <param name="selector">查詢屬性</param>
        /// <returns>列舉中扁平化後所有元素</returns>
        public static IEnumerable<TSource> FlattenMany<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TSource>> selector) {
            var result = source;

            return result.SelectMany(x => {
                var paths = selector(x);

                if (paths == null || paths.Count() == 0) {
                    return new TSource[] { x };
                }

                return new TSource[] { x }.Concat(paths.FlattenMany(selector));
            });
        }
    }
}