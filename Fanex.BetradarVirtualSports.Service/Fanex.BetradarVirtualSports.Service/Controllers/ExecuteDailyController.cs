using Fanex.BetradarVirtualSports.Business.Services;
using Fanex.MailDelivery.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.BetradarVirtualSports.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class ExecuteDailyController(MainDailyService mainService) : ControllerBase
{
    [HttpGet]
    public JobMessage Get()
    {
        return mainService.Execute();
    }
}