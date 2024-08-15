using Fanex.BetradarVirtualSports.Business.Models;
using Fanex.BetradarVirtualSports.Business.Services;
using Fanex.MailDelivery.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.BetradarVirtualSports.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class ExecuteMonthlyVS5Controller : ControllerBase
{
    private readonly MainMonthlyService mainService;

    public ExecuteMonthlyVS5Controller(MainMonthlyService mainService)
    {
        this.mainService = mainService;
    }

    [HttpGet]
    public JobMessage Get()
    {
        return mainService.Execute(Product.VS5);
    }
}