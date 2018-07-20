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

            Assert.Equal(groupedList, list.GroupBy(x => x.Class));
        }
    }
}
