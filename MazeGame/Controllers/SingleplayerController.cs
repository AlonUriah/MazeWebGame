using System.Web.Http;
using Newtonsoft.Json.Linq;
using MazeProjectLibrary;
using MazeGame.Models;

namespace MazeGame.Controllers
{
    public class SingleplayerController : ApiController
    {
        private MazeAppContext _db = new MazeAppContext();

        [Route("api/Singleplayer/CreateGame")]
        [HttpPost]
        public IHttpActionResult CreateGame(JObject gameForm)
        {
            var name = gameForm["name"].Value<string>();
            var rows = gameForm["rows"].Value<int>();
            var cols = gameForm["cols"].Value<int>();
            var algorithm = gameForm["algorithm"].Value<string>();

            var maze = MazeHandler.GenerateMaze(rows, cols);
            var solution = MazeHandler.SolveMaze(maze,algorithm);

            var mazeJason = JObject.FromObject(maze);
            mazeJason["Solution"] = solution;
            return Ok(mazeJason);
        }
    }
}
