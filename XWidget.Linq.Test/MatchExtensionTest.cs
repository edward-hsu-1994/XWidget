using XWidget.Linq.Test.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;

namespace XWidget.Linq.Test {
    public class MatchExtensionTest {
        [Fact(DisplayName = "MatchExtension.Match")]
        public void Match() {
            var list = Student.InitList();
            var groupedList = list.GroupBy("Class".BoxingToArray());

            Assert.Equal(list.Match(new {
                Class = "A"
            }), list.Where(x => x.Class == "A"));
        }
    }
}
