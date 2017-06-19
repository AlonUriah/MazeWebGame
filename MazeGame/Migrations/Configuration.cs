using System;
using MazeGame.Models;

namespace MazeGame.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<MazeGame.Models.MazeAppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MazeGame.Models.MazeAppContext context)
        {
            context.Moves.AddOrUpdate(x => x.Id,
                new Move() { Id = 1, PlayerId = 1, OpponentId = 2, Col = 5, Row = 5, GameId = 1, MoveTimestamp = DateTime.Now},
                new Move() { Id = 2, PlayerId = 2, OpponentId = 1, Col = 5, Row = 6, GameId = 1, MoveTimestamp = DateTime.Now},
                new Move() { Id = 3, PlayerId = 1, OpponentId = 2, Col = 5, Row = 4, GameId = 1, MoveTimestamp = DateTime.Now}
            );

            context.Users.AddOrUpdate(x => x.Id,
                new User() { Id = 1, Username = "someUser1", Password = "somePassword"
                ,Email = "u1@gmail.com", Wins = 0, Loses = 0, Rate = 0},
                new User() { Id = 2, Username = "someUser2", Password = "somePassword"
                ,Email = "u2@gmail.com", Wins = 0, Loses = 0, Rate = 0},
                new User() { Id = 3, Username = "someUser3", Password = "somePassword"
                ,Email = "u3@gmail.com", Wins = 0, Loses = 0, Rate = 0}
            );
        }
    }
}
