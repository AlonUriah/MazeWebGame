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

        [Route("StartGame")]
        [HttpPost]
        public IHttpActionResult StartGame(JObject startGameForm)
        {
            var sessionToken = startGameForm["sessionToken"].Value<string>();
            var name = startGameForm["name"].Value<string>();
            var rows = startGameForm["rows"].Value<int>();
            var cols = startGameForm["cols"].Value<int>();

            var host = _db.Users.FirstOrDefault(u => u.SessionToken.Equals(sessionToken));
            if (host == null)
            {
                return BadRequest("Your session has expired. Please login again.");
            }

            var mazeStr = MazeHandler.GenerateMaze(rows, cols);
            var mazeJson = JObject.FromObject(mazeStr);
            var game = new Game() { Name = name,
                Rows = rows,
                Cols = cols,
                Player1Id = host.Id,
                MazeString = mazeJson["MazeString"].Value<string>() };

            try
            {
                _db.Games.Add(game);
                _db.SaveChanges();
                return Ok(game);
            }
            catch (Exception)
            {
                return BadRequest("Could not create this game, please try again later");
            }
        }

        [Route("JoinGame")]
        [HttpPost]
        public IHttpActionResult JoinGame(JObject joinGameForm)
        {
            var sessionToken = joinGameForm["usertoken"].Value<string>();
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
                _movesHub.Connect(user.Id);
                return Ok(game);
            }
            catch (Exception)
            {
                return BadRequest("Error occurred. Please try again");
            }
        }

        [Route("GetGameState")]
        [HttpGet]
        public IHttpActionResult GetGameState(string session)
        {
            var user = _db.Users.FirstOrDefault(u => u.SessionToken.Equals(session));
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

        [Route("GetGames")]
        [HttpGet]
        public IEnumerable<Game> GetGames()
        {
            // Return only available games
            return _db.Games.Where(g => g.Player2Id == null);
        }

        [ActionName("GetMoves")]
        public IEnumerable<Move> GetUserMoves(int playerId)
        {
            return _db.Moves.Where(m => m.PlayerId == playerId);
        }

        [ResponseType(typeof(Move))]
        public IHttpActionResult PostMove(JObject moveJason)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sessionToken = moveJason["sessionToken"].Value<string>();
            var user = _db.Users.FirstOrDefault(u => u.SessionToken.Equals(sessionToken));

            if(user == null)
            {
                return BadRequest("Session is expired. Please re-login");
            }

            var game = _db.Games.FirstOrDefault(g => g.Player1Id == user.Id || g.Player2Id == user.Id);

            if(game == null)
            {
                return BadRequest();
            }

            int opponentId = (game.Player1Id == user.Id) ? (game.Player2Id.Value) : (game.Player1Id);

            var move = new Move()
            {
                PlayerId = user.Id,
                Col = moveJason["col"].Value<int>(),
                Row = moveJason["row"].Value<int>(),
                OpponentId = opponentId,
                MoveTimestamp = DateTime.Now,
                GameId = game.Id
            };

            _movesHub.SendMove(user.Id, opponentId, move.ToString());

            _db.Moves.Add(move);
            _db.SaveChanges();
            return CreatedAtRoute("DefaultApi", new { id = move.PlayerId }, move);
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