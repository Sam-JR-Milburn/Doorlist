namespace Doorlist.Application.Services.User.DTO.Requests;

using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public record FullUserRegistrationDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = String.Empty;
    
    [Required]
    [MinLength(10)]
    public string Password { get; set; } = String.Empty;
    
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = String.Empty;
    
    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = String.Empty;
    
    [Required]
    [DataType(DataType.Date)]
    [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "Date must be in ISO 8601 format. (YYYY-MM-DD")]
    public string DateOfBirth { get; set; } = String.Empty;
    
    public IFormFile? ProfilePicture { get; set; } // If this changes, it will change via frontend restrictions
}