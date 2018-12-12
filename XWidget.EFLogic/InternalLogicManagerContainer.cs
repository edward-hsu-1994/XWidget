using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.EFLogic {
    internal class InternalLogicManagerContainer<TContext, TParameters>
        where TContext : DbContext {
        public LogicManagerBase<TContext, TParameters> Manager { get; set; }
    }
}
