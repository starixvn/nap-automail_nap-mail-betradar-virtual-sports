using Fanex.BetradarVirtualSports.Business.Models;
using Fanex.BetradarVirtualSports.Business.Services;
using Fanex.MailDelivery.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.BetradarVirtualSports.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class ExecuteMonthlyVS7Controller(MainMonthlyService mainService) : ControllerBase
{
    [HttpGet]
    public JobMessage Get()
    {
        return mainService.Execute(Product.VS7);
    }
}