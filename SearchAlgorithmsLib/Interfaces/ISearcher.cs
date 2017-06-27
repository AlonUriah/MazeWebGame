namespace SearchAlgorithmsLib.Interfaces
{
    /// <summary>
    /// ISearcher interface. 
    /// Each Searcher must support the following.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISearcher<T>
    {
        /// <summary>
        /// Seek for a solution to get from Searchable start point
        /// to end point.
        /// </summary>
        /// <param name="searchable">An ISearchable data structure</param>
        /// <returns>A Solution composed by all states from initial Position to goal Position</returns>
        Solution<T> Search(ISearchable<T> searchable);
        
        /// <summary>
        /// Returns the number of states developed by this searcher
        /// </summary>
        /// <returns></returns>
        int GetNumberOfNodesEvaluated();
    }
}
