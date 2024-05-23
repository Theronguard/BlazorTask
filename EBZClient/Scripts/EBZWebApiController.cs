using EBZShared.Models;
using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;

namespace EBZClient.Scripts
{
    /// <summary>
    /// Communicates with the API
    /// </summary>
    public class EBZWebApiController : IEBZWebApiController
    {
        private readonly HttpClient _httpClient;

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="storage"></param>
        public EBZWebApiController(HttpClient httpClient, IStorage storage)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7147/"); //should be in some settings class - add this later
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", storage.GetWebToken());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<User>?> GetAllUsers()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/users");

            if (!response.IsSuccessStatusCode)
                return new List<User>();

            List<User>? users = await response.Content.ReadFromJsonAsync(typeof(List<User>)) as List<User>;
            return users;
        }

        /// <summary>
        /// Gets all the active users - by active
        /// I mean currently logged in, with JWT Tokens which didn't yet expire
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<User>?> GetActiveUsers()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/users/active");

            if (!response.IsSuccessStatusCode)
                return new List<User>();

            List<User>? users = await response.Content.ReadFromJsonAsync(typeof(List<User>)) as List<User>;
            return users;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<HttpStatusCode> AddUser(User user)
        {
            JsonContent jsonContent = JsonContent.Create(user);
            HttpResponseMessage response = await _httpClient.PostAsync("/users", jsonContent);
            return response.StatusCode;
        }

        /// <summary>
        /// Retrieves a web token from the server
        /// </summary>
        /// <param name="user"></param>
        /// <returns>JWT Token</returns>
        public async Task<string> Login(User user)
        {
            StringContent content = new StringContent(user.Username, Encoding.ASCII);
            HttpResponseMessage response = await _httpClient.PostAsync($"/users/login", content);

            if (response.StatusCode != HttpStatusCode.OK)
                return string.Empty;

            string token = await response.Content.ReadAsStringAsync();
            return token;
        }

        #endregion
    }
}


