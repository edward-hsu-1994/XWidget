using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace XWidget.Linq.Test {
    public class ForEachExtensionTest {
        [Fact(DisplayName = "ForEachExtension.ForEach")]
        public void ForEach() {
            int i = 0;
            Enumerable.Range(1, 100).ForEach(x => {
                i++;
            });
            Assert.Equal(100, i);
        }

        [Fact(DisplayName = "ForEachExtension.ForEachAndIndex")]
        public void ForEachIndex() {
            int sum = 0;
            Enumerable.Range(1, 100).ForEach((x, i) => {
                sum += i + 1;
            });
            Assert.Equal(5050, sum);
        }
    }
}
