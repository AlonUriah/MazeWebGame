using System;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using MazeGame.Models;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace MazeGame.Controllers
{
    public class UsersController : ApiController
    {
        private MazeAppContext _db = new MazeAppContext();

        /// <summary>
        /// Validates wether a given username is taken and already in use
        /// or not.
        /// </summary>
        /// <param name="username">An optional username to validate</param>
        /// <returns>True if this username is valid (not exist in database), False otherwise</returns>
        [Route("api/Users/ValidateUsername")]
        [HttpGet]
        public IHttpActionResult ValidateUsername(string username)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username.Equals(username));
            
            // Return false if this username is invalid (in use), true otherwise
            return Ok(user != null ? false : true);
        }

        /// <summary>
        /// Create a new User based on Request parameters.
        /// Adds it to database and encryptis its password (SHA1)
        /// </summary>
        /// <param name="registerForm">User parameters</param>
        /// <returns>Ok if managed to add to db, False otherwise</returns>
        [Route("api/Users/Register")]
        [HttpPost]
        public IHttpActionResult Register(JObject registerForm)
        {
            string username, password, email;

            // Try get User parameters from Request data
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

            // Encryping User's password
            byte[] passByte = Encoding.UTF8.GetBytes(password);
            var shaProvider = new SHA1CryptoServiceProvider();
            byte[] encrypted = shaProvider.ComputeHash(passByte);

            var user = new User() { Username = username, Password = Encoding.UTF8.GetString(encrypted), Email = email };

            // Check if username is not already in use
            if(_db.Users.FirstOrDefault(u => u.Username.Equals(username)) != null)
            {
                return BadRequest("Username is already taken");
            }

            // Try add User to database
            try
            {
                _db.Users.Add(user);
                _db.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest($"Could not insert user. Exception caught: {e.Message}");
            }
        }

        /// <summary>
        /// Generates a specific User,
        /// once authenticated by username and password, a unique
        /// sessionToken.
        /// </summary>
        /// <param name="loginForm">A username and a password</param>
        /// <returns>A SessionToken generated to this User, if username and password are correct.</returns>
        [Route("api/Users/Login")]
        [HttpPost]
        public IHttpActionResult Login(JObject loginForm)
        {
            string username, password;

            // Try get mandatory fields out of request parameters.
            try
            {
                username = loginForm["username"].Value<string>();
                password = loginForm["password"].Value<string>();
            }
            catch (Exception)
            {
                return BadRequest("Bad request structure. Missing username/password.");
            }

            // Encrypt password since this is how it was saved in database
            byte[] passByte = Encoding.UTF8.GetBytes(password);
            var shaProvider = new SHA1CryptoServiceProvider();
            byte[] decrypted = shaProvider.ComputeHash(passByte);
            string decryptedStr = Encoding.UTF8.GetString(decrypted);

            // Try to locate User
            var user = _db.Users.FirstOrDefault(u => u.Username.Equals(username) && u.Password.Equals(decryptedStr));
            if (user == null)
            {
                return BadRequest("Username or Password are incorrect.");
            }

            // User was found, give him a unique SessionToken
            user.SessionToken = Guid.NewGuid().ToString();

            // Update SessionToken in database
            try
            {
                _db.Entry<User>(user).Property(p => p.SessionToken).IsModified = true;
                _db.SaveChanges();
                return Ok(user.SessionToken);
            }
            catch (Exception)
            {
                return BadRequest("Could not log in, please try again later");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionToken"></param>
        /// <returns></returns>
        [Route("api/Users/Logout")]
        [HttpGet]
        public IHttpActionResult Logout(string sessionToken)
        {
            // Get User by its SessionToken
            var user = _db.Users.FirstOrDefault(u => u.SessionToken.Equals(sessionToken));

            // User was not found or its sessionToken was already expired
            if (user == null)
            {
                return BadRequest("Your session is already been expired.");
            }

            // Detach its current Session Token
            user.SessionToken = null;

            // Update database
            try
            {
                _db.Entry<User>(user).Property(p => p.SessionToken).IsModified = true;
                _db.SaveChanges();
                return Ok("You are now logged out");
            }
            catch (Exception)
            {
                return BadRequest("Could not log out, please try again later.");
            }
        }

        /// <summary>
        /// Get top {recordsNum} Users by Ranking.
        /// </summary>
        /// <param name="recordsNum">A number of records to pull</param>
        /// <returns>A list of {recordsNum} Users leading Ranking score</returns>
        [Route("api/Users/Records/{recordsNum}")]
        [HttpGet]
        public IEnumerable<User> GetRecords(int recordsNum)
        {
            // Get Users sorted by descending Rank
            var users = _db.Users.OrderByDescending(u => u.Rate).ToArray();
            var topRecords = new List<User>();

            // Add {recordsNum} Users to response list
            for (int i = 0; i < recordsNum && i < users.Length; i++)
            {
                topRecords.Add(users[i]);
            }

            return topRecords;
        }
    }
}
