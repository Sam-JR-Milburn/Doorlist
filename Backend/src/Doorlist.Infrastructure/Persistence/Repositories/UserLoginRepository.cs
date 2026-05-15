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
    /// Save UserLogin to the DB.
    /// </summary>
    public async Task CreateUserLoginAsync(UserLogin userLogin)
    {
        await _doorlistDbContext.UserLogins.AddAsync(userLogin);
    }

    /// <summary>
    /// 
    /// </summary>
    public async Task RemoveUserLoginAsync(UserLogin userLogin)
    {
        _doorlistDbContext.UserLogins.Remove(userLogin);
    }
}