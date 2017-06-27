using System.Collections.Generic;

namespace SearchAlgorithmsLib.Interfaces
{
    /// <summary>
    /// ISearchable interface. 
    /// Relevant for any data structures that supports the following methods.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISearchable<T>
    {
        /// <summary>
        /// Returns a starting point of data structure
        /// </summary>
        /// <returns>An initial State</returns>
        State<T> GetInitialState();

        /// <summary>
        /// Returns an ending point of data structure
        /// </summary>
        /// <returns>A goal State</returns>
        State<T> GetGoalState();

        /// <summary>
        /// Returns a list of all possible moves from a given state
        /// </summary>
        /// <param name="state">Current location of seeker</param>
        /// <returns>All possible moves</returns>
        List<State<T>> GetAllPossibleStates(State<T> state);
    }
}
