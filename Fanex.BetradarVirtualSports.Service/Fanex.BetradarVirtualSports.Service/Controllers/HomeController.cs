using Fanex.BetradarVirtualSports.Business.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace Fanex.BetradarVirtualSports.Service.Controllers;

[ApiController]
[Route("")]
public class HomeController : ControllerBase
{
    [HttpGet]
    [Route("")]
    public IActionResult Index()
    {
        if (AppConfigService.SafeToRestart)
        {
            return Problem(
                detail: "Time to restart",
                instance: Environment.MachineName,
                statusCode: (int)HttpStatusCode.ServiceUnavailable,
                title: "Time to restart",
                type: "Time to restart"
            );
        }

        return Ok($"{Environment.MachineName}");
    }
}