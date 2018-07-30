using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.EFLogic {
    internal class InternalLogicManagerContainer<TContext>
        where TContext : DbContext {
        public LogicManagerBase<TContext> Manager { get; set; }
    }
}
