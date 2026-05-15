namespace Doorlist.Infrastructure.Persistence;

using Application;

/// <summary>
/// Save DB writes in the Application layer.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly DoorlistDbContext _doorlistDbContext;
    public UnitOfWork(DoorlistDbContext doorlistDbContext)
    {
        _doorlistDbContext = doorlistDbContext;
    }
    
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _doorlistDbContext.SaveChangesAsync(cancellationToken);
    }
}