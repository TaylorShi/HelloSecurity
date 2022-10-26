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

            // ������һ����վ�ű������ķ���վ������Token
            services.AddAntiforgery(antiforgeryOptions =>
            {
                antiforgeryOptions.HeaderName = "X-CSRF-TOKEN";
            });
            // ��ȫ�ַ�Χ�ڿ�չ����վ�ű���������
            //services.AddMvc(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

            // ����CORS����
            services.AddCors(corsOptions =>
            {
                // ����һ�����ֽ�api��Policy
                corsOptions.AddPolicy("api", corsPolicyBuilder =>
                {
                    corsPolicyBuilder
                    // �����������Դ��https://localhost:5005����Tesla.Order.CrossSite
                    .WithOrigins("https://localhost:5005")
                    // �����������κ�Header
                    .AllowAnyHeader()
                    // �����������κ�ƾ�ݣ�����������Զ�Я���������Cookie��Ϣ
                    .AllowCredentials()
                    // ����ű����Է��ʵ�Header���б�
                    .WithExposedHeaders("abc");
                });

                // ����һ�����ֽ�api-v2��Policy
                corsOptions.AddPolicy("api-v2", corsPolicyBuilder =>
                {
                    corsPolicyBuilder
                    // �ж���Щ�������Ա����п�������
                    .SetIsOriginAllowed(origin => true)
                    // �����������κ�ƾ�ݣ�����������Զ�Я���������Cookie��Ϣ
                    .AllowCredentials()
                    // �����������κ�Header
                    .AllowAnyHeader();
                });
                
                // ȫ��Ĭ�ϲ���
                corsOptions.AddDefaultPolicy(corsPolicyBuilder =>
                {
                    corsPolicyBuilder
                    // �ж���Щ�������Ա����п�������
                    .SetIsOriginAllowed(origin => true)
                    // �����������κ�ƾ�ݣ�����������Զ�Я���������Cookie��Ϣ
                    .AllowCredentials()
                    // �����������κ�Header
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
