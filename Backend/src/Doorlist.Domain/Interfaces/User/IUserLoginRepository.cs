namespace Doorlist.Domain.Interfaces.User;

public interface IUserLoginRepository
{
    public Task<bool> CheckUserExistsByEmailAsync(string email);
}