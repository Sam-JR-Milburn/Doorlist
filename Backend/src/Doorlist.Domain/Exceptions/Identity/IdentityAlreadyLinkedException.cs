namespace Doorlist.Domain.Exceptions.Identity;

using Doorlist.Domain.Exceptions;

/// <summary>
/// For when a user attempts to sign-up with an existing match for the Provider ('Google', 'Microsoft') and Issuer (eg. the Microsoft tenant) 
/// </summary>
public class IdentityAlreadyLinkedException : DomainException
{
    public IdentityAlreadyLinkedException(string message) : base(message) { }
}