using EBZShared.Models;

namespace EBZWebApi.Services
{
    public interface IActiveUsersService
    {
        void AddActiveUser(string username);
        IEnumerable<User> GetActiveUsers();
        void RemoveActiveUser(string username);
    }
}