﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YoApp.Data.Extensions;
using YoApp.Utils.Extensions;
using YoApp.Data;
using YoApp.Data.Repositories;

namespace YoApp.Friends
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; private set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", reloadOnChange: true, optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Persistence and connection strings.
            services.AddEntityFramework(Configuration["ConnectionStrings:DefaultConnection"]);

            //Set App wide protection keyring.
            var section = Configuration.GetSection("Blobs:keyring");
            services.ConfigureDataProtectionOnAzure("YoApp", section["Account"], section["Secret"]);

            // Add framework services.
            services.AddMvc();

            //IoC
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IVerificationTokensRepository, VerificationTokensRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddAzureWebAppDiagnostics();

            app.UseOAuthValidation();
            app.UseMvc();
        }
    }
}
