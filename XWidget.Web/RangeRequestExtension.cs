using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection {
    /// <summary>
    /// Range Request功能擴充
    /// </summary>
    public static class RangeRequestExtension {
        /// <summary>
        /// 啟用Range Request
        /// </summary>
        public static IServiceCollection EnableRangeRequest(this IServiceCollection service) {
            AppContext.SetSwitch("Switch.Microsoft.AspNetCore.Mvc.EnableRangeProcessing", true);
            return service;
        }
    }
}
