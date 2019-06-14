using Castle.DynamicProxy;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace XWidget.PropertyHook {
    /// <summary>
    /// 屬性掛勾回呼函數
    /// </summary>
    /// <typeparam name="T">物件類型</typeparam>
    /// <param name="sender">物件</param>
    /// <param name="indexs">索引子參數</param>
    /// <param name="value">值</param>
    public delegate void PropertyHookCallback<T>(T sender, object[] indexs, ref object value);
    public partial class PropertyHookInjector<T>
        where T : class {
        PropertyHookInterceptor<T> Interceptor { get; set; }

        /// <summary>
        /// 建立物件注射器
        /// </summary>
        public PropertyHookInjector() {
            Interceptor = new PropertyHookInterceptor<T>();
        }

        /// <summary>
        /// 加入GetBefore屬性掛勾
        /// </summary>
        /// <typeparam name="TProperty">屬性類型</typeparam>
        /// <param name="selector">屬性選擇器</param>
        /// <param name="callback">掛勾回呼</param>
        /// <returns>物件注射器</returns>
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

        /// <summary>
        /// 加入GetAfter屬性掛勾
        /// </summary>
        /// <typeparam name="TProperty">屬性類型</typeparam>
        /// <param name="selector">屬性選擇器</param>
        /// <param name="callback">掛勾回呼</param>
        /// <returns>物件注射器</returns>
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

        /// <summary>
        /// 加入SetBefore屬性掛勾
        /// </summary>
        /// <typeparam name="TProperty">屬性類型</typeparam>
        /// <param name="selector">屬性選擇器</param>
        /// <param name="callback">掛勾回呼</param>
        /// <returns>物件注射器</returns>
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

        /// <summary>
        /// 加入SetAfter屬性掛勾
        /// </summary>
        /// <typeparam name="TProperty">屬性類型</typeparam>
        /// <param name="selector">屬性選擇器</param>
        /// <param name="callback">掛勾回呼</param>
        /// <returns>物件注射器</returns>
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
