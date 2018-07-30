using System;
using System.Collections.Generic;
using System.Text;
using XWidget.EFLogic.Test.Models;

namespace XWidget.EFLogic.Test.Logic {
    public class CategoryLogic : LogicBase<TestContext, Category, Guid> {
        public CategoryLogic(TestLogicManager manager) : base(manager) {

        }
    }
}
