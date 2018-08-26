using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace XWidget.Linq.Test {
    public class PagingTest {
        [Fact(DisplayName = "PagingTest")]
        public void Paging() {
            var paging = new Paging<int>(Enumerable.Range(0, 100), 0, 10);


            Assert.Equal(paging.Result, Enumerable.Range(0, 10));

            paging.MovePage(1);

            Assert.Equal(paging.Result, Enumerable.Range(10, 10));

            var newPaging = paging.GetMovePage(3);

            Assert.Equal(newPaging.Result, Enumerable.Range(40, 10));

            newPaging.Reset();
            paging.Reset();

            Assert.Equal(newPaging.Result, paging.Result);

            var noPaging = new Paging<int>(Enumerable.Range(0, 100), 0, -1);
            Assert.Equal(noPaging.Result, Enumerable.Range(0, 100));

            Assert.Equal(Enumerable.Range(0, 100).AsPaging().Result, Enumerable.Range(0, 10));

            Assert.Equal(Enumerable.Range(0, 100).AsPaging().GetMoveToPage(1).Result, Enumerable.Range(10, 10));
        }
    }
}
