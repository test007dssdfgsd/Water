using ApiAll.Contex;
using ApiAll.Model;
using ApiAll.Repository;
using ApiAll.Repository.Interface;
using ApiAll.Service;
using ApiAll.Service.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using System;
using Newtonsoft;
using Microsoft.AspNetCore.Authentication.Certificate;
using System.IO;
using ApiAll.Settings;
using Microsoft.OpenApi.Models;

namespace ApiAll
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
            services.AddHttpClient();
            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddAuthentication(
        CertificateAuthenticationDefaults.AuthenticationScheme)
        .AddCertificate();
              services.AddCors(c =>
              {
                  c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin());
              });
            
            services.AddMvc();
            services.AddDbContext<ApplicationContext>(options =>
            options.UseNpgsql(connection,
            options => options.SetPostgresVersion(new Version(9, 6))));
            services.AddTransient<IAuthorizationService, AuthorizationService>();
            services.AddTransient<IBaseRepository<Authorization>, BaseRepository<Authorization>>();
            services.AddTransient<IBaseRepository<Company>, BaseRepository<Company>>();


            //services.AddDbContext<ApplicationContext>(opts => opts.UseNpgsql("Host=localhost;Port=5432;Database=allapi;Username=postgres;Password=BeK_159753"));
            // services.AddControllers();
            services.AddSwaggerGenNewtonsoftSupport();
            services.AddControllers().AddNewtonsoftJson(options =>
             options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddSwaggerGen(c =>
            {
                //filterdi ochirb turdim
               // c.DocumentFilter<CustomSwaggerFilter>();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ALL", Version = "v1" });
                c.SwaggerDoc("v6", new OpenApiInfo { Title = "ARCHIVE", Version = "v6" });
                c.SwaggerDoc("v9", new OpenApiInfo { Title = "WATER", Version = "v9" });
            });
            //BeK_159753


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();// For the wwwroot folder

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                            Path.Combine(Directory.GetCurrentDirectory(), "images")),
                RequestPath = "/images"
            });
            //Enable directory browsing
            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = new PhysicalFileProvider(
                            Path.Combine(Directory.GetCurrentDirectory(), "images")),
                RequestPath = "/images"
            });


            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials

             app.UseAuthentication();
             app.UseHttpsRedirection();
             app.UseSwagger();
             app.UseSwaggerUI(c =>
              {
                  c.SwaggerEndpoint("/swagger/v1/swagger.json", "ALL");
                  c.SwaggerEndpoint("/swagger/v6/swagger.json", "ARCHIVE");
                  c.SwaggerEndpoint("/swagger/v9/swagger.json", "WATER");

              });



            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


          
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                // Vue SPA fallback uchun (API route lar bo'lmagan barcha route lar uchun index.html qaytaradi)
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
