using EBZShared.Models;

namespace EBZClient.Scripts
{
    public interface IAuth
    {
        Action<User?> OnLoggedStateChange { get; set; }

        Task<bool> Login(User user);
        void Logout();
    }
}