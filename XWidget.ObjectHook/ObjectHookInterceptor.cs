using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace XWidget.ObjectHook {
    internal class PropertyHookInterceptor<T> : IInterceptor
        where T : class {
        public T TargetObject { get; set; }

        public Dictionary<HookMethodInfo, PropertyHookCallback<T>> PropertyBeforeCallbackDict { get; set; }
            = new Dictionary<HookMethodInfo, PropertyHookCallback<T>>();

        public Dictionary<HookMethodInfo, PropertyHookCallback<T>> PropertyAfterCallbackDict { get; set; }
            = new Dictionary<HookMethodInfo, PropertyHookCallback<T>>();

        public Dictionary<HookMethodInfo, MethodHookCallback<T>> MethodBeforeCallbackDict { get; set; }
            = new Dictionary<HookMethodInfo, MethodHookCallback<T>>();

        public Dictionary<HookMethodInfo, MethodHookCallback<T>> MethodAfterCallbackDict { get; set; }
            = new Dictionary<HookMethodInfo, MethodHookCallback<T>>();


        public void Intercept(IInvocation invocation) {
            if (PropertyBeforeCallbackDict.Any(x => x.Key.Method == invocation.Method)) {
                var method = PropertyBeforeCallbackDict.Single(x => x.Key.Method == invocation.Method);
                List<object> indexs = new List<object>();
                if (method.Key.Type == MethodType.IndexerGatter ||
                    method.Key.Type == MethodType.IndexerSetter) {
                    indexs.AddRange(invocation.Arguments.Take(method.Key.Method.GetParameters().Length - 1));
                }
                if (method.Key.Type == MethodType.PropertySetter ||
                    method.Key.Type == MethodType.IndexerSetter) {
                    var value = invocation.Arguments.LastOrDefault();
                    PropertyBeforeCallbackDict[method.Key].Invoke(TargetObject, indexs.ToArray(), ref value);
                    if (invocation.Arguments.Any()) {
                        invocation.SetArgumentValue(invocation.Arguments.Length - 1, value);
                    }
                    invocation.ReturnValue = value;
                } else if (method.Key.Type == MethodType.PropertyGatter ||
                           method.Key.Type == MethodType.IndexerGatter) {
                    var value = invocation.ReturnValue;
                    PropertyBeforeCallbackDict[method.Key].Invoke(TargetObject, indexs.ToArray(), ref value);
                    invocation.ReturnValue = value;
                }
            } else if (MethodBeforeCallbackDict.Any(x => x.Key.Method == invocation.Method)) {
                var method = MethodBeforeCallbackDict.Single(x => x.Key.Method == invocation.Method);
                method.Value.Invoke(TargetObject, invocation.Arguments);
            }

            invocation.Proceed();

            if (PropertyAfterCallbackDict.Any(x => x.Key.Method == invocation.Method)) {
                var method = PropertyAfterCallbackDict.Single(x => x.Key.Method == invocation.Method);
                List<object> indexs = new List<object>();
                if (method.Key.Type == MethodType.IndexerGatter ||
                    method.Key.Type == MethodType.IndexerSetter) {
                    indexs.AddRange(invocation.Arguments.Take(method.Key.Method.GetParameters().Length - 1));
                }
                if (method.Key.Type == MethodType.PropertySetter ||
                    method.Key.Type == MethodType.IndexerSetter) {
                    var value = invocation.Arguments.LastOrDefault();
                    PropertyAfterCallbackDict[method.Key].Invoke(TargetObject, indexs.ToArray(), ref value);
                    if (invocation.Arguments.Any()) {
                        invocation.SetArgumentValue(invocation.Arguments.Length - 1, value);
                    }
                    invocation.ReturnValue = value;
                } else if (method.Key.Type == MethodType.PropertyGatter ||
                           method.Key.Type == MethodType.IndexerGatter) {
                    var value = invocation.ReturnValue;
                    PropertyAfterCallbackDict[method.Key].Invoke(TargetObject, indexs.ToArray(), ref value);
                    invocation.ReturnValue = value;
                }
            } else if (MethodAfterCallbackDict.Any(x => x.Key.Method == invocation.Method)) {
                var method = MethodAfterCallbackDict.Single(x => x.Key.Method == invocation.Method);
                method.Value.Invoke(TargetObject, invocation.Arguments);
            }
        }

        public PropertyHookInterceptor<T> UseFor(T targetObject) {
            var result = new PropertyHookInterceptor<T>();
            result.TargetObject = targetObject;
            result.PropertyBeforeCallbackDict = this.PropertyBeforeCallbackDict.ToDictionary(x => x.Key, x => x.Value);
            result.PropertyAfterCallbackDict = this.PropertyAfterCallbackDict.ToDictionary(x => x.Key, x => x.Value);

            return result;
        }
    }
}
