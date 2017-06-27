using System;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using MazeGame.Models;
using System.Text;
using System.Security.Cryptography;

namespace MazeGame.Controllers
{
    public class UsersController : ApiController
    {
        private readonly object _locker = new object();
        private MazeAppContext _db = new MazeAppContext();

        [Route("api/Users/Register")]
        [HttpPost]
        public IHttpActionResult Register(JObject registerForm)
        {
            string username;
            string password;
            string email;

            try
            {
                username = registerForm["username"].Value<string>();
                password = registerForm["password"].Value<string>();
                email = registerForm["email"].Value<string>();
            }
            catch (Exception)
            {
                return BadRequest("Bad request structure. Missing username/password/email.");
            }

            byte[] passByte = Encoding.UTF8.GetBytes(password);
            var shaProvider = new SHA1CryptoServiceProvider();
            byte[] encrypted = shaProvider.ComputeHash(passByte);

            var user = new User() { Username = username, Password = Encoding.UTF8.GetString(encrypted), Email = email };
            bool isUsernameTaken = _db.Users.FirstOrDefault(u => u.Username.Equals(username)) != null;
            
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
            string username;
            string password;

            try
            {
                username = loginForm["username"].Value<string>();
                password = loginForm["password"].Value<string>();
            }
            catch (Exception)
            {
                return BadRequest("Bad request structure. Missing username/password.");
            }

            byte[] passByte = Encoding.UTF8.GetBytes(password);
            var shaProvider = new SHA1CryptoServiceProvider();
            byte[] decrypted = shaProvider.ComputeHash(passByte);
            string decryptedStr = Encoding.UTF8.GetString(decrypted);

            var user = _db.Users.FirstOrDefault(u => u.Username.Equals(username) && u.Password.Equals(decryptedStr));
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
        public IHttpActionResult Logout(string sessionToken)
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
                return $"{Guid.NewGuid()}";
            }
        }
    }
}
