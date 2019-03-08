using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.EFLogic {
    /// <summary>
    /// 安全刪除範圍
    /// </summary>
    public class SafeRemoveCascadeScope<TContext> : SafeRemoveCascadeScope<TContext, object[]>
        where TContext : DbContext {
        public SafeRemoveCascadeScope(
            LogicManagerBase<TContext> manager,
            params Type[] types) : base(manager, types) {
        }
    }

    /// <summary>
    /// 安全刪除範圍
    /// </summary>
    public class SafeRemoveCascadeScope<TContext, TParameters> : IDisposable
        where TContext : DbContext {
        public LogicManagerBase<TContext, TParameters> Manager { get; private set; }
        public Type[] Types { get; private set; }
        public SafeRemoveCascadeScope(
            LogicManagerBase<TContext, TParameters> manager,
            params Type[] types) {
            Manager = manager;
            Types = types;
            Manager.SafeRemoveCascade = this;
        }

        public void Dispose() {
            Manager.SafeRemoveCascade = null;
        }
    }
}
