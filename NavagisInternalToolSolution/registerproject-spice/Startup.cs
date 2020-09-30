using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using Google.Apis.Auth.OAuth2;
using Google.Apis.CloudResourceManager.v1;
using Google.Apis.Services;
using Newtonsoft.Json;
using Data = Google.Apis.CloudResourceManager.v1.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Google.Apis.Util.Store;
using RegisterProject_Spice.Pages.Models;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;

namespace RegisterProject_Spice
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

          //  services.AddHttpsRedirection(options => options.HttpsPort = 5002);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //    services.AddDefaultIdentity<ApplicationUser>()
            //.AddEntityFrameworkStores<ApplicationDbContext>()
            //.AddDefaultTokenProviders();

            services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = 443;
            });

            
            var DefaultConnection = Configuration.GetConnectionString("NavagisProjectBillingDBConnection");
            services.AddDbContextPool<ApplicationDbContext>(options => options.UseSqlite(DefaultConnection));
            
            services.AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped(typeof(IDesignTimeDbContextFactory<ApplicationDbContext>), typeof(AppDbContextFactory));

            //services.AddAuthentication().AddGoogle(googleOptions =>
            //{
            //    googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
            //    googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            //});


            services.AddSession();
            services.AddMemoryCache();
            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithRedirects("/404.html");
                app.UseExceptionHandler("CustomMessages/SystemError");
                app.UseHsts();                
            }

           // app.UseHttpsRedirection();
           
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    public class AppDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        ApplicationDbContext IDesignTimeDbContextFactory<ApplicationDbContext>.CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("NavagisProjectBillingDBConnection");

            builder.UseSqlite(connectionString);

            return new ApplicationDbContext(builder.Options);
        }
    }
}
