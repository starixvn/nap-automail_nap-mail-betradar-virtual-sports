using Fanex.BetradarVirtualSports.Business.Models;
using Fanex.MailDelivery.Core.Models;
using System;

namespace Fanex.BetradarVirtualSports.Business.Services;

public interface MainMonthlyService
{
    JobMessage Execute(Product product);
}

public class MainMonthlyServiceImp : MainMonthlyService
{
    private readonly Log log;
    private readonly DataService dataService;
    private readonly ExcelMonthlyService excelService;
    private readonly MailService mailService;

    public MainMonthlyServiceImp(
        Log log,
        DataService dataService,
        ExcelMonthlyService excelService,
        MailService mailService)
    {
        this.log = log;
        this.dataService = dataService;
        this.excelService = excelService;
        this.mailService = mailService;
    }

    public JobMessage Execute(Product product)
    {
        var result = new ExecuteResultModel();
        var jobMessage = new JobMessage();
        try
        {
            result = SendNotification(product);
        }
        catch (Exception e)
        {
            result.Success = false;
            result.Message = e.Message;
            log.Log(e);
        }
        finally
        {
            jobMessage.Mail = result.Mail;
            jobMessage.JobStatus = result.Success ? JobStatus.Success : JobStatus.DataIsNotReady;
            jobMessage.TaskLog = new TaskLog
            {
                LogTime = DateTime.Now,
                Message = result.Message
            };
        }

        return jobMessage;
    }

    private ExecuteResultModel SendNotification(Product product)
    {
        if (!dataService.IsDataAvailable())
        {
            return new ExecuteResultModel { Success = false, Message = "DataIsNotReady, TaskID = 38" };
        }

        var data = dataService.QueryMonthlyData(product);
        var excelPath = excelService.Export(data, product);
        var mail = mailService.CreateMailMonthly(excelPath, product);

        return new ExecuteResultModel
        {
            Mail = mail,
            Message = string.Empty,
            Success = true
        };
    }
}