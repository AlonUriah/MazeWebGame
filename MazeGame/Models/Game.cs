using System.ComponentModel.DataAnnotations;

namespace MazeGame.Models
{
    /// <summary>
    /// Game Model represents a Game in db.
    /// Its members are straight forward except - 
    /// Player1Id which I have used to capture Host Id
    /// Player2Id which I have used to capture Joinned Opponent Id
    /// </summary>
    public class Game
    {
        [Required]
        public int Id { set; get; }
        [Required]
        public string Name { set; get; }
        [Required]
        public int Rows { set; get; }
        [Required]
        public int Cols { set; get; }
        [Required]
        public string Maze { set; get; }
        [Required]
        public int Player1Id { set; get; }
        public int? Player2Id { set; get; }
    }
}