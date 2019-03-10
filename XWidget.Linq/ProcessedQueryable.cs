using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace XWidget.Linq {
    public class ProcessedQueryable<T> : IQueryable<T> {
        public IQueryable<T> Source { get; internal set; }

        public Func<T, T> Process { get; internal set; }

        public Type ElementType => Source.ElementType;

        public Expression Expression => Source.Expression;

        public IQueryProvider Provider => Source.Provider;

        public IEnumerator<T> GetEnumerator() {
            return new ProcessedEnumerator<T>() {
                Source = Source.GetEnumerator(),
                Process = Process
            };
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
