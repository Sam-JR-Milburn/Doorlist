namespace Doorlist.Infrastructure.Persistence.Repositories;

using Domain.Entities;
using Domain.Interfaces.User;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Store non-auth data about users.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly DoorlistDbContext _doorlistDbContext;

    public UserRepository(DoorlistDbContext doorlistDbContext)
    {
        _doorlistDbContext = doorlistDbContext;
    }

    public async Task AddUserAsync(User user)
    {
        await _doorlistDbContext.Users.AddAsync(user);
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _doorlistDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        User? user = await _doorlistDbContext.Users.FindAsync(userId);
        if (user != null)
        {
            _doorlistDbContext.Users.Remove(user);
        }
    }

    public async Task UpdateUserAsync(User updatedUser)
    {
        _doorlistDbContext.Users.Update(updatedUser);
    }
}