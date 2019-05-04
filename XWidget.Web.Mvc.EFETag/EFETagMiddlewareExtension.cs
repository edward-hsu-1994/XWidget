using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XWidget.Web.Mvc.EFETag {
    public static class EFETagMiddlewareExtension {
        public static IApplicationBuilder UseEFETag<TContext>(this IApplicationBuilder app)
            where TContext : DbContext {
            return app.UseResponseBuffering()
                .UseMiddleware<EFETagMiddleware<TContext>>();
        }
    }
}
