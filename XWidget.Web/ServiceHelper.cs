using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace XWidget.Web {
    /// <summary>
    /// 提供執行階段取得目前DI物件的幫助類別
    /// </summary>
    public static class ServiceHelper {
        /// <summary>
        /// 取得目前執行階段指定服務
        /// </summary>
        /// <typeparam name="T">服務類型</typeparam>
        /// <returns>服務類型實例</returns>
        public static T GetService<T>() {
            return (T)ServiceProviderExtension.ServiceProvider.GetService(typeof(T));
        }

        /// <summary>
        /// 取得目前執行階段指定服務
        /// </summary>
        /// <param name="type">服務類型</param>
        /// <returns>服務類型實例</returns>
        public static object GetService(Type type) {
            return ServiceProviderExtension.ServiceProvider.GetService(type);
        }
    }
}
