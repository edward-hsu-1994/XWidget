using System;
using System.Linq.Expressions;

namespace XWidget.PropertyHook {
    public delegate void PropertyCallback<T, TProperty>(T sender, TProperty oldValue, TProperty newValue);
    public partial class PropertyHookInjector<T> {
        public T OrigionObject { get; private set; }

        private PropertyHookInterceptor<T> Interceptor { get; set; }

        public PropertyHookInjector(T origionObject) {
            OrigionObject = origionObject;
            Interceptor.OrigionObject = origionObject;
        }

        public PropertyHookInjector<T> HookGetBeforeProperty<TProperty>(
            Expression<Func<T, object>> selector,
            PropertyCallback<T, TProperty> callback) {
            throw new NotImplementedException();
        }

        public PropertyHookInjector<T> HookGetAfterProperty<TProperty>(
            Expression<Func<T, object>> selector,
            PropertyCallback<T, TProperty> callback) {
            throw new NotImplementedException();
        }

        public PropertyHookInjector<T> HookSetBeforeProperty<TProperty>(
            Expression<Func<T, object>> selector,
            PropertyCallback<T, TProperty> callback) {
            throw new NotImplementedException();
        }

        public PropertyHookInjector<T> HookSetAfterProperty<TProperty>(
            Expression<Func<T, object>> selector,
            PropertyCallback<T, TProperty> callback) {
            throw new NotImplementedException();
        }

        public T Inject() {
            throw new NotImplementedException();
        }
    }
}
