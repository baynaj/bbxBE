using bbxBE.Application;
using bbxBE.Application.BackgroundServices;
using bbxBE.Application.Commands;
using bbxBE.Application.Queries;
using bbxBE.Infrastructure.Persistence;
using bbxBE.Infrastructure.Shared;
using bbxBE.WebApi.Extensions;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Text.Json;

namespace bbxBE.WebApi
{
    public class Startup
    {
        public IConfiguration _config { get; }

        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCommonInfrastructure(_config);
            services.AddApplicationLayer();
            services.AddPersistenceInfrastructure(_config);
            services.AddCommandInfrastructure(_config);
            services.AddQueryInfrastructure(_config);
            services.AddSharedInfrastructure(_config);

            services.AddSwaggerExtension();
            services.AddControllersExtension();

            services.AddHostedServices(_config, "bbxBE.Application.BackgroundServices");


            // CORS
            services.AddCorsExtension();

            services.AddHealthChecks();


            //API Security


            services.AddJWTAuthentication(_config);

            services.AddAuthorizationPolicies(_config);
            // API version
            services.AddApiVersioningExtension();
            // API explorer
            services.AddMvcCore()
                .AddApiExplorer();

            // API explorer version
            services.AddVersionedApiExplorerExtension();

            services.AddMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
                //options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;

            });
            services.AddControllersWithViews();

            services.AddMvc().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            // also the following given it's a Web API project

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            services.AddHttpContextAccessor();

            services.AddAutoMapper(typeof(Startup));
            services.AddControllersWithViews();


            services.AddHangfire(x => x.UseSqlServerStorage(_config.GetConnectionString("bbxdbconnection")));
            services.AddHangfireServer();

            // using ILogger in DI
            services.AddSingleton(Log.Logger);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            //dbContext.Database.EnsureCreated();

            // Add this line; you'll need `using Serilog;` up the top, too
            app.UseRouting();

            app.UseSession();


            //Enable CORS
            app.UseCors("AllowAll");


           //ne legyen automatikus https app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwaggerExtension();
            app.UseErrorLoggingMiddleware();
            app.UseErrorHandlingMiddleware();
            app.UseHealthChecks("/health");


            app.UseEndpoints(endpoints =>
             {
                 endpoints.MapControllers();
             });

        }
    }
}