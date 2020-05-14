using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using CRM.Services;
using CRM.Models.Entities;
using Microsoft.EntityFrameworkCore;
using CRM.Services.ViewModels;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CRM
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


            services.AddControllersWithViews();
            //services.AddMvc();
            services.AddMvc().AddRazorRuntimeCompilation();

            services.Configure<SmtpOptions>(Configuration);
            services.Configure<ContactOptions>(Configuration);
            services.Configure<AuthOptions>(Configuration);

            var connectionString = Configuration.GetConnectionString("CRMDatabase");
            services.AddDbContext<CRMContext>(options => options.UseMySql(connectionString));

            //services.AddDbContext<CRMContext>(options => options.UseSqlServer(connectionString));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            services.AddSingleton<Smtp, Smtp>();
            services.AddSingleton<Cryptography>();
            services.AddScoped<EZAuth>();
            services.AddScoped<EZSession>();

            services.AddTransient<CustomerService, CustomerService>();
            services.AddTransient<CampaignService, CampaignService>();
            services.AddTransient<PackageService, PackageService>();
            services.AddTransient<UserService, UserService>();

            services.AddSession();
 
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
              
            })
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                  
				});
            
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

               

            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();

                //endpoints.MapControllerRoute(
                //name: "Customer",
                //pattern: "Customer/{action}/{id?}",
                //defaults: new { controller = "Customer", action = "Index" });

                //endpoints.MapControllerRoute(
                //name: "Customer",
                //pattern: "Customer/Page/{currentPage?}/{pagesize?}",
                //defaults: new { controller = "Customer", action = "Page" });

                //endpoints.MapControllerRoute(
                //    name: "UserManagement",
                //    pattern: "Management/User/{action}/{id?}",
                //    defaults: new { controller = "UserManagement", action = "Index" });

                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller=Home}/{action=Index}/{id?}");

                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller=Account}/{action=Login}");
            });
        }
    }
}
