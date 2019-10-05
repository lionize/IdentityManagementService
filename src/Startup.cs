// Copyright (c) Brock Allen & Dominick Baier. All rights reserved. Licensed under the Apache
// License, Version 2.0. See LICENSE in the project root for license information.

using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using TIKSN.DependencyInjection;
using TIKSN.Lionize.IdentityManagementService.Data;
using TIKSN.Lionize.IdentityManagementService.Models;
using TIKSN.Lionize.IdentityManagementService.Services;
using TIKSN.Lionize.IdentityManagementService.Shell;

namespace TIKSN.Lionize.IdentityManagementService
{
    public class Startup
    {
        private readonly string AllowSpecificCorsOrigins = "_AllowSpecificCorsOrigins_";

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/1.0/swagger.json", "API 1.0");
            });

            app.UseCors(AllowSpecificCorsOrigins);

            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseMvcWithDefaultRoute();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Users")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddControllers();

            services.AddApiVersioning();
            services.AddVersionedApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("1.0", new OpenApiInfo { Title = "Lionize / Identity Management Service", Version = "1.0" });
            });

            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            .AddAspNetIdentity<ApplicationUser>()
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b => b.UseSqlServer(Configuration.GetConnectionString("Configuration"), sql => sql.MigrationsAssembly(migrationsAssembly));
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b => b.UseSqlServer(Configuration.GetConnectionString("Operational"), sql => sql.MigrationsAssembly(migrationsAssembly));

                options.EnableTokenCleanup = true;
            });

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                //TODO: pass the valid certificate.
                //throw new Exception("need to configure key material");
                builder.AddDeveloperSigningCredential();
            }

            services.AddAuthentication();

            services.AddCors(options =>
            {
                options.AddPolicy(AllowSpecificCorsOrigins,
                cpbuilder =>
                {
                    var origins = Configuration.GetSection("Cors").GetSection("Origins").Get<string[]>();

                    if (origins != null)
                    {
                        cpbuilder.AllowAnyMethod();
                        cpbuilder.AllowAnyHeader();
                        cpbuilder.WithOrigins(origins);
                    }
                });
            });

            services.AddFrameworkPlatform();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            ConfigureContainer(containerBuilder);

            return new AutofacServiceProvider(containerBuilder.Build());
        }

        private void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<PlatformModule>();

            builder.RegisterType<AccountService>().As<IAccountService>().InstancePerLifetimeScope();
            builder.RegisterType<ShellCommands>().As<IShellCommands>().SingleInstance();
        }
    }
}