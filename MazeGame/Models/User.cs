using System.ComponentModel.DataAnnotations;

namespace MazeGame.Models
{
    public class User
    {
        [Required]
        public int Id { set; get; }
        [Required]
        public string Username { set; get; }
        [Required]
        public string Password { set; get; }
        [Required]
        public string Email { set; get; }

        public string SessionToken { set; get; }
        public int Wins { set; get; }
        public int Loses { set; get; }
        public int Rate { set; get; }
    }
}