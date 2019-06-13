using Castle.DynamicProxy;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace XWidget.PropertyHook {
    public delegate void PropertyHookCallback<T>(T sender, object[] indexs, ref object value);
    public partial class PropertyHookInjector<T>
        where T : class {
        public T OrigionObject { get; private set; }

        public PropertyHookInterceptor<T> Interceptor { get; set; }

        public PropertyHookInjector(T origionObject) {
            OrigionObject = origionObject;
            Interceptor = new PropertyHookInterceptor<T>();
            Interceptor.OrigionObject = origionObject;
        }

        public PropertyHookInjector<T> HookGetBeforeProperty<TProperty>(
            Expression<Func<T, TProperty>> selector,
            PropertyHookCallback<T> callback) {

            if (selector.Body.NodeType == ExpressionType.Call) {
                var methodCallExpression = selector.Body as MethodCallExpression;
                Interceptor.MethodBeforeInfoCallbackDict[(true, false, methodCallExpression.Method)] = callback;
            } else if (selector.Body.NodeType == ExpressionType.MemberAccess) {
                var memberException = selector.Body as MemberExpression;
                Interceptor.MethodBeforeInfoCallbackDict[(false, false, ((PropertyInfo)memberException.Member).GetMethod)] = callback;
            }

            return this;
        }

        public PropertyHookInjector<T> HookGetAfterProperty<TProperty>(
            Expression<Func<T, TProperty>> selector,
            PropertyHookCallback<T> callback) {

            if (selector.Body.NodeType == ExpressionType.Call) {
                var methodCallExpression = selector.Body as MethodCallExpression;
                Interceptor.MethodAfterInfoCallbackDict[(true, false, methodCallExpression.Method)] = callback;
            } else if (selector.Body.NodeType == ExpressionType.MemberAccess) {
                var memberException = selector.Body as MemberExpression;
                Interceptor.MethodAfterInfoCallbackDict[(false, false, ((PropertyInfo)memberException.Member).GetMethod)] = callback;
            }

            return this;
        }

        public PropertyHookInjector<T> HookSetBeforeProperty<TProperty>(
            Expression<Func<T, TProperty>> selector,
            PropertyHookCallback<T> callback) {

            if (selector.Body.NodeType == ExpressionType.Call) {
                var methodCallExpression = selector.Body as MethodCallExpression;

                var property = typeof(T).GetProperties().Single(x => x.GetMethod == methodCallExpression.Method || x.SetMethod == methodCallExpression.Method);

                Interceptor.MethodBeforeInfoCallbackDict[(true, true, property.SetMethod)] = callback;
            } else if (selector.Body.NodeType == ExpressionType.MemberAccess) {
                var memberException = selector.Body as MemberExpression;
                Interceptor.MethodBeforeInfoCallbackDict[(false, true, ((PropertyInfo)memberException.Member).SetMethod)] = callback;
            }

            return this;
        }

        public PropertyHookInjector<T> HookSetAfterProperty<TProperty>(
            Expression<Func<T, TProperty>> selector,
            PropertyHookCallback<T> callback) {

            if (selector.Body.NodeType == ExpressionType.Call) {
                var methodCallExpression = selector.Body as MethodCallExpression;

                var property = typeof(T).GetProperties().Single(x => x.GetMethod == methodCallExpression.Method || x.SetMethod == methodCallExpression.Method);

                Interceptor.MethodAfterInfoCallbackDict[(true, true, property.SetMethod)] = callback;
            } else if (selector.Body.NodeType == ExpressionType.MemberAccess) {
                var memberException = selector.Body as MemberExpression;
                Interceptor.MethodAfterInfoCallbackDict[(false, true, ((PropertyInfo)memberException.Member).SetMethod)] = callback;
            }

            return this;
        }

        public T Inject() {
            return new Castle.DynamicProxy.ProxyGenerator()
                    .CreateClassProxyWithTarget(
                        OrigionObject,
                        Interceptor);
        }
    }
}
