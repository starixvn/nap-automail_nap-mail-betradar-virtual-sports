using Fanex.BetradarVirtualSports.Business.Models;
using Fanex.BetradarVirtualSports.Business.Services;
using Fanex.Data.Repository;
using Fanex.MailDelivery.Client;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.IO;

namespace Fanex.BetradarVirtualSports.Service;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection Services { get; set; } = null!;

    public static void AddConfig(this IServiceCollection services)
    {
        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        var text = File.ReadAllText("appsettings.json");
        AppConfigService.Config = JsonConvert.DeserializeObject<ConfigSettingModel>(text);
    }

    public static void AddInternalService(this IServiceCollection services)
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

        try
        {
            MailClientHandler.Start();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}