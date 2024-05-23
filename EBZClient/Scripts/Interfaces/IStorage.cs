using EBZShared.Models;

namespace EBZClient.Scripts
{
    /// <summary>
    /// Abstracting away storage to potentially switch it later
    /// </summary>
    public interface IStorage
    {
        public void SetWebToken(string token);
        public string GetWebToken();

        public void SetLoggedInUser(User? user);
        public User? GetLoggedInUser();
    }
}
