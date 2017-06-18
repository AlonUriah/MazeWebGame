using System.ComponentModel.DataAnnotations;

namespace MazeGame.Models
{
    public class Move
    {
        public int Id { set; get; }
        [Required]
        public int PlayerId { set; get; }
        [Required]
        public int OpponentId { set; get; }
        [Required]
        public int Row { set; get; }
        public int Col { set; get; }
    }
}