using System.Data.Entity;

namespace MazeGame.Models
{
    public class MazeAppContext : DbContext
    {
        public MazeAppContext() : base("name=MazeAppContext")
        {
        }
        public DbSet<User> Users { set; get; }
        public DbSet<Game> Games { set; get; }
    }
}