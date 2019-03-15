using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using XWidget.Linq.Test.Models;

namespace XWidget.Linq.Test {

    public class SearchExtensionTest {
        [Fact(DisplayName = "SearchExtensionTest.Search")]
        public void SearchTest() {
            var list = Student.InitList();

            var listResult = list.Search(
                x => x.ToUpper().Contains("A"),
                x => x.Name,
                x => x.Class.ToUpper()).ToList();

            Assert.Equal(
                list.Where(x => x.Name.ToUpper().Contains("A") || x.Class.ToUpper().ToUpper().Contains("A")),
                listResult);

            var queryList = Student.InitList().AsQueryable();

            var queryResult = queryList.Search(
                x => x.ToUpper().Contains("USER0"),
                x => x.Name,
                x => x.Class.ToUpper()).ToList();

            Assert.Equal(
                queryList.Where(x => x.Name.ToUpper().Contains("USER0") || x.Class.ToUpper().ToUpper().Contains("USER0")),
                queryResult);
        }
    }
}
