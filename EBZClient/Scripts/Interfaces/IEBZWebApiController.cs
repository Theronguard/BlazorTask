using EBZShared.Models;
using System.Net;

namespace EBZClient.Scripts
{
    public interface IEBZWebApiController
    {
        Task<HttpStatusCode> AddUser(User user);
        Task<IEnumerable<User>?> GetActiveUsers();
        Task<IEnumerable<User>?> GetAllUsers();
        Task<string> Login(User user);
    }
}