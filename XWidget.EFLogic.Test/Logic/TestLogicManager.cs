using System;
using System.Collections.Generic;
using System.Text;
using XWidget.EFLogic.Test.Models;

namespace XWidget.EFLogic.Test.Logic {
    public class TestLogicManager : LogicManagerBase<TestContext> {

        public TestLogicManager(TestContext dbContext) : base(dbContext) { }

    }
}
