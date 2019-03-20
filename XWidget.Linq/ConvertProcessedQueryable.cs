using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace XWidget.Linq {
    /// <summary>
    /// 可轉換查詢列舉
    /// </summary>
    /// <typeparam name="Tin">輸入元素類型</typeparam>
    /// <typeparam name="Tout">輸出元素類型</typeparam>
    public class ConvertProcessedQueryable<Tin, Tout> : IQueryable<Tout> {
        public IQueryable<Tin> Source { get; internal set; }

        public Func<Tin, Tout> Process { get; internal set; }

        public Type ElementType => Source.ElementType;

        public Expression Expression => Source.Expression;

        public IQueryProvider Provider => Source.Provider;

        public IEnumerator<Tout> GetEnumerator() {
            return new ConvertProcessedEnumerator<Tin, Tout>() {
                Source = Source.GetEnumerator(),
                Process = Process
            };
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
