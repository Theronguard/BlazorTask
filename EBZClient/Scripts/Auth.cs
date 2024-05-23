using EBZShared.Models;

namespace EBZClient.Scripts
{
    public class Auth : IAuth
    {
        private readonly IEBZWebApiController _webApiController;
        private readonly IStorage _storage;

        /// <summary>
        /// Notifies when a user logged in, or logged off.
        /// Returns a user if a login was completed,
        /// returns null if the user has been logged off.
        /// </summary>
        public Action<User?> OnLoggedStateChange { get; set; }


        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webApiController"></param>
        /// <param name="storage"></param>
        public Auth(IEBZWebApiController webApiController, IStorage storage)
        {
            _webApiController = webApiController;
            _storage = storage;
        }

        /// <summary>
        /// Logs in a user if such a user exsits on the server
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> Login(User user)
        {
            string token = await _webApiController.Login(user);
            _storage.SetWebToken(token);

            User? loggedInUser = null;

            if (token != string.Empty && token != null)
                loggedInUser = user;

            _storage.SetLoggedInUser(loggedInUser);

            OnLoggedStateChange?.Invoke(loggedInUser);
            return loggedInUser != null;
        }

        /// <summary>
        /// Logs out the current user.
        /// I could make an endpoint to the server and let it handle active
        /// users, but I wasnt sure if i needed to in the task (also I hate dealing with CORS)
        /// </summary>
        public void Logout()
        {
            _storage.SetLoggedInUser(null);
            _storage.SetWebToken(string.Empty);
            OnLoggedStateChange?.Invoke(null);
        }

        #endregion
    }
}
