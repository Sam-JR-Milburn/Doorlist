namespace Doorlist.Domain.Entities;

using Exceptions;
using Exceptions.Identity;

/// <summary>
/// The User entity. Connects to a Keycloak user for AuthN.
/// </summary>
public class User
{
    // Unique ID
    public Guid Id { get; private set; }
    
    // External identities linked to the user. N:1.
    private readonly List<UserLogin> _logins = new();
    public IReadOnlyCollection<UserLogin> Logins => _logins;
    
    // Serves as a username
    public string Email { get; private set; }
    public bool EmailVerified { get; private set; } = false;
    
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    
    public DateTime DateOfBirth { get; private set; }
    
    public string? ProfilePicturePath { get; private set; }
    
    public User() {} // EF Core
    
    public User(Guid id, string email, string firstName, string lastName, DateTime dateOfBirth)
    {
        Id = id;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
    }
    
    /// <summary>
    /// Link an external auth provider to this identity. 
    /// </summary>
    /// <exception cref="IdentityAlreadyLinkedException">This throws if the user already has an identity linked against a provider and issuer</exception>
    public bool LinkIdentity(string providerName, string providerKey, string issuer)
    {
        if (string.IsNullOrWhiteSpace(providerName) || string.IsNullOrWhiteSpace(providerKey)) return false; 
        
        if (!_logins.Any(x => x.ProviderName == providerName && x.Issuer ==  issuer))
        {
            throw new IdentityAlreadyLinkedException($"Already a matching provider (${providerName}) and issuer (${issuer}) pair for user ${Id}");
        }
        _logins.Add(new UserLogin(providerName, providerKey, issuer));
        return true;
    }

    public bool UpdateFirstName(string newFirstName)
    {
        if (string.IsNullOrWhiteSpace(newFirstName)) return false;
        if (newFirstName == FirstName) return true;
        
        this.FirstName = newFirstName;
        return true;
    }

    public bool UpdateLastName(string newLastName)
    {
        if (string.IsNullOrWhiteSpace(newLastName)) return false;
        if (newLastName == LastName) return true;
        
        this.LastName = newLastName;
        return true;
    }
    
    /// <exception cref="DomainException">This throws if the age is beyond reasonable bounds</exception>
    public bool UpdateDateOfBirth(DateTime newDateOfBirth) 
    {
        var today =  DateTime.Today;
        if (newDateOfBirth > today) return false;
        if (newDateOfBirth == DateOfBirth) return true;
        
        int age =  today.Year - newDateOfBirth.Year;
        if (// If we haven't reached the birth month yet 
            today.Month < DateOfBirth.Month || 
            // If it's the current month and the day is less than that day 
            (today.Month == DateOfBirth.Month && today.Day < DateOfBirth.Day)) { age -= 1; }
        
        // Check for reasonable ages.
        if (age < 5 || age > 125) throw new DomainException("Age cannot be lower than 5 or higher than 125.");
        this.DateOfBirth = newDateOfBirth;
        return true;
    }

    public bool SetEmailVerified()
    {
        EmailVerified = true;
        return true;
    }
    
    public bool SetProfilePicture(string profilePicturePath)
    {
        if (string.IsNullOrWhiteSpace(profilePicturePath)) return false;
        if (profilePicturePath == ProfilePicturePath) return true;
        
        this.ProfilePicturePath = profilePicturePath;
        return true;
    }
}