using Fanex.BetradarVirtualSports.Business.Models;
using Fanex.MailDelivery.Core.Models;
using System;

namespace Fanex.BetradarVirtualSports.Business.Services;

public interface MainDailyService
{
    JobMessage Execute();
}

public class MainDailyServiceImp : MainDailyService
{
    private readonly Log log;
    private readonly DataService dataService;
    private readonly ExcelDailyService excelService;
    private readonly MailService mailService;

    public MainDailyServiceImp(
        Log log,
        DataService dataService,
        ExcelDailyService excelService,
        MailService mailService)
    {
        this.log = log;
        this.dataService = dataService;
        this.excelService = excelService;
        this.mailService = mailService;
    }

    public JobMessage Execute()
    {
        var result = new ExecuteResultModel();
        var jobMessage = new JobMessage();
        try
        {
            AppConfigService.Counter.Add(1);
            result = SendNotification();
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

            if (AppConfigService.Counter.Count > 0)
            {
                AppConfigService.Counter.RemoveAt(0);
            }

            if (AppConfigService.Counter.Count == 0)
            {
                AppConfigService.SafeToRestart = true;
            }
        }

        return jobMessage;
    }

    private ExecuteResultModel SendNotification()
    {
        if (!dataService.IsDataAvailable())
        {
            return new ExecuteResultModel { Success = false, Message = "DataIsNotReady, TaskID = 38" };
        }

        var data = dataService.QueryDailyData();
        var excelPath = excelService.Export(data);
        var mail = mailService.CreateMailDaily(excelPath);

        return new ExecuteResultModel
        {
            Mail = mail,
            Message = string.Empty,
            Success = true
        };
    }
}