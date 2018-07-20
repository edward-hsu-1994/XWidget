using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection {
    /// <summary>
    /// 使用提供執行階段取得目前DI物件的幫助類別
    /// </summary>
    public static class ServiceHelperExtension {
        /// <summary>
        /// 使用執行階段取得目前DI物件幫助類別功能
        /// </summary>
        /// <param name="builder">應用程式建構器</param>
        public static void UseServiceHelper(this IApplicationBuilder builder) {
            builder.UseServiceProviderHelper();
        }
    }
}
