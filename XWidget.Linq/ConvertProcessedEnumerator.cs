using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Linq {
    /// <summary>
    /// 可轉換列舉
    /// </summary>
    /// <typeparam name="Tin">輸入元素類型</typeparam>
    /// <typeparam name="Tout">輸出元素類型</typeparam>
    public class ConvertProcessedEnumerator<Tin, Tout> : IEnumerator<Tout> {
        public IEnumerator<Tin> Source { get; internal set; }

        public Func<Tin, Tout> Process { get; internal set; }

        public Tout Current => Process(Source.Current);

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
