using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace XWidget.PropertyHook {
    public partial class PropertyHookInjector<T> {
        internal class PropertyHookInterceptor<T2> : IInterceptor {
            public T2 OrigionObject { get; set; }

            public void Intercept(IInvocation invocation) {
                invocation.Proceed();
            }
        }
    }
}
