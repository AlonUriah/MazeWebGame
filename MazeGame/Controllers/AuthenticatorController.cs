using System;
using System.Web.Http;
using MazeGame.Models;
using System.Linq;
using System.Text;

namespace MazeGame.Controllers
{
    public class AuthenticatorController : ApiController
    {
        private readonly MazeAppContext _db = new MazeAppContext();

        [Route("api/Validate")]
        [HttpGet]
        public IHttpActionResult Validate(string sessionToken)
        {
            var user = _db.Users.FirstOrDefault(u => u.SessionToken != null && u.SessionToken.Equals(sessionToken, StringComparison.OrdinalIgnoreCase));

            if (user != null) return Ok();

            var uriBuilder = new StringBuilder(Request.RequestUri.GetLeftPart(UriPartial.Authority));
            uriBuilder.Append("/login");
            var uri = new Uri(uriBuilder.ToString());
            return Redirect(uri);
        }
    }
}
