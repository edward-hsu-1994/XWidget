using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.EFLogic {
    /// <summary>
    /// 停用連鎖機制，請使用using區塊宣告
    /// </summary> 
    public class DisableCascadeScope<TContext> : DisableCascadeScope<TContext, object[]>
        where TContext : DbContext {
        public DisableCascadeScope(LogicManagerBase<TContext> manager) : base(manager) {
        }
    }

    /// <summary>
    /// 停用連鎖機制，請使用using區塊宣告
    /// </summary> 
    public class DisableCascadeScope<TContext, TParameters> : IDisposable
        where TContext : DbContext {
        public LogicManagerBase<TContext, TParameters> Manager { get; private set; }

        public DisableCascadeScope(LogicManagerBase<TContext, TParameters> manager) {
            Manager = manager;
            Manager.DisableCascade = this;
        }

        public void Dispose() {
            Manager.DisableCascade = null;
        }
    }
}
