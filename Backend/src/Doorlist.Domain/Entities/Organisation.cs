namespace Doorlist.Domain.Entities;


/// <summary>
/// Organisation represents membership of a business, venue, etc.
/// It's a stub for now.
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
