namespace Doorlist.Application;

public interface IUnitOfWork
{
    // Returns the number of changes persisted.
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}