using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace XWidget.Web.AddThis.Test {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseAddThisShareButton("ra-5c3de839571af158");

            app.Run(async (request) => {
                request.Response.StatusCode = StatusCodes.Status200OK;
                request.Response.ContentType = "text/html";

                var testHtml = "<html><head></head><body><div>test</div>Content</body></html>";

                var testData = Encoding.UTF8.GetBytes(testHtml);

                request.Response.Body.Write(testData, 0, testData.Length);
            });
        }
    }
}
