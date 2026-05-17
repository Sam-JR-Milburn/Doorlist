namespace Doorlist.Presentation.Controllers;

using System.Security.Claims;
using Application.User;
using Application.User.DTOs;
using Domain.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Users: configure user profiles and metadata.
/// Keycloak handles AuthN and AuthZ. 
/// </summary>
[ApiController]
[Route("[controller]")]
public class UserController :  ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private  readonly IUserService _userService;
    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    /// <summary>
    ///  Fully register a user with Keycloak.
    /// </summary>
    [AllowAnonymous]
    [HttpPost]
    [Route("register-full")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterLocal([FromForm] FullUserRegistrationDto registrationData)
    {
        Result<UserRegistrationResponseDto> result = await _userService.RegisterLocalAsync(registrationData, CancellationToken.None);
        if (result.IsSuccess)
        {
            UserRegistrationResponseDto? resultData = result.Value;
            return Ok(resultData); 
        }

        switch (result.ErrorType)
        {
            case ErrorType.Validation:
                return BadRequest(result.ErrorMessage); // Date parsing issues
            case ErrorType.Conflict:
                return Conflict(result.ErrorMessage); // If the email already exists
            case ErrorType.DependencyFailure:
                return StatusCode(503, result.ErrorMessage); // Service Unavailable, if either Keycloak or the DB repos are down.
            default:
                return BadRequest(result.ErrorMessage);
        }
    }
    
    
    
    // Below: Partial registration for
    // public async Task<IActionResult> RegisterExternal
}