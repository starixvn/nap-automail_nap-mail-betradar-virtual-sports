using Fanex.BetradarVirtualSports.Business.Services;
using Fanex.MailDelivery.Client.Jobs;
using Fanex.MailDelivery.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Fanex.BetradarVirtualSports.Service.Services;

public class ExecuteMonthlyService : JobBase
{
    protected override JobMessage Run(Schedule schedule, IJobExecutionContext jobContext)
    {
        var services = ServiceCollectionExtensions.Services;
        var serviceProvider = services.BuildServiceProvider();

        var mainService = serviceProvider.GetService<MainMonthlyService>();
        var result = mainService.Execute();
        return new JobMessage
        {
            JobStatus = result.JobStatus,
            Schedule = schedule,
            Mail = result.Mail
        };
    }
}