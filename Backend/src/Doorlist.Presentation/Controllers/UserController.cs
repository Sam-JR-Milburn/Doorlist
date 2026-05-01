namespace Doorlist.Presentation.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Doorlist.Application.Common.DTOs.User;

/// <summary>
/// Users: configure user profiles and metadata.
/// Keycloak handles AuthN and AuthZ. 
/// </summary>
[ApiController]
[Route("[controller]")]
public class UserController :  ControllerBase
{
    private readonly ILogger<UserController> _logger;
    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("RegisterUser")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromForm] UserRegistrationDTO registrationData)
    {
        // Check for ISO 8601 YYYY-MM-DD
        if (!DateTime.TryParse(registrationData.DateOfBirth, out var dateOfBirth))
        {
            return BadRequest("Invalid date format.");
        }
        
        // Check if the user exists by their email.
        
        // ----
        
        return Ok();
    }
}