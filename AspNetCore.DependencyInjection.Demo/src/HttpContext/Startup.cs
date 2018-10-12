using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Services;

namespace HttpContext
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddHttpContextAccessor();

            services.AddScoped<LocalhostService>();
            services.AddScoped<CloudService>();

            services.AddScoped(serviceProvider => {

                var httpContext = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;

                if (httpContext == null)
                {
                    // Not HTTP request
                    return null;
                }

                //// Use any request data

                //var queryString = httpContext.Request.Query;

                //return queryString.ContainsKey("useLocal")
                //    ? serviceProvider.GetService<LocalhostService>() as IService
                //    : serviceProvider.GetService<CloudService>() as IService;

                //if (httpContext.Request.ContentType == "application/x-www-form-urlencoded")
                //{
                //    var formValues = httpContext.Request.Form;
                //}

                var requestHeaders = httpContext.Request.Headers;

                return requestHeaders.ContainsKey("Use-local")
                    ? serviceProvider.GetService<LocalhostService>() as IService
                    : serviceProvider.GetService<CloudService>() as IService;
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}