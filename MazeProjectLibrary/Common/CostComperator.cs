using System.Collections.Generic;
using SearchAlgorithmsLib;

namespace MazeProjectLibrary.Common
{
    /// <summary>
    /// Compares two generic State objects
    /// </summary>
    /// <typeparam name="T">A state key</typeparam>
    class CostComperator<T> : IComparer<State<T>>
    {
        /// <summary>
        /// Returns 0 if two states are equal.
        /// Returns 1 if x > y.
        /// Returns -1 if y > x.
        /// </summary>
        /// <param name="x">First state to compare</param>
        /// <param name="y">Second state to compare</param>
        /// <returns>Relative ration between x and y as described on top</returns>
        public int Compare(State<T> x, State<T> y)
        {
            double xCost = x.Cost;
            double yCost = y.Cost;

            if (xCost == yCost) return 0;
            else if (xCost - yCost < 0) return -1;
            else return 1;
        }
    }
}
