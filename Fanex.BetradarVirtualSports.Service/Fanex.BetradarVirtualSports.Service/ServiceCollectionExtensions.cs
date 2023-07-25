using Fanex.BetradarVirtualSports.Business.Models;
using Fanex.BetradarVirtualSports.Business.Services;
using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.MailDelivery.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OfficeOpenXml;
using SharpRaven;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fanex.BetradarVirtualSports.Service;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection Services { get; set; }

    public static void LoadConfiguration(this IServiceCollection services)
    {
        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        var text = File.ReadAllText("appsettings.json");
        AppConfigService.Config = JsonConvert.DeserializeObject<ConfigSettingModel>(text);
    }

    public static void SentryRegister(this IServiceCollection services)
    {
        var sentry = new SentryLogging(new SentryEngineOptions
        {
            Dsn = new Dsn(AppConfigService.Config.SentryClientKey)
        });

        LogManager
            .SetDefaultLogCategory(AppConfigService.Config.CategorySentryLog)
            .Use(new List<ILogEngine>
            {
                sentry
            });
    }

    public static void FanexDataRegister(this IServiceCollection services, IConfiguration config)
    {
        var connections = config.GetSection("ConnectionStrings")
            .GetChildren()
            .ToDictionary(
                x => x.Key,
                y => new ConnectionConfiguration(y.Key, y.Value)
            );

        DbSettingProviderManager
            .StartNewSession()
            .Use(connections)
            .WithSqlServer()
            .UseDefaultDbSettingProvider(config.GetSection("StoredProcedurePath").Value)
            .Run();
    }

    public static void InternalServiceRegister(this IServiceCollection services)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        services.AddSingleton<Log, LogImp>();
        services.AddSingleton<IDynamicRepository, DynamicRepository>();
        services.AddSingleton<DataService, DataServiceImp>();
        services.AddSingleton<DateRangeService, DateRangeServiceImp>();
        services.AddSingleton<MainDailyService, MainDailyServiceImp>();
        services.AddSingleton<MainMonthlyService, MainMonthlyServiceImp>();
        services.AddSingleton<MailService, MailServiceImp>();
        services.AddSingleton<ExcelDailyService, ExcelDailyServiceImp>();
        services.AddSingleton<ExcelMonthlyService, ExcelMonthlyServiceImp>();

        Services = services;
        MailClientHandler.Start();
    }
}