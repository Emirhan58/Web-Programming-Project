using WebProgrammingProject.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebProgrammingProject.Entity;
using Microsoft.AspNetCore.Identity;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

namespace BuildingFormsWeb
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ApplicationIdentityDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("MsSqlConnection"))
            );

            services.AddLocalization(opt => { opt.ResourcesPath = "Resources"; });

            services.Configure<RequestLocalizationOptions>(
                opt =>
                {
                    var supportedCulteres = new List<CultureInfo>
                    {
                        new CultureInfo("en"),
                        new CultureInfo("tr")
                    };
                    opt.DefaultRequestCulture = new RequestCulture("en");
                    opt.SupportedCultures = supportedCulteres;
                    opt.SupportedUICultures = supportedCulteres;
                });

            services.AddTransient<ApplicationIdentityDbContext>();
            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 7;
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc(option => option.EnableEndpointRouting = false)
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Latest);
                
                
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                var scope = app.ApplicationServices.CreateScope();
                DataSeeding seeder = new DataSeeding(scope.ServiceProvider.GetService<ApplicationIdentityDbContext>());
                DataSeeding.Seed(app);
                seeder.SeedAdminUserAndRoles();
            }

            app.UseRouting();
            app.UseStaticFiles(); // wwwroot

            app.UseStaticFiles(new StaticFileOptions
            { 
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(),"node_modules")),
                RequestPath="/modules"
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                name: "DefaultApi",
                template: "{api:Exists}/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                    );
                
            });
        }

        
    }
}
