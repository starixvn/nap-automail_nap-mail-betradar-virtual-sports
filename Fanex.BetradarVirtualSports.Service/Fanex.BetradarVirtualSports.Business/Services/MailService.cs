using Fanex.BetradarVirtualSports.Business.Models;
using Fanex.MailDelivery.Core.Models;
using RazorEngineCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace Fanex.BetradarVirtualSports.Business.Services;

public interface MailService
{
    Mail CreateMailDaily(string fileName);

    Mail CreateMailMonthly(string fileName, Product product);
}

public class MailServiceImp(DateRangeService dateRangeService) : MailService
{
    private static readonly ConcurrentDictionary<int, IRazorEngineCompiledTemplate> CompiledTemplateCache = new();

    public Mail CreateMailDaily(string fileName)
    {
        var attachments = new List<MailAttachment>
        {
            new(fileName, FileExtension.MS_EXCEL)
        };

        return new Mail
        {
            Title = "ONEworks Auto Generated Report Administrator",
            Subject = $"[Betradar Virtual Sports] - Daily Report - {dateRangeService.DateRange().RptDate:MM/dd/yyyy}",
            Body = LoadBodyDaily(),
            Attachments = attachments
        };
    }

    public Mail CreateMailMonthly(string fileName, Product product)
    {
        var subjects = new Dictionary<Product, string>
        {
            { Product.VS2, "[Betradar Virtual Sports] - Monthly Report" },
            { Product.VS3, "[Virtual Sports 3] - Monthly Report" },
            { Product.VS4, "[Virtual Sports 4] - Monthly Report" },
            { Product.VS5, "[Virtual Sports 5] - Monthly Report" },
            { Product.VS7, "[Virtual Sports 7] - Monthly Report" },
        };

        var attachments = new List<MailAttachment>
        {
            new(fileName, FileExtension.MS_EXCEL)
        };

        var date = dateRangeService.DateRange();
        var subject = $"{subjects[product]} - {date.FromDate:MM/dd/yyyy} - {date.ToDate:MM/dd/yyyy}";
        return new Mail
        {
            Title = "ONEworks Auto Generated Report Administrator",
            Subject = subject,
            Body = LoadBodyMonthly(product),
            Attachments = attachments
        };
    }

    private string LoadBodyDaily()
    {
        var date = $"{dateRangeService.DateRange().RptDate:MM/dd/yyyy}";
        var tuple = new Tuple<string>(date);
        const string templatePath = "Templates/Daily-Template.cshtml";
        var hashCode = templatePath.GetHashCode();
        var compiledTemplate = CompiledTemplateCache.GetOrAdd(hashCode, _ =>
        {
            var template = File.ReadAllText(templatePath);
            var razorEngine = new RazorEngine();
            return razorEngine.Compile(template);
        });

        return compiledTemplate.Run(tuple);
    }

    private string LoadBodyMonthly(Product product)
    {
        var date = dateRangeService.DateRange();
        var fromDate = $"{date.FromDate:MM/dd/yyyy}";
        var toDate = $"{date.ToDate:MM/dd/yyyy}";
        var tuple = new Tuple<string, string>(fromDate, toDate);
        var templatePath = $"Templates/Monthly-{product}-Template.cshtml";
        var hashCode = templatePath.GetHashCode();
        var compiledTemplate = CompiledTemplateCache.GetOrAdd(hashCode, _ =>
        {
            var template = File.ReadAllText(templatePath);
            var razorEngine = new RazorEngine();
            return razorEngine.Compile(template);
        });

        return compiledTemplate.Run(tuple);
    }
}