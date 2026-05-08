namespace Doorlist.Application.User.DTOs;

using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class FullUserRegistrationDto
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
    
    // Nullable for now
    public IFormFile? ProfilePicture { get; set; }
}