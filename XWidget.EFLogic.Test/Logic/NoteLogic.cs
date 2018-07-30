using System;
using System.Collections.Generic;
using System.Text;
using XWidget.EFLogic.Test.Models;

namespace XWidget.EFLogic.Test.Logic {
    public class NoteLogic : LogicBase<TestContext, Note, Guid> {
        public NoteLogic(TestLogicManager manager, CategoryLogic categoryLogic) : base(manager) {

        }
    }
}
