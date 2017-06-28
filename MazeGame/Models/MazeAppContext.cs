using System.Data.Entity;

namespace MazeGame.Models
{
    public class MazeAppContext : DbContext
    {
        public MazeAppContext() : base("name=MazeAppContext")
        {
        }

        // Users table in db.
        public DbSet<User> Users { set; get; }

        // Games table in db
        public DbSet<Game> Games { set; get; }
    }
}