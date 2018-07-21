using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace XWidget.Utilities.Test {
    public class DateTimeUtilityTest {
        [Fact(DisplayName = "DateTimeUtility.UnixTimestamp")]
        public void UnixTimestamp() {
            DateTime now = DateTime.Now.ToUniversalTime();
            DateTime now2 = DateTimeUtility.FromUnixTimestamp(DateTimeUtility.ToUnixTimestamp(now));

            Assert.Equal(now.ToString("yyyyMMdd-HHmmss"), now2.ToString("yyyyMMdd-HHmmss"));
        }
    }
}
