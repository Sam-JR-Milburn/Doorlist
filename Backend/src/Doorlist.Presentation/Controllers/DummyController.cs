using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Authorization;

namespace Doorlist.Presentation.Controllers;

using Microsoft.AspNetCore.Mvc;

[Route("[controller]")]
public class DummyController : Controller
{
    
    [HttpGet]
    [AllowAnonymous]
    [Route("/[controller]/[action]")]
    public IActionResult GetTime()
    {
        DateTime now = DateTime.Now;
        return Ok(now.ToString("HH:mm:ss") + "\n");
    }
    
    [HttpGet("public")]
    public IActionResult Public() => Ok();
    
    [HttpGet("protected")]
    [Authorize(Policy = "UserPolicy")]
    public IActionResult Protected() => Ok();
    
    [HttpGet("also-protected")]
    [Authorize(Policy = "AdminPolicy")]
    public IActionResult AlsoProtected() => Ok();
}