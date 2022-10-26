using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tesla.Order.NormalSite
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = "/home/login";
                    options.Cookie.HttpOnly = true;
                });
            services.AddControllersWithViews();

            // 设置了一个跨站脚本攻击的防跨站攻击的Token
            services.AddAntiforgery(antiforgeryOptions =>
            {
                antiforgeryOptions.HeaderName = "X-CSRF-TOKEN";
            });
            // 在全局范围内开展防跨站脚本攻击策略
            //services.AddMvc(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

            // 启用CORS服务
            services.AddCors(corsOptions =>
            {
                // 定义一个名字叫api的Policy
                corsOptions.AddPolicy("api", corsPolicyBuilder =>
                {
                    corsPolicyBuilder
                    // 允许它跨域的源是https://localhost:5005，即Tesla.Order.CrossSite
                    .WithOrigins("https://localhost:5005")
                    // 允许它发起任何Header
                    .AllowAnyHeader()
                    // 允许它发起任何凭据，发起请求会自动携带域下面的Cookie信息
                    .AllowCredentials()
                    // 允许脚本可以访问到Header的列表
                    .WithExposedHeaders("abc");
                });

                // 定义一个名字叫api-v2的Policy
                corsOptions.AddPolicy("api-v2", corsPolicyBuilder =>
                {
                    corsPolicyBuilder
                    // 判断哪些域名可以被运行跨域请求
                    .SetIsOriginAllowed(origin => true)
                    // 允许它发起任何凭据，发起请求会自动携带域下面的Cookie信息
                    .AllowCredentials()
                    // 允许它发起任何Header
                    .AllowAnyHeader();
                });
                
                // 全局默认策略
                corsOptions.AddDefaultPolicy(corsPolicyBuilder =>
                {
                    corsPolicyBuilder
                    // 判断哪些域名可以被运行跨域请求
                    .SetIsOriginAllowed(origin => true)
                    // 允许它发起任何凭据，发起请求会自动携带域下面的Cookie信息
                    .AllowCredentials()
                    // 允许它发起任何Header
                    .AllowAnyHeader();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
