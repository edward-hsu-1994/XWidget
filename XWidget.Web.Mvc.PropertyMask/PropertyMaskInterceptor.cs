using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace XWidget.Web.Mvc.PropertyMask {
    internal class PropertyMaskInterceptor : IInterceptor {
        public List<string> MaskedProperties { get; set; } = new List<string>();
        public Dictionary<string, object> ReplaceProperties { get; set; } = new Dictionary<string, object>();
        public void Intercept(IInvocation invocation) {
            if (invocation.Method.Name.StartsWith("set_") ||
                invocation.Method.Name.StartsWith("get_")) {
                var propertyName = invocation.Method.Name.Split(new char[] { '_' }, 2)[1];

                if (MaskedProperties.Contains(propertyName)) {
                    invocation.ReturnValue = invocation.Method.ReturnType.IsValueType ? Activator.CreateInstance(invocation.Method.ReturnType) : null;
                    return;
                }

                if (invocation.Method.Name.StartsWith("get_") ||
                    ReplaceProperties.ContainsKey(propertyName)) {
                    invocation.ReturnValue = ReplaceProperties[propertyName];
                    return;
                }
            }
            invocation.Proceed();
        }
    }
}