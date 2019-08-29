// Copyright (c) Brock Allen & Dominick Baier. All rights reserved. Licensed under the Apache
// License, Version 2.0. See LICENSE in the project root for license information.

using CommandLine;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Linq;
using System.Threading.Tasks;
using TIKSN.Lionize.IdentityManagementService.Data;
using TIKSN.Lionize.IdentityManagementService.Shell;

namespace TIKSN.Lionize.IdentityManagementService
{
    public class Program
    {
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                    .UseStartup<Startup>()
                    .UseSerilog((context, configuration) =>
                    {
                        configuration
                            .MinimumLevel.Debug()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                            .MinimumLevel.Override("System", LogEventLevel.Warning)
                            .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                            .Enrich.FromLogContext()
                            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate);
                    });
        }

        public static async Task Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.MigrateAsync();
            }

            using (var scope = host.Services.CreateScope())
            {
                await scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.MigrateAsync();
            }

            using (var scope = host.Services.CreateScope())
            {
                await scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.MigrateAsync();
            }

            await Parser.Default.ParseArguments<AddApiSecretOptions, AddClientSecretOptions, Sha256Options>(args)
                .MapResult(
                    (AddApiSecretOptions options) =>
                    {
                        using (var scope = host.Services.CreateScope())
                        {
                            return scope.ServiceProvider.GetRequiredService<IShellCommands>().AddApiSecretAsync(options);
                        }
                    },
                    (AddClientSecretOptions options) =>
                    {
                        using (var scope = host.Services.CreateScope())
                        {
                            return scope.ServiceProvider.GetRequiredService<IShellCommands>().AddClientSecretAsync(options);
                        }
                    },
                    (Sha256Options options) =>
                    {
                        using (var scope = host.Services.CreateScope())
                        {
                            return scope.ServiceProvider.GetRequiredService<IShellCommands>().Sha256Async(options);
                        }
                    },
                    errors =>
                    {
                        if (!errors.Any(x => x.StopsProcessing))
                            return host.RunAsync();
                        return Task.CompletedTask;
                    });
        }
    }
}