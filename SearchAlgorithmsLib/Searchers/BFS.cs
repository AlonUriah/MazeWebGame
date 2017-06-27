using System.Collections.Generic;
using SearchAlgorithmsLib.Interfaces;
using Medallion.Collections;

namespace SearchAlgorithmsLib.Searchers
{
    /// <summary>
    /// Searcher implementing BFS Algorithm. 
    /// Algorithm holds a list of visited nodes and try to find 
    /// the shortest way (using a comperator) from initial state of a Searchable
    /// object to goal state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BFS<T> : Searcher<T>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="comparer">A Comperator to manage the priority queue according to</param>
        public BFS(IComparer<State<T>> comparer = null)
        {
            _openList = (comparer != null) ? new PriorityQueue<State<T>>(comparer) : new PriorityQueue<State<T>>();
        }

        /// <summary>
        /// Private method to assist Search. 
        /// Check if a potential route to a given node is cheaper than
        /// existing route
        /// </summary>
        /// <param name="parentOption">Neighbor of node, optional parent</param>
        /// <param name="node">A state in a Searchable object</param>
        /// <returns>True if route to node throguh parentOption is better than existing route</returns>
        private bool IsNewPathBetter(State<T> parentOption, State<T> node)
        {
            //Assumption - all links between nodes are equal to 1
            return node.Cost > parentOption.Cost + 1;
        }

        /// <summary>
        /// ISearcher implementation. 
        /// Search from an Init position to Goal position in a searchable structure.
        /// </summary>
        /// <param name="searchable">A searchable structure holding an Init and Goal positions</param>
        /// <returns>A route represented in a Solution</returns>
        public override Solution<T> Search(ISearchable<T> searchable)
        {
            var initState = searchable.GetInitialState();
            _openList.Enqueue(searchable.GetInitialState());
            _evaluatedNodes++;

            var closed = new HashSet<State<T>>();

            while (_openList.Count > 0)
            {
                // Open list is implemented as a Priority queue, take best option
                State<T> currentState = _openList.Dequeue();
                // Mark as visited
                closed.Add(currentState);

                // Reached goal state
                if (currentState.Equals(searchable.GetGoalState()))
                {
                    return Solution<T>.Backtrace(currentState);
                }

                List<State<T>> succerssors = searchable.GetAllPossibleStates(currentState);
                foreach (var succerssor in succerssors)
                {
                    _evaluatedNodes++;
                    if (!closed.Contains(succerssor) && !_openList.Contains(succerssor))
                    {
                        succerssor.Parent = currentState;
                        succerssor.Cost = currentState.Cost + 1;
                        _openList.Enqueue(succerssor);
                    }
                    else if(IsNewPathBetter(currentState,succerssor))  // if new path is better than previous one 
                    {
                        if (_openList.Contains(succerssor))
                        {
                            _openList.Remove(succerssor);
                        }
                        
                        // Update state's cost
                        succerssor.Cost = currentState.Cost + 1;
                        _openList.Enqueue(succerssor);                            
                    }
                }
            }

            // Could not find a solution, empty trace
            return new Solution<T>(); 
        }
    }
}
