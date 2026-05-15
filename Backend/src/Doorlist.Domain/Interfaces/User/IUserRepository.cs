namespace Doorlist.Domain.Interfaces.User;

using Entities;

public interface IUserRepository
{
    public Task AddUserAsync(User user);
    public Task<User?> GetUserByIdAsync(Guid userId);
    public Task DeleteUserAsync(Guid userId);
    public Task UpdateUserAsync(User user);
}