using MazeLib;

namespace MazeProjectLibrary.Common
{
    public static class Extensions
    {
        public static bool EqualsLower(this string arg1, string arg2)
        {
            return (arg1.ToLower().Equals(arg2.ToLower()));
        }

        public static int CompareTo(this Position position, Position other)
        {
            return 1;
        }

        /// <summary>
        /// Returns a relative direction from a given Position to another
        /// </summary>
        /// <param name="from">Current Position</param>
        /// <param name="to">Goal Position</param>
        /// <returns>Direction between from and to</returns>
        public static Direction Subtract(this Position from, Position to)
        {
            int fromX = from.Col, fromY = from.Row;
            int toX = to.Col, toY = to.Row;

            // Assumption - no diagonal moves are allowed
            if (toX - fromX == -1) return Direction.Left;
            else if (toX - fromX == 1) return Direction.Right;
            else if (toY - fromY == -1) return Direction.Down;
            else return Direction.Up;
        }

    }
}
