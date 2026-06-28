namespace Doorlist.Domain.Entities;


/// <summary>
/// Tenant represents an organisation that many users can be part of.
/// It's a stub for now.
/// </summary>
public class Tenant
{
    public Guid Id { get; private set; }
    
    public string TenantName { get; private set; }

    public Tenant() {}

    public Tenant(Guid id, string tenantName)
    {
        this.Id = id;
        this.TenantName = tenantName;
    }
}
