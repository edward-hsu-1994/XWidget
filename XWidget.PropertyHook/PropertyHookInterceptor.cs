using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace XWidget.PropertyHook {
    public class PropertyHookInterceptor<T> : IInterceptor
        where T : class {
        public T OrigionObject { get; set; }

        public Dictionary<(bool indexer, bool setter, MethodInfo method), PropertyHookCallback<T>> MethodBeforeInfoCallbackDict { get; set; }
            = new Dictionary<(bool indexer, bool setter, MethodInfo method), PropertyHookCallback<T>>();

        public Dictionary<(bool indexer, bool setter, MethodInfo method), PropertyHookCallback<T>> MethodAfterInfoCallbackDict { get; set; }
            = new Dictionary<(bool indexer, bool setter, MethodInfo method), PropertyHookCallback<T>>();

        public void Intercept(IInvocation invocation) {
            if (MethodBeforeInfoCallbackDict.Any(x => x.Key.method == invocation.Method)) {
                var method = MethodBeforeInfoCallbackDict.Single(x => x.Key.method == invocation.Method);
                List<object> indexs = new List<object>();
                if (method.Key.indexer) {
                    indexs.AddRange(invocation.Arguments.Take(method.Key.method.GetParameters().Length - 1));
                }
                if (method.Key.setter) {
                    var value = invocation.Arguments.LastOrDefault();
                    MethodBeforeInfoCallbackDict[method.Key].Invoke(OrigionObject, indexs.ToArray(), ref value);
                    if (invocation.Arguments.Any()) {
                        invocation.SetArgumentValue(invocation.Arguments.Length - 1, value);
                    }
                    invocation.ReturnValue = value;
                } else {
                    var value = invocation.ReturnValue;
                    MethodBeforeInfoCallbackDict[method.Key].Invoke(OrigionObject, indexs.ToArray(), ref value);
                    invocation.ReturnValue = value;
                }
            }

            invocation.Proceed();

            if (MethodAfterInfoCallbackDict.Any(x => x.Key.method == invocation.Method)) {
                var method = MethodAfterInfoCallbackDict.Single(x => x.Key.method == invocation.Method);
                List<object> indexs = new List<object>();
                if (method.Key.indexer) {
                    indexs.AddRange(invocation.Arguments.Take(method.Key.method.GetParameters().Length - 1));
                }
                if (method.Key.setter) {
                    var value = invocation.Arguments.LastOrDefault();
                    MethodAfterInfoCallbackDict[method.Key].Invoke(OrigionObject, indexs.ToArray(), ref value);
                    if (invocation.Arguments.Any()) {
                        invocation.SetArgumentValue(invocation.Arguments.Length - 1, value);
                    }
                    invocation.ReturnValue = value;
                } else {
                    var value = invocation.ReturnValue;
                    MethodAfterInfoCallbackDict[method.Key].Invoke(OrigionObject, indexs.ToArray(), ref value);
                    invocation.ReturnValue = value;
                }
            }
        }
    }
}
