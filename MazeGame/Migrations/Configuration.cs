using System;
using MazeGame.Models;

namespace MazeGame.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Security.Cryptography;
    using System.Text;

    internal sealed class Configuration : DbMigrationsConfiguration<MazeGame.Models.MazeAppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MazeGame.Models.MazeAppContext context)
        {
            context.Users.AddOrUpdate(x => x.Id,
            new User() { Id = 1, Username = "someUser1", Password = GetEncryptedPassword("somePassword")
            , Email = "u1@gmail.com", Wins = 0, Loses = 0, Rate = 0 },
            new User() { Id = 2, Username = "someUser2", Password = GetEncryptedPassword("somePassword")
            , Email = "u2@gmail.com", Wins = 0, Loses = 0, Rate = 0 },
            new User() { Id = 3, Username = "someUser3", Password = GetEncryptedPassword("somePassword")
            , Email = "u3@gmail.com", Wins = 0, Loses = 0, Rate = 0 });
        }

        private string GetEncryptedPassword(string password)
        {
            byte[] passByte = Encoding.UTF8.GetBytes(password);
            var shaProvider = new SHA1CryptoServiceProvider();
            byte[] encrypted = shaProvider.ComputeHash(passByte);
            return Encoding.UTF8.GetString(encrypted);
        }
    }
}
