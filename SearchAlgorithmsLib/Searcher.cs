using SearchAlgorithmsLib.Interfaces;
using Medallion.Collections;

namespace SearchAlgorithmsLib
{
    /// <summary>
    /// This is Searcher Base Class.
    /// It has features like how many nodes were visited during search
    /// and holds a PriorityQueue to search by a specific order.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Searcher<T> : ISearcher<T>
    {
        protected PriorityQueue<State<T>> _openList;
        protected int _evaluatedNodes;

        /// <summary>
        /// Get number of nodes visited during this Searcher search
        /// </summary>
        /// <returns>Number of nodes visited during this Searcher search</returns>
        public int GetNumberOfNodesEvaluated()
        {
            return _evaluatedNodes;
        }

        // Template method for looking in a Searchable structure
        public abstract Solution<T> Search(ISearchable<T> searchable);
    }
}
