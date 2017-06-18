using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using MazeGame.Models;

namespace MazeGame.Controllers
{
    public class MultiplayerController : ApiController
    {
        private MazeAppContext db = new MazeAppContext();

        [ActionName("GetMoves")]
        public IEnumerable<Move> GetUserMoves(int playerId)
        {
            return db.Moves.Where(m => m.PlayerId == playerId);
        }

        [ResponseType(typeof(Move))]
        public IHttpActionResult PostMove(Move move)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Moves.Add(move);
            db.SaveChanges();
            return CreatedAtRoute("DefaultApi", new { id = move.PlayerId }, move);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
