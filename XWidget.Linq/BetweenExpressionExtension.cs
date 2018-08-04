using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace System.Linq {
    /// <summary>
    /// 範圍查詢擴充
    /// </summary>
    public static class BetweenExpressionExtension {
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
        public static IQueryable<TSource> Between<TSource, TProperty>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, TProperty>> selector,
            Nullable<TProperty> min,
            Nullable<TProperty> max)
            where TProperty : struct, IComparable {
            var selectPropertyName = (selector.Body as MemberExpression)?.Member?.Name;

            var isParam = selector.Body is ParameterExpression;

            var result = source;

            var p = Expression.Parameter(typeof(TSource), "x");

            if (min.HasValue) {
                var minFilter = Expression.OrElse(
                    Expression.Equal(
                        Expression.Constant(min.Value, typeof(Nullable<TProperty>)),
                        Expression.Constant(null, typeof(Nullable<TProperty>))
                    ),
                    Expression.GreaterThanOrEqual(
                        Expression.Convert(
                            isParam ? p : (Expression)Expression.PropertyOrField(
                                p,
                                selectPropertyName
                            ),
                            typeof(Nullable<TProperty>)
                        ),
                        Expression.Constant(min.Value, typeof(Nullable<TProperty>))
                    )
                );
                result = result
                            .Where(Expression.Lambda<Func<TSource, bool>>(minFilter, p));
            }

            if (max.HasValue) {
                var maxFilter = Expression.OrElse(
                    Expression.Equal(
                        Expression.Constant(max, typeof(Nullable<TProperty>)),
                        Expression.Constant(null, typeof(Nullable<TProperty>))
                    ),
                    Expression.LessThanOrEqual(
                        Expression.Convert(
                            isParam ? p : (Expression)Expression.PropertyOrField(
                                p,
                                selectPropertyName
                            ),
                            typeof(Nullable<TProperty>)
                        ),
                        Expression.Constant(max, typeof(Nullable<TProperty>))
                    )
                );
                result = result
                            .Where(Expression.Lambda<Func<TSource, bool>>(maxFilter, p));
            }

            return result;
        }
    }
}
