using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XWidget.Extensions.Test {
    public class IEnumerableExtensionTest {
        [Fact(DisplayName = "IEnumerableExtension.Split")]
        public void EnumerableSplit() {
            var segments = Enumerable.Range(0, 100).Split(10);

            Assert.Equal(segments.Count(), 10);
            Assert.Equal(segments.Sum(x => x.Count()), 100);
        }
    }
}
