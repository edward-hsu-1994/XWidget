using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XWidget.EFLogic.Test.Controllers;
using XWidget.EFLogic.Test.Logic;
using XWidget.EFLogic.Test.Models;

namespace XWidget.EFLogic.Test {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddScoped<TestContext>(x => TestContext.CreateInstance());

            services.AddLogic<TestLogicManager, TestContext>().AddFromDbContext();

            services.AddMvc()
                .AddApplicationPart(typeof(TestController).GetTypeInfo().Assembly)
                .AddControllersAsServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
