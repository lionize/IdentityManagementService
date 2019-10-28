// Copyright (c) Brock Allen & Dominick Baier. All rights reserved. Licensed under the Apache
// License, Version 2.0. See LICENSE in the project root for license information.

using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
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

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors(AllowSpecificCorsOrigins);

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/1.0/swagger.json", "API 1.0");
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseIdentityServer();

            app.UseEndpoints(opt =>
            {
                opt.MapControllers();
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("Users")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddControllers().AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.PropertyNamingPolicy = null;
                opt.JsonSerializerOptions.DictionaryKeyPolicy = null;
            });

            services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = false;
                o.ReportApiVersions = true;
            });
            services.AddVersionedApiExplorer(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = false;
                o.SubstituteApiVersionInUrl = true;
            });

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
                options.ConfigureDbContext = b => b.UseNpgsql(Configuration.GetConnectionString("Configuration"), sql => sql.MigrationsAssembly(migrationsAssembly));
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b => b.UseNpgsql(Configuration.GetConnectionString("Operational"), sql => sql.MigrationsAssembly(migrationsAssembly));

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
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<PlatformModule>();

            builder.RegisterType<AccountService>().As<IAccountService>().InstancePerLifetimeScope();
            builder.RegisterType<ShellCommands>().As<IShellCommands>().SingleInstance();
        }
    }
}