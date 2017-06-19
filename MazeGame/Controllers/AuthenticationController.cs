using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace MazeGame.Controllers
{
    public class AuthenticationController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post(HttpRequest request)
        {
            //read parameters - session
            //if exists redirect
            //else to login page

            var response = Request.CreateResponse(HttpStatusCode.Moved);
            response.Headers.Location = new Uri("/api/login");
            return Ok();
            //return response;
        }

        [HttpGet]
        public IHttpActionResult Get(HttpRequest request)
        {
            return Ok();
        }
    }
}
