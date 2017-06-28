using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using MazeGame.Models;
using Newtonsoft.Json.Linq;
using MazeProjectLibrary;

namespace MazeGame.Controllers
{
    public class MultiplayerController : ApiController
    {
        private MovesHub _movesHub = new MovesHub();
        private MazeAppContext _db = new MazeAppContext();

        /// <summary>
        /// Start multiplayer game.
        /// Create game from request parameters (name, rows and cols).
        /// Add it to database.
        /// Waits for opponent to join.
        /// Once joined, return Game jason to host player.
        /// </summary>
        /// <param name="startGameForm"></param>
        /// <returns></returns>
        [Route("api/Multiplayer/StartNewGame")]
        [HttpPost]
        public IHttpActionResult StartGame(JObject startGameForm)
        {
            Game game = null;
            
            // Try create a Game object based on request parameters
            if (!TryCreateGame(startGameForm, out game))
            {
                return BadRequest("Could not found one or more parameters");
            }

            // Update Games table so other uses can see this game
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
            int opponentId;
            BusyWaitToOpponent(game.Name, out opponentId);

            _db = new MazeAppContext();
            game = _db.Games.FirstOrDefault(g => g.Name == game.Name);

            // Try create game response in client expected format
            JObject gameJson;
            if(!TryCreateGameResponse(game, out gameJson))
            {
                return BadRequest("Could not create game response");
            }

            return Ok(gameJson);
        }

        /// <summary>
        /// Try create Game object from request parameters.
        /// Expected fields are name, rows and cols.
        /// </summary>
        /// <param name="startGameRequest">Request to create a new game</param>
        /// <param name="game">Game object based on request parameters</param>
        /// <returns>True if all mandatory fields were included in request, false otherwise</returns>
        private bool TryCreateGame(JObject startGameRequest, out Game game)
        {
            string sessionToken, name;
            int rows, cols;
            game = null;

            // Get request parameters
            try
            {
                sessionToken = startGameRequest["sessionToken"].Value<string>();
                name = startGameRequest["name"].Value<string>();
                rows = startGameRequest["rows"].Value<int>();
                cols = startGameRequest["cols"].Value<int>();
            }
            catch (Exception)
            {
                // Some mandatory parameters are missing
                return false;
            }

            var host = _db.Users.FirstOrDefault(u => u.SessionToken.Equals(sessionToken));
            if (host == null)
            {
                // Could not retreive host player Id
                return false;
            }

            var mazeJson = MazeHandler.GenerateMaze(rows, cols);

            game = new Game()
            {
                Name = name,
                Rows = rows,
                Cols = cols,
                Player1Id = host.Id,
                Maze = mazeJson["Maze"].Value<string>()
            };

            return true;
        }

        /// <summary>
        /// Wait for opponent to connect game.
        /// </summary>
        /// <param name="gameName">Game to suspect</param>
        /// <param name="opponentId">Returns opponent id</param>
        private void BusyWaitToOpponent(string gameName, out int opponentId)
        {
            Game game;
            bool didOpponentJoin = false;

            // Stop waiting only when this game has Player2Id value
            while (!didOpponentJoin)
            {
                if ((game = _db.Games.FirstOrDefault(g => g.Name.Equals(gameName) && g.Player2Id != null)) != null)
                    didOpponentJoin = true;
            }

            // Refresh app context
            _db = new MazeAppContext();

            // Get updated game and keep opponentId
            game = _db.Games.FirstOrDefault(g => g.Name.Equals(gameName, StringComparison.OrdinalIgnoreCase)
                                        && g.Player2Id != null);
            // Initialize opponentId from database game, it is updated now.
            opponentId = game.Player2Id.Value;
        }

        /// <summary>
        /// Try create game response in expected format.
        /// </summary>
        /// <param name="game">Game to send</param>
        /// <param name="response">Game in client expected format</param>
        /// <returns>True if managed to create game jason response, false otherwise</returns>
        private bool TryCreateGameResponse(Game game, out JObject response)
        {
            response = new JObject();
            try
            {
                response["Id"] = game.Id;
                response["Name"] = game.Name;
                response["Rows"] = game.Rows;
                response["Cols"] = game.Cols;
                response["Player1Id"] = game.Player1Id;
                response["Player2Id"] = game.Player2Id;
                response["Maze"] = game.Maze;

                // Extract Start and End from Maze string
                JObject JObstartEndJobject = ExtractStartAndEndFromMaze(game.Maze, game.Rows, game.Cols);

                response["Start"] = JObstartEndJobject["Start"];
                response["End"] = JObstartEndJobject["End"];
                response["CurrentPos"] = JObstartEndJobject["Start"];
                response["OppPos"] = JObstartEndJobject["Start"];
            }
            catch (Exception)
            {
                // Some of the fields were missing. 
                return false;
            }

            return true;
        }

        /// <summary>
        /// Pulls Game by name from db.
        /// Update its Player2Id (since this method JOINS to an
        /// existing game, and not hosting it).
        /// Return game in client's expected format.
        /// </summary>
        /// <param name="joinGameForm">Request to join game, expected sessionToken and game name</param>
        /// <returns>Game in client expected format</returns>
        [Route("api/Multiplayer/JoinGame")]
        [HttpPost]
        public IHttpActionResult JoinGame(JObject joinGameForm)
        {
            string sessionToken;
            string gameName;

            try
            {
                sessionToken = joinGameForm["sessionToken"].Value<string>();
                gameName = joinGameForm["name"].Value<string>();
            }
            catch (Exception)
            {
                return BadRequest("Some mandatory fields were missing in request");
            }

            // Get relevant Game and User instanced by parameters
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

            // Update Game record with joined player id (Player2Id)
            game.Player2Id = user.Id;
            try
            {
                _db.Entry<Game>(game).Property(p => p.Player2Id).IsModified = true;
                _db.SaveChanges();
            }
            catch (Exception)
            {
                return BadRequest("Error occurred. Please try again");
            }

            // Adapt response to expected format
            JObject gameJson;
            if(!TryCreateGameResponse(game,out gameJson))
            {
                return BadRequest("Error generating game response");
            }

            return Ok(gameJson);
        }

        /// <summary>
        /// Generate Json object describing
        /// Start and End positions (Row and Col) by parsing 
        /// Maze string
        /// </summary>
        /// <param name="mazeStr">Maze string as generated by MazeGenerator</param>
        /// <param name="rows">Number of rows to calculate row/col</param>
        /// <param name="cols">Number of cols to calculate row/col</param>
        /// <returns></returns>
        private JObject ExtractStartAndEndFromMaze(string mazeStr, int rows, int cols)
        {
            int startIndex = mazeStr.IndexOf("*");
            int endIndex = mazeStr.IndexOf("#");

            if (startIndex < 0 || endIndex < 0)
                throw new InvalidExpressionException("Could not detect Start/End symbols in maze string");

            // Using strings and diviation truncates result and returns the right row
            var start = new JObject();
            start["Row"] = startIndex / cols;
            start["Col"] = startIndex - (startIndex / cols) * cols;

            var end = new JObject();
            end["Row"] = endIndex / cols;
            end["Col"] = endIndex - (endIndex / cols) * cols;

            // Return value
            var startEndObject = new JObject();
            startEndObject["Start"] = start;
            startEndObject["End"] = end;

            return startEndObject;
        }

        /// <summary>
        /// Returns a list of valid games to join.
        /// Valid game - games with host waiting.
        /// </summary>
        /// <returns>A list of Game names</returns>
        [Route("api/Multiplayer/GetList")]
        [HttpGet]
        public IEnumerable<string> GetList()
        {
            var gameNames = new List<string>();
            var games = _db.Games.Where(g => g.Player2Id == null);
            
            // Add relevant games to gameNames list
            foreach (var game in games)
            {
                gameNames.Add(game.Name);
            }

            return gameNames;
        }

        /// <summary>
        /// Winner is responsible for letting server know
        /// game is over.
        /// This method:
        /// 1. Updates winner/loser Wins/Losses/Rank
        /// 2. Remove Game from Games table
        /// </summary>
        /// <param name="sessionToken">Winner sessionToken, used to identify User</param>
        /// <returns>Ok if managed to do 1-2, BadRequest otherwise</returns>
        [Route("api/Multiplayer/PlayerWon/{sessionToken}")]
        [HttpGet]
        public IHttpActionResult PlayerWon(string sessionToken)
        {
            // Find winner by his sessionToken
            User winner = _db.Users.FirstOrDefault(u => u.SessionToken.Equals(sessionToken));
            if (winner == null)
            {
                return BadRequest("Your session has expired. Could not report win");
            }

            // Find game by players (winner is either Player1Id or Player2Id)
            Game game = _db.Games.FirstOrDefault(g => g.Player1Id == winner.Id || g.Player2Id == winner.Id);
            if (game == null)
            {
                return BadRequest("Could not find game record.");
            }

            try
            {
                // Remove Game from games list
                _db.Games.Remove(game);
            }
            catch (Exception)
            {
                return BadRequest("Could not delete game");
            }

            // Get loser id to update its losses/rank as well
            int loserId = (game.Player1Id == winner.Id) ? game.Player2Id.Value : game.Player1Id;
            var loser = _db.Users.FirstOrDefault(u => u.Id == loserId);

            if (loser == null)
            {
                return BadRequest("Could not update opponent score");
            }

            // Update losses/wins/ranks at both winner and loser
            loser.Loses++;
            loser.Rate--;
            winner.Wins++;
            winner.Rate++;

            try
            {
                var updatedLoser = _db.Entry<User>(loser);
                updatedLoser.Property(p => p.Loses).IsModified = true;
                updatedLoser.Property(p => p.Rate).IsModified = true;

                var updatedWinner = _db.Entry<User>(winner);
                updatedLoser.Property(p => p.Wins).IsModified = true;
                updatedLoser.Property(p => p.Rate).IsModified = true;

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