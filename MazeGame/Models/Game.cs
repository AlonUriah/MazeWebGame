using System.ComponentModel.DataAnnotations;

namespace MazeGame.Models
{
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