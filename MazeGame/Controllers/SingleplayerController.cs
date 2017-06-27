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

            var maze = MazeHandler.GenerateMaze(rows, cols);
            return Ok(maze);
        }

        //append Post
        [Route("api/Singleplayer/Solve")]
        [HttpPost]
        public IHttpActionResult Solve(JObject game)
        {
            JObject jasonGame = (JObject)game["game"];
            var algorithm = game["algorithm"].Value<int>();
            var solution = MazeHandler.SolveMaze(jasonGame.ToString(), algorithm == 0 ? "bfs" : "dfs");
            return Ok(solution);
        }
    }
}
