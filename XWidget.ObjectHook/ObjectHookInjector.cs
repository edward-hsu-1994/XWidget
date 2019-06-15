using Castle.DynamicProxy;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace XWidget.ObjectHook {
    /// <summary>
    /// 屬性掛勾回呼函數
    /// </summary>
    /// <typeparam name="T">物件類型</typeparam>
    /// <param name="sender">物件</param>
    /// <param name="indexs">索引子參數</param>
    /// <param name="value">值</param>
    public delegate void PropertyHookCallback<T>(T sender, object[] indexs, ref object value);

    /// <summary>
    /// 方法掛勾回呼函數
    /// </summary>
    /// <typeparam name="T">物件類型</typeparam>
    /// <param name="sender">物件</param>
    /// <param name="parameters">方法參數</param>
    public delegate void MethodHookCallback<T>(T sender, object[] parameters);

    /// <summary>
    /// 物件掛勾注射器
    /// </summary>
    /// <typeparam name="T">注射類型</typeparam>
    public partial class ObjectHookInjector<T>
        where T : class {
        PropertyHookInterceptor<T> Interceptor { get; set; }

        /// <summary>
        /// 建立物件注射器
        /// </summary>
        public ObjectHookInjector() {
            Interceptor = new PropertyHookInterceptor<T>();
        }

        /// <summary>
        /// 加入GetBefore屬性掛勾
        /// </summary>
        /// <typeparam name="TProperty">屬性類型</typeparam>
        /// <param name="selector">屬性選擇器</param>
        /// <param name="callback">掛勾回呼</param>
        /// <returns>物件注射器</returns>
        public ObjectHookInjector<T> HookGetPropertyBefore<TProperty>(
            Expression<Func<T, TProperty>> selector,
            PropertyHookCallback<T> callback) {

            if (selector.Body.NodeType == ExpressionType.Call) {
                var methodCallExpression = selector.Body as MethodCallExpression;
                Interceptor.PropertyBeforeCallbackDict[new HookMethodInfo() { Type = MethodType.IndexerGatter, Method = methodCallExpression.Method }] = callback;
            } else if (selector.Body.NodeType == ExpressionType.MemberAccess) {
                var memberException = selector.Body as MemberExpression;
                Interceptor.PropertyBeforeCallbackDict[new HookMethodInfo() { Type = MethodType.PropertyGatter, Method = ((PropertyInfo)memberException.Member).GetMethod }] = callback;
            }

            return this;
        }

        /// <summary>
        /// 加入GetAfter屬性掛勾
        /// </summary>
        /// <typeparam name="TProperty">屬性類型</typeparam>
        /// <param name="selector">屬性選擇器</param>
        /// <param name="callback">掛勾回呼</param>
        /// <returns>物件注射器</returns>
        public ObjectHookInjector<T> HookGetPropertyAfter<TProperty>(
            Expression<Func<T, TProperty>> selector,
            PropertyHookCallback<T> callback) {

            if (selector.Body.NodeType == ExpressionType.Call) {
                var methodCallExpression = selector.Body as MethodCallExpression;
                Interceptor.PropertyAfterCallbackDict[new HookMethodInfo() { Type = MethodType.IndexerGatter, Method = methodCallExpression.Method }] = callback;
            } else if (selector.Body.NodeType == ExpressionType.MemberAccess) {
                var memberException = selector.Body as MemberExpression;
                Interceptor.PropertyAfterCallbackDict[new HookMethodInfo() { Type = MethodType.PropertyGatter, Method = ((PropertyInfo)memberException.Member).GetMethod }] = callback;
            }

            return this;
        }

        /// <summary>
        /// 加入SetBefore屬性掛勾
        /// </summary>
        /// <typeparam name="TProperty">屬性類型</typeparam>
        /// <param name="selector">屬性選擇器</param>
        /// <param name="callback">掛勾回呼</param>
        /// <returns>物件注射器</returns>
        public ObjectHookInjector<T> HookSetPropertyBefore<TProperty>(
            Expression<Func<T, TProperty>> selector,
            PropertyHookCallback<T> callback) {

            if (selector.Body.NodeType == ExpressionType.Call) {
                var methodCallExpression = selector.Body as MethodCallExpression;

                var property = typeof(T).GetProperties().Single(x => x.GetMethod == methodCallExpression.Method || x.SetMethod == methodCallExpression.Method);

                Interceptor.PropertyBeforeCallbackDict[new HookMethodInfo() { Type = MethodType.IndexerSetter, Method = property.SetMethod }] = callback;
            } else if (selector.Body.NodeType == ExpressionType.MemberAccess) {
                var memberException = selector.Body as MemberExpression;

                Interceptor.PropertyBeforeCallbackDict[new HookMethodInfo() { Type = MethodType.PropertySetter, Method = ((PropertyInfo)memberException.Member).SetMethod }] = callback;
            }

            return this;
        }

        /// <summary>
        /// 加入SetAfter屬性掛勾
        /// </summary>
        /// <typeparam name="TProperty">屬性類型</typeparam>
        /// <param name="selector">屬性選擇器</param>
        /// <param name="callback">掛勾回呼</param>
        /// <returns>物件注射器</returns>
        public ObjectHookInjector<T> HookSetPropertyAfter<TProperty>(
            Expression<Func<T, TProperty>> selector,
            PropertyHookCallback<T> callback) {

            if (selector.Body.NodeType == ExpressionType.Call) {
                var methodCallExpression = selector.Body as MethodCallExpression;

                var property = typeof(T).GetProperties().Single(x => x.GetMethod == methodCallExpression.Method || x.SetMethod == methodCallExpression.Method);

                Interceptor.PropertyAfterCallbackDict[new HookMethodInfo() { Type = MethodType.IndexerSetter, Method = property.SetMethod }] = callback;
            } else if (selector.Body.NodeType == ExpressionType.MemberAccess) {
                var memberException = selector.Body as MemberExpression;

                Interceptor.PropertyAfterCallbackDict[new HookMethodInfo() { Type = MethodType.PropertySetter, Method = ((PropertyInfo)memberException.Member).SetMethod }] = callback;
            }

            return this;
        }

        public ObjectHookInjector<T> HookMethodInvokeBefore(
            Expression<Action<T>> selector,
            MethodHookCallback<T> callback) {

            if (selector.Body.NodeType == ExpressionType.Call) {
                var methodCallExpression = selector.Body as MethodCallExpression;

                Interceptor.MethodBeforeCallbackDict[new HookMethodInfo() { Type = MethodType.Default, Method = methodCallExpression.Method }] = callback;
            }

            return this;
        }

        public ObjectHookInjector<T> HookMethodInvokeAfter(
           Expression<Action<T>> selector,
           MethodHookCallback<T> callback) {

            if (selector.Body.NodeType == ExpressionType.Call) {
                var methodCallExpression = selector.Body as MethodCallExpression;

                Interceptor.MethodAfterCallbackDict[new HookMethodInfo() { Type = MethodType.Default, Method = methodCallExpression.Method }] = callback;
            }

            return this;
        }

        /// <summary>
        /// 注入掛勾
        /// </summary>
        /// <param name="targetObject">目標注射物件</param>
        /// <returns>注入後的Proxy物件</returns>
        public T Inject(T targetObject) {
            return new Castle.DynamicProxy.ProxyGenerator()
                    .CreateClassProxyWithTarget(
                        targetObject,
                        Interceptor.UseFor(targetObject));
        }
    }
}
