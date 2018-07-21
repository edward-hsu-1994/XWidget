using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace XWidget.Utilities.Test {
    public class TaskUtilityTest {
        [Fact(DisplayName = "TaskUtility.LimitedTask")]
        public async Task LimitedTask() {
            var ok = await TaskUtility.LimitedTask(() => {
                Thread.Sleep(1000);
            }, 1);

            Assert.False(ok);
        }
    }
}
