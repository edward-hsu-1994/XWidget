using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XWidget.Web.Test.Controllers;

namespace XWidget.Web.Test {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.EnableRangeRequest();

            services.AddScoped(typeof(Nullable<bool>), x => true);

            services.AddMvc()
                .AddApplicationPart(typeof(TestController).GetTypeInfo().Assembly)
                .AddControllersAsServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseNoCache();

            app.UseMvc();

            app.UseHtmlHandler(async (context, html) => {
                if (context.Request.Path == "/gg") {
                    return "<html>gg</html>";
                } else {
                    return html;
                }
            });

            app.Run(async (request) => {
                request.Response.ContentType = "text/html";

                var testHtml = "<html><head><base href=\"/\"></head><body><div>test</div>Content</body></html>";

                await request.Response.WriteAsync(testHtml);
            });
        }
    }
}
