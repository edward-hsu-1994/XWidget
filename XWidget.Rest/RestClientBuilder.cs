using System;
using System.Collections.Generic;
using Castle.DynamicProxy;

namespace XWidget.Rest {
    /// <summary>
    /// RESTful API客戶端建構器
    /// </summary>
    /// <typeparam name="T">定義類型</typeparam>
    public class RestClientBuilder<T>
        where T : class {
        /// <summary>
        /// 預設目標連結
        /// </summary>
        public string DefultBaseHref { get; set; }

        public RestClientBuilder() { }

        /// <summary>
        /// 建構客戶端實例
        /// </summary>
        /// <returns></returns>
        public T Build() {
            if (typeof(T).IsClass) {
                return new ProxyGenerator().CreateClassProxy<T>(new RestInterceptor());
            } else if (typeof(T).IsInterface) {
                return new ProxyGenerator().CreateInterfaceProxyWithoutTarget<T>(new RestInterceptor());
            } else {
                throw new NotSupportedException();
            }
        }
    }
}
