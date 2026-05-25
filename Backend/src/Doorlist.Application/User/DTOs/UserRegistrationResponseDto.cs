namespace Doorlist.Application.User.DTOs;

public record UserRegistrationResponseDto
{
    public Guid UserId { get; set; } = Guid.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}