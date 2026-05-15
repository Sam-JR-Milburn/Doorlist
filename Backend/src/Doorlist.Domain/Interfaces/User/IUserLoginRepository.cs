namespace Doorlist.Domain.Interfaces.User;

using Entities;

public interface IUserLoginRepository
{
    public Task CreateUserLoginAsync(UserLogin userLogin);
    public Task RemoveUserLoginAsync(UserLogin userLogin);
}