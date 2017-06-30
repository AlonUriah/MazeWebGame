using System;
using System.Web.Http;
using MazeGame.Models;
using System.Linq;
using System.Text;

namespace MazeGame.Controllers
{
    public class AuthenticatorController : ApiController
    {
        private MazeAppContext _db = new MazeAppContext();

        /// <summary>
        /// Validate if sessionToken exists in Users table.
        /// If so - it means user is connected and session is alive.
        /// Else, it means user must log in again.
        /// </summary>
        /// <param name="sessionToken">User sessionToken to be validated</param>
        /// <returns>True if session is alive, False otherwise</returns>
        [Route("api/Validate/{sessionToken}")]
        [HttpGet]
        public IHttpActionResult Validate(string sessionToken)
        {
            // Update app context in case changes were made
            _db = new MazeAppContext();

            // Try find user with this sessionToken
            var user = _db.Users.FirstOrDefault(u => u.SessionToken != null && u.SessionToken.Equals(sessionToken, StringComparison.OrdinalIgnoreCase));

            // If found one it means session is alive
            if (user != null) return Ok();

            // Else return redirect response to log in page since session is expired
            var uriBuilder = new StringBuilder(Request.RequestUri.GetLeftPart(UriPartial.Authority));
            uriBuilder.Append("/login");
            var uri = new Uri(uriBuilder.ToString());
            return Redirect(uri);
        }
    }
}
