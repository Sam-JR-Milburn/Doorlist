namespace Doorlist.Domain.Entities;

/// <summary>
/// Each role translates to some action verbs on a given organisation
/// </summary>
public enum OrganisationRole
{
    Member,
    Admin,
    Owner
}

/// <summary>
/// Records user membership of an organisation, and their role within it.
/// </summary>
public class OrganisationMember
{
    public Guid UserId { get; private set; }
    public Guid OrganisationId { get; private set; }
    
    public OrganisationRole Role { get; private set; }
    
    public OrganisationMember() {}
    public OrganisationMember(Guid userId, Guid organisationId)
    {
        UserId = userId;
        OrganisationId = organisationId;
        Role = OrganisationRole.Member;
    }

    public bool ChangeOrganisationRole(OrganisationRole role)
    {
        Role = role;
        return true;
    }
}