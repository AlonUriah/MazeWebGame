using System.ComponentModel.DataAnnotations;

namespace MazeGame.Models
{
    /// <summary>
    /// User Model.
    /// Keeps credential information.
    /// Session Token will be used to validate active session and will
    /// only has value in it once User was logged in.
    /// </summary>
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