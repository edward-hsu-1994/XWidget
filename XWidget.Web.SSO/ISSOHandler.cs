using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XWidget.Web.SSO {
    public interface ISSOHandler {
        Task OnBinding(HttpContext context);
        Task OnLogin(HttpContext context);
    }
}
