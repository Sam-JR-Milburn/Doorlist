namespace Doorlist.Infrastructure.Persistence.Repositories;

using Domain.Entities;
using Domain.Interfaces.User;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Link authentication details against a user ID.
/// </summary>
public class UserLoginRepository : IUserLoginRepository
{
    private DoorlistDbContext _doorlistDbContext;

    public UserLoginRepository(DoorlistDbContext doorlistDbContext)
    {
        _doorlistDbContext = doorlistDbContext;
    }
    
    /// <summary>
    /// Check all emails (ProviderKey/'subject'), check if it exists
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<bool> CheckUserExistsByEmailAsync(string email)
    {
        return await _doorlistDbContext.UserLogins.AnyAsync(x => x.ProviderKey == email);
    }
    
}