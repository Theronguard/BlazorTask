using EBZShared.Models;
using EBZWebApi.DataAccess;

namespace EBZWebApi.Services
{
    /// <summary>
    /// Handles marking users as active or inactive
    /// either due to a login/logout or token expiration.
    /// </summary>
    public class ActiveUsersService : IActiveUsersService
    {
        private readonly IConfiguration _configuration;

        private Dictionary<string, DateTime> _lastUserTokenUpdate = new Dictionary<string, DateTime>();

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public ActiveUsersService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Adds a user to the dictionary of active users
        /// </summary>
        /// <param name="username"></param>
        public void AddActiveUser(string username)
        {
            if (!_lastUserTokenUpdate.TryAdd(username, DateTime.UtcNow))
                _lastUserTokenUpdate[username] = DateTime.UtcNow;
        }

        /// <summary>
        /// Removes an active user by expiring his tokens
        /// (unless you have an infinite web token, but that's your problem)
        /// </summary>
        /// <param name="username"></param>
        public void RemoveActiveUser(string username)
        {
            if (!_lastUserTokenUpdate.TryAdd(username, DateTime.MinValue))
                _lastUserTokenUpdate[username] = DateTime.MinValue;
        }

        /// <summary>
        /// Retrieves all active users
        /// </summary>
        /// <returns></returns>
        public IEnumerable<User> GetActiveUsers()
        {
            using UserDA userDA = new UserDA();

            foreach (User user in userDA.Users)
            {
                if (!_lastUserTokenUpdate.ContainsKey(user.Username))
                    continue;

                TimeSpan timeSpan = DateTime.UtcNow - _lastUserTokenUpdate[user.Username];
                int tokenExpirationMinutes = _configuration.GetValue<int>("JWTAuth:TokenExpiration");

                if (timeSpan.Minutes <= tokenExpirationMinutes)
                    yield return user;
            }
        }

        #endregion
    }
}
