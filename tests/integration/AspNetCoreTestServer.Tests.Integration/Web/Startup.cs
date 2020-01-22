using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreTestServer.Tests.Integration.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RazorViewEngineOptions>(options =>
                    {
                        options.ViewLocationFormats.Clear();
                        options.ViewLocationFormats.Add
                            ("/Web/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
                    })
                    .AddControllers()
                    .Services.AddMvc();
        }

        public void Configure([NotNull] IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting()
               .UseEndpoints(endpoints => endpoints.MapControllers())
               .UseStaticFiles()
               .UseDeveloperExceptionPage();
        }
    }
}