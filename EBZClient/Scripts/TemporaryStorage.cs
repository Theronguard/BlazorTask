using EBZShared.Models;

namespace EBZClient.Scripts
{
    /// <summary>
    /// Non persistent storage. I could probably user interop or Blazored storage,
    /// but this wasn't in the requirements and adding another library seems like an overkill.
    /// </summary>
    public class TemporaryStorage : IStorage
    {
        private string _webToken = string.Empty;
        private User? _LoggedInUser = null;

        /// <summary>
        /// Sets the token
        /// </summary>
        /// <param name="token"></param>
        public void SetWebToken(string token)
        {
            _webToken = token;
        }

        /// <summary>
        /// Retrieves the token
        /// </summary>
        /// <returns></returns>
        public string GetWebToken()
        {
            return _webToken;
        }

        /// <summary>
        /// Sets a logged in user. Pass null to mark the value as not logged in.
        /// </summary>
        /// <param name="user"></param>
        public void SetLoggedInUser(User? user)
        {
            _LoggedInUser = user;
        }

        /// <summary>
        /// Retrieves a logged in user or returns null
        /// if you're logged out.
        /// </summary>
        /// <returns></returns>
        public User? GetLoggedInUser()
        {
            return _LoggedInUser;
        }
    }
}
