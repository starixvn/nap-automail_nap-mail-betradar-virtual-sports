using Fanex.BetradarVirtualSports.Business.Services;
using Fanex.MailDelivery.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.BetradarVirtualSports.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class ExecuteMonthlyController : ControllerBase
{
    private readonly MainMonthlyService mainService;

    public ExecuteMonthlyController(MainMonthlyService mainService)
    {
        this.mainService = mainService;
    }

    [HttpGet]
    public JobMessage Get()
    {
        return mainService.Execute();
    }
}