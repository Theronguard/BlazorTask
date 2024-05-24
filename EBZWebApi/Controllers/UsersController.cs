using EBZWebApi.DataAccess;
using EBZShared.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using EBZWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authentication;

namespace EBZWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IJWTAuthService _jwtAuthService;
        private readonly IActiveUsersService _activeUsersService;

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jwtAuthservice"></param>
        /// <param name="activeUsersService"></param>
        public UsersController(IJWTAuthService jwtAuthservice, IActiveUsersService activeUsersService)
        {
            _jwtAuthService = jwtAuthservice;
            _activeUsersService = activeUsersService;
        }

        /// <summary>
        /// Retrieves all users from the DB
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            using UserDA userDA = new();
            IEnumerable<User> users = userDA.Users.ToList(); //prevents a disposal error
            
            if (users.Count() == 0)
                return NotFound("No users found");

            return Ok(users);
        }

        /// <summary>
        /// Not sure whether adding more endpoints was allowed by this task - if not
        /// then I could probably include parameters in the request body.
        /// </summary>
        /// <returns></returns>
        [HttpGet("active")]
        [Authorize]
        public ActionResult<IEnumerable<User>> GetAllActiveUsers()
        {
            List<User> users = _activeUsersService.GetActiveUsers().ToList();

            if (users.Count() == 0)
                return NotFound("No active users found!");

            return Ok(users);
        }

        /// <summary>
        /// Retrieves the user by his ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetUserByID(int id)
        {
            using UserDA userDA = new();
            User? user = userDA.Users.Where(x => x.Id == id).FirstOrDefault();

            if (user is null)
                return NotFound("User not found");

            return Ok(user);
        }

        /// <summary>
        /// Inserts a user into the database
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult CreateUser([FromBody] User user)
        {
            using UserDA userDA = new UserDA();

            if (userDA.Users.Where(x => x.Username == user.Username).Count() > 0)
                return BadRequest("This username is already taken!");

            DateTime now = DateTime.Now;
            user.Created = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
            user.LastActive = user.Created;

            userDA.Add(user);
            userDA.SaveChanges();

            return Ok("Username created successfully!");
        }

        /// <summary>
        /// Updates an already existing user.
        /// This doesnt create a new user when a user isnt found.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateUser(int id, [FromBody] User user)
        {
            using UserDA userDA = new UserDA();

            if (userDA.Users.Where(x => x.Id == id).Count() == 0)
                return NotFound("Update failed. User not found!");

            user.Id = id;

            userDA.Update(user);
            userDA.SaveChanges();

            return Ok("Username updated sucesfully");
        }

        /// <summary>
        /// Deletes a user by the id from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            using UserDA userDA = new UserDA();

            User? user = userDA.Users.Where(x => x.Id == id).FirstOrDefault();

            if (user is not null)
                userDA.Remove(user);
            else
                return NotFound($"User with id {id} doesn't exist!");

            userDA.SaveChanges();
            return Ok("Deleted a user!");
        }

        /// <summary>
        /// Changes the city of a user specified by his id.
        /// Also ensures the city name is correct
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("change-city/{id}")]
        public async Task<IActionResult> ChangeUserCity(int id)
        {
            using StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8);
            string city = await reader.ReadToEndAsync();

            if (!ValidateProperty<User>(city, nameof(EBZShared.Models.User.City), out List<ValidationResult> results))
            {
                StringBuilder validationMessage = new("City name is invalid:\n");

                foreach (ValidationResult result in results)
                    validationMessage.AppendLine($"-{result.ErrorMessage}");

                return BadRequest(validationMessage.ToString());
            }

            using UserDA userDA = new UserDA();
            User? user = userDA.Users.Where(x => x.Id == id).FirstOrDefault();

            if (user is null)
                return StatusCode(404, "User not found!");

            user.City = city;
            userDA.SaveChanges();

            return StatusCode(201, "Users city information changed succesfully!");
        }

        /// <summary>
        /// Logs in a user if it exists in the DB.
        /// Also updates the "LastActive" time of a user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login()
        {
            using StreamReader reader = new StreamReader(Request.Body, Encoding.ASCII);
            string username = await reader.ReadToEndAsync();
           
            using UserDA userDA = new UserDA();
            User user = userDA.Users.Where(x => x.Username == username).FirstOrDefault();

            if (user is null)
                return NotFound("User not found!");

            string token = _jwtAuthService.GenerateToken(user);

            if (token != string.Empty)
                _activeUsersService.AddActiveUser(username);

            DateTime now = DateTime.Now;
            user.LastActive = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
            userDA.SaveChanges();

            return Ok(token);
        }

        /// <summary>
        /// Keeping up with DRY and reusing the existing validation attributes of the user model.
        /// I could probably extract this into some sort of utility class but the project seems too small for this.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="attributeName"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        public bool ValidateProperty<T>(object value, string attributeName, out List<ValidationResult> results)
        {
            PropertyInfo property = typeof(T).GetProperty(attributeName)!;
            IEnumerable<ValidationAttribute> attributes = property.GetCustomAttributes<ValidationAttribute>();
            ValidationContext context = new ValidationContext(value);
            context.MemberName = property.Name;

            results = new();
            return Validator.TryValidateValue(
                value,
                context,
                results,
                attributes
            );
        }

        #endregion
    }
}
