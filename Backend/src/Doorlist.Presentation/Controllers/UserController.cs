namespace Doorlist.Presentation.Controllers;

using System.Security.Claims;
using Application.Services.User;
using Application.Services.User.DTO.Requests;
using Application.Services.User.DTO.Responses;
using Domain.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Users: configure user profiles and metadata.
/// Keycloak handles AuthN.  
/// </summary>
[ApiController]
[Route("/api/[controller]")]
public class UserController :  ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    /// <summary>
    /// Fully register a user with Doorlist.
    /// At present, that's with the Keycloak authentication service.
    /// </summary>
    [AllowAnonymous]
    [HttpPost]
    [Route("registerUserInternal")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status499ClientClosedRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> RegisterUserInternal(
        [FromForm] FullUserRegistrationDto registrationData, 
        CancellationToken requestAborted)
    {
        using var timeoutSource = new CancellationTokenSource(TimeSpan.FromSeconds(4));
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(requestAborted, timeoutSource.Token);

        try
        {
            Result<UserRegistrationResponseDto> result =
                await _userService.RegisterLocalAsync(registrationData, linkedSource.Token);
            if (result.IsSuccess)
            {
                UserRegistrationResponseDto? resultData = result.Value;
                return CreatedAtRoute(nameof(GetUserById), new { id = resultData?.UserId }, resultData); // HTTP 201: Here's the created user
            }

            switch (result.ErrorType)
            {
                case ErrorType.Validation:
                    return BadRequest(result.ErrorMessage); // HTTP 400: Date parsing issues
                case ErrorType.Conflict:
                    return Conflict(result.ErrorMessage); // HTTP 409: If the email already exists
                case ErrorType.DependencyFailure:
                    return
                        StatusCode(503,
                            result.ErrorMessage); // HTTP 503: Service Unavailable, if either Keycloak or the DB repos are down.
                default:
                    return BadRequest(result.ErrorMessage);
            }
        }
        // Catch timeouts
        catch (OperationCanceledException) when (timeoutSource.IsCancellationRequested)
        {
            _logger.LogError("Registration request timed out at the controller threshold.");
            return StatusCode(503, "Identity service took too long to respond.");
        }
        // Catch user disconnection
        catch (OperationCanceledException)
        {
            _logger.LogError("Registration request timed due to client disconnection.");
            return StatusCode(499, "Client closed request");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }
    
    /// <summary>
    /// Register a user against external auth.
    /// The feature is on pause for now, pre domain-name.
    /// </summary>
    [AllowAnonymous]
    [HttpPost]
    [Route("registerUserExternal")]
    public async Task<IActionResult> RegisterUserExternal(/* DTOs here later */)
    {
        return StatusCode(500, "Not Implemented");
    }
    
    /// <summary>
    /// Get User details about a GUID.
    /// </summary>
    /// <remarks>
    /// This will be the among the first real areas touching authorisation
    /// </remarks>
    [Authorize]
    [HttpGet("{id:guid}", Name = nameof(GetUserById))]
    // Needs auth controller - site read_users and also the exact user themselves
    public async Task<IActionResult> GetUserById(Guid id)
    {
        throw new NotImplementedException();
    }
    
}