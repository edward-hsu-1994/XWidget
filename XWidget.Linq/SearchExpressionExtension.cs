using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace XWidget.Linq {
    /// <summary>
    /// 搜尋擴充方法
    /// </summary>
    public static class SearchExpressionExtension {
        /// <summary>
        /// 在指定的屬性或表達式中搜尋符合條件的項目
        /// </summary>
        /// <typeparam name="TSource">元素類別</typeparam>
        /// <typeparam name="TSearch">搜尋屬性類別</typeparam>
        /// <param name="source">目前實例</param>
        /// <param name="cond">條件</param>
        /// <param name="keySelectors">主鍵選擇器</param>
        /// <returns>搜尋結果</returns>
        public static IQueryable<TSource> Search<TSource, TSearch>(
            this IQueryable<TSource> source,
            Expression<Func<TSearch, bool>> cond,
            params Expression<Func<TSource, TSearch>>[] keySelectors) {
            if (keySelectors.Length == 0) throw new ArgumentException($"{nameof(keySelectors)}.Length require >= 1");

            List<Expression> condExpList = new List<Expression>();
            ParameterExpression p = Expression.Parameter(typeof(TSource), "x");

            foreach (var keySelector in keySelectors) {
                condExpList.Add(Expression.Invoke(cond, Expression.Invoke(keySelector, p)));
            }

            // OR串接
            Expression AllOr(IEnumerable<Expression> exps) {
                Expression result = exps.First();

                foreach (var exp in exps.Skip(1)) {
                    result = Expression.OrElse(result, exp);
                }
                return result;
            }

            var queryExpression = Expression.Lambda<Func<TSource, bool>>(
                AllOr(condExpList), p
            );

            return source.Where(queryExpression);
        }
    }
}
