using BuildingFormsWeb.Data;
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
using BuildingFormsWeb.Entity;
using Microsoft.AspNetCore.Identity;

namespace BuildingFormsWeb
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            //services.AddDbContext<ProductContext>(options =>
            //    //options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"))
            //    options.UseSqlServer(Configuration.GetConnectionString("MsSqlConnection"))
            //);

            services.AddDbContext<ApplicationIdentityDbContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("MsSqlConnection"))    
            );


            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 7;
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc(option => option.EnableEndpointRouting = false)
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Latest);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                DataSeeding.Seed(app);
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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                    );
            });
        }
    }
}
