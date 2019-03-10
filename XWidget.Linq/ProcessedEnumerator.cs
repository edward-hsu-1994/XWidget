using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Linq {
    public class ProcessedEnumerator<T> : IEnumerator<T> {
        public IEnumerator<T> Source { get; internal set; }

        public Func<T, T> Process { get; internal set; }

        public T Current => Process(Source.Current);

        object IEnumerator.Current => this.Current;

        public void Dispose() {
            Source.Dispose();
        }

        public bool MoveNext() {
            return Source.MoveNext();
        }

        public void Reset() {
            Source.Reset();
        }
    }
}
