using System;
using System.Linq;
using System.Web;
using System.Web.Http;

using MazeGame.Models;

namespace MazeGame.Controllers
{
    [RoutePrefix("api/Multiplayer")]
    public class AuthenticationController : ApiController
    {
        private readonly MazeAppContext _db = new MazeAppContext();

        [Route("")]
        public IHttpActionResult Validate(HttpRequest request)
        {
            var sessionToken = request.Params["sessionToken"];

            if (string.IsNullOrWhiteSpace(sessionToken))
            {
                return Redirect(new Uri("/api/Register"));
            }

            var user = _db.Users.FirstOrDefault(u => u.SessionToken.Equals(sessionToken));

            if(user == null)
            {
                return Redirect(new Uri("/api/Register"));
            }

            var validatedUri = request.Url.AbsolutePath.Replace("Validate/", string.Empty);
            return Redirect(new Uri(validatedUri));
        }
    }
}
