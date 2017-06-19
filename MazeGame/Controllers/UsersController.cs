using System;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using MazeGame.Models;

namespace MazeGame.Controllers
{
    public class UsersController : ApiController
    {
        private readonly DateTime _time; 
        private readonly object _locker = new object();
        private MazeAppContext _db = new MazeAppContext();

        [Route("api/Users/Register")]
        [HttpPost]
        public IHttpActionResult Register(JObject registerForm)
        {
            string username = registerForm["username"].Value<string>();
            string password = registerForm["password"].Value<string>();
            string email = registerForm["email"].Value<string>();

            var user = new User() { Username = username, Password = password, Email = email };
            try
            {
                _db.Users.Add(user);
                _db.SaveChanges();
                return Ok();
            }
            catch(Exception)
            {
                return BadRequest("Please select another username");
            }
        }
        
        [Route("api/Users/Login")]
        [HttpPost]
        public IHttpActionResult Login(JObject loginForm)
        {
            var username = loginForm["username"].Value<string>();
            var password = loginForm["password"].Value<string>();

            var user = _db.Users.FirstOrDefault(u => u.Username.Equals(username) && u.Password.Equals(password));
            if(user == null)
            {
                return BadRequest("Username or Password are incorrect.");
            }

            user.SessionToken = GenerateToken();
            try
            {
                _db.SaveChanges();
                return Ok(user.SessionToken);
            }
            catch (Exception)
            {
                return BadRequest("Could not log in, please try again later");
            }
        }

        [Route("api/Users/Logout")]
        [HttpGet]
        public IHttpActionResult Logout(JObject sessionToken)
        {
            var user = _db.Users.FirstOrDefault(u => u.SessionToken.Equals(sessionToken));

            if (user == null)
            {
                return BadRequest("Your session is already been expired.");
            }

            user.SessionToken = null;
            try
            {
                _db.SaveChanges();
                return Ok("You are now logged out");
            }
            catch (Exception)
            {
                return BadRequest("Could not log out, please try again later.");
            }
        }

        private string GenerateToken()
        {
            lock (_locker)
            {
                return $"{DateTime.Now:yyyyMMddHHmmsszzz}";
            }
        }
    }
}
