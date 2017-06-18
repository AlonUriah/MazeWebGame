using MazeGame.Models;

namespace MazeGame.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MazeGame.Models.MazeAppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MazeGame.Models.MazeAppContext context)
        {
            context.Moves.AddOrUpdate(x => x.Id,
                new Move() { Id = 1, PlayerId = 1, OpponentId = 2, Col = 5, Row = 5},
                new Move() { Id = 2, PlayerId = 2, OpponentId = 1, Col = 5, Row = 6 },
                new Move() { Id = 3, PlayerId = 1, OpponentId = 2, Col = 5, Row = 4 }
            );
        }
    }
}
