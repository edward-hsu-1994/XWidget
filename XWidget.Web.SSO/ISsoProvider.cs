using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XWidget.Web.SSO {
    public interface ISsoProvider {
        string Name { get; }

        ISsoConfiguration Configuration { get; }
        Task<string> GetLoginUrlAsync(HttpContext context);

        Task<bool> VerifyCallbackRequest(HttpContext context);

        Task<string> GetLoginCallbackTokenAsync(HttpContext context);

        Task<bool> VerifyTokenAsync(string token);
    }
}
