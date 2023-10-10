using Fanex.BetradarVirtualSports.Business.Models;
using Fanex.MailDelivery.Core.Models;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.BetradarVirtualSports.Business.Services;

public interface MailService
{
    Mail CreateMailDaily(string fileName);

    Mail CreateMailMonthly(string fileName, Product product);
}

public class MailServiceImp : MailService
{
    private readonly DateRangeService dateRangeService;

    public MailServiceImp(DateRangeService dateRangeService)
    {
        this.dateRangeService = dateRangeService;
    }

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
        var attachments = new List<MailAttachment>
        {
            new(fileName, FileExtension.MS_EXCEL)
        };

        var date = dateRangeService.DateRange();
        var subject = product == Product.VS2
            ? $"[Betradar Virtual Sports] - Monthly Report - {date.FromDate:MM/dd/yyyy} - {date.ToDate:MM/dd/yyyy}"
            : $"[Virtual Sports 3] - Monthly Report - {date.FromDate:MM/dd/yyyy} - {date.ToDate:MM/dd/yyyy}";
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
        var model = new Tuple<string>(date);
        var template = File.ReadAllText("Templates/Daily-Template.cshtml");
        const string tplKey = "Fanex.BetradarVirtualSports.Service-Daily";
        var result = Engine.Razor.IsTemplateCached(tplKey, null)
            ? Engine.Razor.Run(tplKey, null, model)
            : Engine.Razor.RunCompile(template, tplKey, model: model);

        return result;
    }

    private string LoadBodyMonthly(Product product)
    {
        var date = dateRangeService.DateRange();
        var fromDate = $"{date.FromDate:MM/dd/yyyy}";
        var toDate = $"{date.ToDate:MM/dd/yyyy}";
        var model = new Tuple<string, string>(fromDate, toDate);
        var template = File.ReadAllText($"Templates/Monthly-{product}-Template.cshtml");
        var tplKey = $"Fanex.BetradarVirtualSports.Service-Monthly-{product}";
        var result = Engine.Razor.IsTemplateCached(tplKey, null)
            ? Engine.Razor.Run(tplKey, null, model)
            : Engine.Razor.RunCompile(template, tplKey, model: model);

        return result;
    }
}