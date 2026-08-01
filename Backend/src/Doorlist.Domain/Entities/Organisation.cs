namespace Doorlist.Domain.Entities;

/// <summary>
/// Organisations represent a business unit booking or operating music venues, etc etc.
/// </summary>
public class Organisation
{
    public Guid Id { get; private set; }
    
    public string OrganisationName { get; private set; }

    public Organisation() {}

    public Organisation(Guid id, string tenantName)
    {
        this.Id = id;
        this.OrganisationName = tenantName;
    }
}
