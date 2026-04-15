using System.Runtime.InteropServices.JavaScript;

namespace Doorlist.Presentation.Controllers;

using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
public class DummyController : Controller
{
    
    [HttpGet]
    [Route("/[controller]/[action]")]
    public IActionResult GetTime()
    {
        DateTime now = DateTime.Now;
        return Ok(now.ToString("HH:mm:ss"));
    }
}