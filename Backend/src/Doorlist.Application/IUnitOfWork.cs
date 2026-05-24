namespace Doorlist.Application;

using Microsoft.EntityFrameworkCore.Storage;

public interface IUnitOfWork
{
    // Returns the number of changes persisted.
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}