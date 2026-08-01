namespace Doorlist.Domain.Entities;

using System.Security.Cryptography;
using Doorlist.Domain.Utility.Extensions;

/// <summary>
/// Handle an organisation invite.
/// </summary>
public class OrganisationInvite
{
    public Guid Id { get; private set; }
    public string InviteToken { get; private set; } = null!;

    public Guid InvitedUserId { get; private set; }
    public Guid OrganisationId { get; private set; }
    public OrganisationRole Role { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; } = false;
    public DateTime? AcceptedAt { get; private set; }

    private static TimeSpan ExpiresIn => new TimeSpan(24*7, 0, 0); // 7 days
    private static readonly int TokenLength = 32;
    private static string ValidTokenCharacters => StringExtensions.GetUppercaseAlphabet() + StringExtensions.GetLowercaseAlphabet() + StringExtensions.GetDigits();
    
    public OrganisationInvite() {}

    public OrganisationInvite(Guid invitedUserId, Guid organisationId, OrganisationRole role)
    {
        Id = Guid.CreateVersion7();
        InvitedUserId = invitedUserId;
        OrganisationId = organisationId;
        Role = role;
        
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = DateTime.UtcNow + ExpiresIn;
        IsRevoked = false;

        InviteToken = GenerateSecureToken();
    }

    /// <summary>
    /// Is this token still valid?
    /// </summary>
    public bool IsValid()
    {
        return !IsRevoked && AcceptedAt == null && DateTime.UtcNow < ExpiresAt;
    }

    public void Revoke()
    {
        IsRevoked = true;
    }

    /// <summary>
    /// Generate a token for accepting the invite.  
    /// </summary>
    private static string GenerateSecureToken()
    {
        byte[] randomBytes = new byte[TokenLength];
        RandomNumberGenerator.Fill(randomBytes);
        
        var tokenChars = new char[TokenLength];
        for (int ind = 0; ind < TokenLength; ind++)
        {
            tokenChars[ind] = ValidTokenCharacters[randomBytes[ind] % ValidTokenCharacters.Length];
        }
        return new string(tokenChars);
    }
    
}