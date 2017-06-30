using System.Web.Http;
using Newtonsoft.Json.Linq;
using System.Linq;
using MazeProjectLibrary;
using MazeGame.Models;
using System;

namespace MazeGame.Controllers
{
    public class SingleplayerController : ApiController
    {
        private MazeAppContext _db = new MazeAppContext();

        /// <summary>
        /// Create a Maze.
        /// In singleplayer mode all user has to get
        /// is a Maze.
        /// </summary>
        /// <param name="gameForm">Game request parameters - expected name, rows and cols.</param>
        /// <returns>Ok with a Maze object or a BadRequest in case one/more mandatory fields were missing</returns>
        [Route("api/Singleplayer/CreateGame")]
        [HttpPost]
        public IHttpActionResult CreateGame(JObject gameForm)
        {
            string name, sessionToken;
            int rows, cols;

            // Try get mandatory fields from Request
            try
            {
                name = gameForm["name"].Value<string>();
                rows = gameForm["rows"].Value<int>();
                cols = gameForm["cols"].Value<int>();
                sessionToken = gameForm["sessionToken"].Value<string>();
            }
            catch (Exception)
            {
                return BadRequest("Could not find one or more mandatory fields - name,rows and cols");
            }

            var user = _db.Users.FirstOrDefault(u => u.SessionToken.Equals(sessionToken));
            if(user == null)
            {
                return BadRequest("User must be logged in in order to play");
            }

            // Generate maze by using MazeHandler
            var maze = MazeHandler.GenerateMaze(rows, cols);
            return Ok(maze);
        }

        /// <summary>
        /// Return solution to a given maze.
        /// </summary>
        /// <param name="game">Composed by Maze object ('game') and an algorithm to solve</param>
        /// <returns>Relative solution to a given maze</returns>
        [Route("api/Singleplayer/Solve")]
        [HttpPost]
        public IHttpActionResult Solve(JObject game)
        {
            string sessionToken;
            JObject jasonGame;
            int algorithm;

            // Get game from request parameters
            try
            {
                sessionToken = game["sessionToken"].Value<string>();
                jasonGame = (JObject)game["game"];
                algorithm = game["algorithm"].Value<int>();
            }
            catch
            {
                return BadRequest("One/more mandatory fields are missing");
            }

            var user = _db.Users.FirstOrDefault(u => u.SessionToken.Equals(sessionToken));
            if (user == null)
            {
                return BadRequest("User must be logged in in order to play");
            }

            // Get solution by calling MazeHandler.SolveMaze
            var solution = MazeHandler.SolveMaze(jasonGame.ToString(), algorithm == 0 ? "bfs" : "dfs");
            return Ok(solution);
        }
    }
}
