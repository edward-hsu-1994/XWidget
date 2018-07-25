using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace XWidget.Reflection {
    /// <summary>
    /// 針對<see cref="MethodInfo"/>的擴充方法
    /// </summary>
    public static class MethodInfoExtension {
        /// <summary>
        /// 用指定的參數與泛型參數，叫用目前執行個體所表示的方法或建構函式
        /// </summary>
        /// <param name="methodBase">目前實例</param>
        /// <param name="genericTypes">泛型參數</param>
        /// <param name="parameters">參數</param>
        /// <returns>引動結果</returns>
        public static object InvokeGeneric(this MethodBase methodBase, Type[] genericTypes, object[] parameters) {
            if (!methodBase.IsGenericMethod) throw new NotSupportedException("必須為泛型方法");
            var methodInfo = methodBase as MethodInfo;
            return methodInfo.MakeGenericMethod(genericTypes).Invoke(parameters);
        }

        /// <summary>
        /// 用指定的參數與泛型參數，叫用目前執行個體所表示的方法或建構函式
        /// </summary>
        /// <param name="methodBase">目前實例</param>
        /// <param name="genericTypes">泛型參數</param>
        /// <param name="instance">引動實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>引動結果</returns>
        public static object InvokeGeneric(this MethodBase methodBase, Type[] genericTypes, object instance, object[] parameters) {
            if (!methodBase.IsGenericMethod) throw new NotSupportedException("必須為泛型方法");
            var methodInfo = methodBase as MethodInfo;
            return methodInfo.MakeGenericMethod(genericTypes).Invoke(instance, parameters);
        }

        /// <summary>
        /// 用指定的參數，叫用目前執行個體所表示的方法或建構函式
        /// </summary>
        /// <param name="methodBase">目前實例</param>
        /// <param name="parameters">參數</param>
        /// <returns>引動結果</returns>
        public static object Invoke(this MethodBase methodBase, params object[] parameters) {
            return methodBase.Invoke(null, parameters);
        }

        /// <summary>
        /// 用指定的參數與泛型參數，叫用目前執行個體所表示的方法或建構函式
        /// </summary>
        /// <param name="methodBase">目前實例</param>
        /// <param name="genericTypes">泛型參數</param>
        /// <param name="parameters">參數</param>
        /// <returns>引動結果</returns>
        public static object Invoke(this MethodInfo methodBase, Type[] genericTypes, params object[] parameters) {
            return methodBase.MakeGenericMethod(genericTypes).Invoke(parameters);
        }

        /// <summary>
        /// 嘗試將MemberInfo轉換為MethodInfo並用指定的參數，叫用目前執行個體所表示的方法或建構函式
        /// </summary>
        /// <param name="info">目前實例</param>
        /// <param name="instance">實例</param>
        /// <param name="result">結果</param>
        /// <param name="parameters">參數</param>
        /// <returns>是否成功引動</returns>
        public static bool AsInvoke(this MemberInfo info, object instance, out object result, params object[] parameters) {
            if (info is MethodInfo) {
                try {
                    result = ((MethodInfo)info).Invoke(instance, parameters);
                } catch {
                    result = null;
                    return false;
                }
                return true;
            } else {
                result = default(object);
                return false;
            }
        }

        /// <summary>
        /// 將目前執行個體所表示的方法或建構函式轉換為Func委派。
        /// </summary>
        /// <param name="info">目前實例</param>
        /// <param name="instance">實例</param>
        /// <returns>引動結果</returns>
        public static Func<object[], object> ToDelegate(this MethodInfo info, object instance = null) {
            return delegate (object[] parameters) {
                return info.Invoke(instance, parameters);
            };
        }
    }
}
