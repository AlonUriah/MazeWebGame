using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using MazeGame.Models;
using Newtonsoft.Json.Linq;
using MazeProjectLibrary;

namespace MazeGame.Controllers
{
    public class MultiplayerController : ApiController
    {
        private MovesHub _movesHub = new MovesHub();
        private MazeAppContext _db = new MazeAppContext();

        [Route("api/Multiplayer/StartNewGame")]
        [HttpPost]
        public IHttpActionResult StartGame(JObject startGameForm)
        {
            string sessionToken;
            string name;
            int rows;
            int cols;

            try
            {
                sessionToken = startGameForm["sessionToken"].Value<string>();
                name = startGameForm["name"].Value<string>();
                rows = startGameForm["rows"].Value<int>();
                cols = startGameForm["cols"].Value<int>();
            }
            catch (Exception)
            {
                return BadRequest("Could not found one or more parameters");
            }

            var host = _db.Users.FirstOrDefault(u => u.SessionToken.Equals(sessionToken));
            if (host == null)
            {
                return BadRequest("Your session has expired. Please login again.");
            }

            var mazeJson = MazeHandler.GenerateMaze(rows, cols);
            var game = new Game() {
                Name = name,
                Rows = rows,
                Cols = cols,
                Player1Id = host.Id,
                Maze = mazeJson["Maze"].Value<string>() };

            try
            {
                _db.Games.Add(game);
                _db.SaveChanges();
            }
            catch (Exception)
            {
                return BadRequest("Could not create this game, please try again later");
            }

            // Busy wait until opponent join before returning game
            bool didOpponentJoin = false;
            while (!didOpponentJoin)
            {
                if (_db.Games.FirstOrDefault(g => g.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && g.Player2Id != null) != null)
                    didOpponentJoin = true;
            }

            var gameJson = new JObject();
            gameJson["Id"] = game.Id;
            gameJson["Name"] = game.Name;
            gameJson["Rows"] = game.Rows;
            gameJson["Cols"] = game.Cols;
            gameJson["Player1Id"] = game.Player1Id;
            gameJson["Player2Id"] = game.Player2Id;
            gameJson["Maze"] = game.Maze;
            gameJson["Start"] = mazeJson["Start"];
            gameJson["End"] = mazeJson["End"];

            var currentPos = new JObject();
            currentPos["Row"] = ((JObject)mazeJson["Start"])["Row"];
            currentPos["Col"] = ((JObject)mazeJson["Start"])["Col"];
            gameJson["CurrentPos"] = currentPos;

            return Ok(gameJson);
        }

        [Route("api/Multiplayer/JoinGame")]
        [HttpPost]
        public IHttpActionResult JoinGame(JObject joinGameForm)
        {
            var sessionToken = joinGameForm["sessionToken"].Value<string>();
            var gameName = joinGameForm["name"].Value<string>();

            var user = _db.Users.FirstOrDefault(u => u.SessionToken.Equals(sessionToken));
            if (user == null)
            {
                return BadRequest("Your session has expired. Please log in again");
            }

            var game = _db.Games.FirstOrDefault(g => g.Name.Equals(gameName));
            if (game == null)
            {
                return BadRequest("Game has recently added. Please pick another game");
            }

            if (game.Player2Id != null)
            {
                return BadRequest("Two players currently playing the game you have requested. Please pick another game");
            }

            game.Player2Id = user.Id;
            try
            {
                _db.SaveChanges();
                return Ok(game);
            }
            catch (Exception e)
            {
                return BadRequest("Error occurred. Please try again");
            }
        }

        [Route("api/Multiplayer/GetGameState")]
        [HttpGet]
        public IHttpActionResult GetGameState(string sessionToken)
        {
            var user = _db.Users.FirstOrDefault(u => u.SessionToken.Equals(sessionToken));
            if(user == null)
            {
                return BadRequest("Your session has expired. Please re-login");
            }

            var game = _db.Games.FirstOrDefault(g => g.Player1Id == user.Id || g.Player2Id == user.Id);
            if (game == null)
            {
                return BadRequest($"{user.Username} is not playing in any game");
            }

            bool isReady = game.Player2Id != null;

            if (isReady) _movesHub.Connect(user.Id);

            return Ok(isReady ? "Ready" : "Waiting for player...");
        }

        [Route("api/Multiplayer/GetList")]
        [HttpGet]
        public IEnumerable<string> GetList()
        {
            var gameNames = new List<string>();
            var games = _db.Games.Where(g => g.Player2Id == null);
            
            foreach(var game in games)
            {
                gameNames.Add(game.Name);
            }

            return gameNames;
        }

        [Route("api/Multiplayer/PlayerWon")]
        [HttpGet]
        public IHttpActionResult PlayerWon(string sessionToken)
        {
            var winner = _db.Users.FirstOrDefault(u => u.SessionToken.Equals(sessionToken, StringComparison.OrdinalIgnoreCase));

            if(winner == null)
            {
                return BadRequest("Your session has expired. Could not report win");
            }

            var game = _db.Games.FirstOrDefault(g => g.Player1Id == winner.Id || g.Player2Id == winner.Id);

            if(game == null)
            {
                return BadRequest("Could not find game record.");
            }

            try
            {
                _db.Games.Remove(game);
            }
            catch (Exception)
            {
                return BadRequest("Could not delete game");
            }

            int? loserId = (game.Player1Id == winner.Id) ? game.Player2Id : game.Player1Id;
            var loser = _db.Users.FirstOrDefault(u => u.Id == loserId);

            if(loser == null)
            {
                return BadRequest("Could not update opponent score");
            }

            loser.Loses++;
            loser.Rate--;
            winner.Wins++;
            winner.Rate++;

            try
            {
                _db.SaveChanges();
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest("Could not update users scores");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}