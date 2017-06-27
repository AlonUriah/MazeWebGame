using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SearchAlgorithmsLib.Interfaces;

namespace SearchAlgorithmsLib.Searchers
{
    /// <summary>
    /// Searcher implementing DFS Algorithm.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DFS<T> : Searcher<T>
    {
        private HashSet<State<T>> _visited;

        /// <summary>
        /// Runs recursively DFS Search.
        /// Returns true if DFS searching should be stopped (found Goal)
        /// </summary>
        /// <param name="searchable">Searchable structure</param>
        /// <param name="current">Current state</param>
        /// <returns>True if reached to Goal State</returns>
        private bool RecursiveDFS(ISearchable<T> searchable, State<T> current)
        {
            // Return true when reached to Goal State.
            if (current.Equals(searchable.GetGoalState()))
            {
                return true;
            }

            var neighbors = searchable.GetAllPossibleStates(current);
            bool shouldStop; 

            foreach (var neighbor in neighbors)
            {
                if (!_visited.Contains(neighbor))
                {
                    neighbor.Parent = current;
                    _visited.Add(neighbor);
                    _evaluatedNodes++;

                    // Keep looking in this direction (DFS Algo)
                    shouldStop = RecursiveDFS(searchable, neighbor);
                    if (shouldStop)
                    {
                        break;
                    }
                }
            }

            // No solution was found
            return false;
        }

        /// <summary>
        /// ISearcher implemetation. 
        /// Calling DFS Recursive Search starting from Init Position.
        /// </summary>
        /// <param name="searchable"></param>
        /// <returns></returns>
        public override Solution<T> Search(ISearchable<T> searchable)
        {
            _visited = new HashSet<State<T>>();
            var initialState = searchable.GetInitialState();

            RecursiveDFS(searchable, initialState);

            var goalState = searchable.GetGoalState();
            var goal = _visited.FirstOrDefault(z => z.Equals(goalState));

            // If goal is null it means that goal was not visited, hence no route from Init Pos
            if (goal == null)
            {
                return new Solution<T>();
            }

            // Return solution
            return Solution<T>.Backtrace(goal);
        }
    }
}
