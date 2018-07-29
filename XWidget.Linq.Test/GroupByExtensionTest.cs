using XWidget.Linq.Test.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;

namespace XWidget.Linq.Test {
    public class GroupByExtensionTest {
        [Fact(DisplayName = "GroupByExtension.GroupBy")]
        public void GroupBy() {
            var list = Student.InitList();
            var groupedList = list.GroupBy("Class".BoxingToArray());

            Assert.Equal(list.GroupBy(x => x.Class), groupedList);

            var list2 = list.AsQueryable();
            var expressionGroupedList = list2.GroupBy("Class".BoxingToArray());

            Assert.Equal(list2.GroupBy(x => x.Class), expressionGroupedList);
        }
    }
}
