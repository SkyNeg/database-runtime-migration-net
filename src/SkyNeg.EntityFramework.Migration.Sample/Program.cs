﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SkyNeg.EntityFramework.Migration;
using SkyNeg.EntityFramework.Migration.Sample;

await Host.CreateDefaultBuilder(args)
    .ConfigureLogging(options => options.AddConsole())
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        IHostEnvironment env = hostingContext.HostingEnvironment;
        config.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", true, true)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
        .AddEnvironmentVariables();
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddManagedDbContext<RuntimeContext>((options) =>
        {
            options.SetDbContextOptions(options => options.UseSqlite(@"Data Source=data.db"));
            options.AddResourceScriptProvider("Data.Create", "Data.Update");
        }, ServiceLifetime.Singleton);

        services.AddHostedService<DatabaseManagerService>();
    })
    .Build()
    .RunAsync();