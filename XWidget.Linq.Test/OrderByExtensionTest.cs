using XWidget.Linq.Test.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;

namespace XWidget.Linq.Test {
    public class OrderByExtensionTest {
        [Fact(DisplayName = "OrderByExtension.OrderBy")]
        public void OrderBy() {
            var list = Student.InitList();
            var orderedList = list.OrderBy(new(bool isDec, string name)[] {
                (isDec : true,name: "Class"),
                (isDec : false,name: "Id"),
            });

            Assert.Equal(orderedList, list.OrderByDescending(x => x.Class).ThenBy(x => x.Id));
        }
    }
}
