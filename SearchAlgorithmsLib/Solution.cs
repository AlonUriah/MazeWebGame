using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;

namespace SearchAlgorithmsLib
{
    /// <summary>
    /// Solution Class.
    /// Holding a HashSet to keep track of nodes including in this Solution.
    /// Holding a Stack of States - top meant to be Init State, bottom meant to be Goal State
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Solution<T>
    {
        private readonly HashSet<T> _values = new HashSet<T>();
        public Stack<State<T>> States { set; get; }

        /// <summary>
        /// Ctor
        /// </summary>
        public Solution()
        {
            States = new Stack<State<T>>();
        }

        /// <summary>
        /// Backtraces from a goal state to initial state throush
        /// States Parents
        /// </summary>
        /// <param name="goal">Goal State in a Searchable structure</param>
        /// <returns>A Solution composed by all states from Init to Goal</returns>
        public static Solution<T> Backtrace(State<T> goal)
        {
            // A Solution to return
            var solution = new Solution<T>();
            State<T> current = goal;
            
            // State backtracking, from Goal State to Init state (Init has no Parent = null)
            do
            {
                solution.States.Push(current);
                // To anwers queries about States route in O(1)
                solution._values.Add(current.Value);
            } while ((current = current.Parent) != null);

            return solution;
        }

        /// <summary>
        /// Specifically for this qurey I have used a HashSet
        /// </summary>
        /// <param name="value">A Value that may or may not been a part of this Solution</param>
        /// <returns>True if Solution goes through value</returns>
        public bool Contains(T value)
        {
            return _values.Contains(value);
        }

        /// <summary>
        /// Returns true if this solution is not empty
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return States.Count == 0;
        }

        /// <summary>
        /// Returns a string representation of a Jason Solution
        /// This is a default Implemetaiton.
        /// 
        /// PS - I assumed specific orders about how to represent Solution
        /// should not be a part of Generic Solution Clas, hence I have
        /// used the most default implementation I could have think of.
        /// Also, specific implementation will work with Position, should 
        /// not be located in here!
        /// </summary>
        /// <returns>A JObject describing this Solution</returns>
        public JObject ToJason()
        {            
            var enumerator = States.GetEnumerator();
            var statesBuilder = new StringBuilder();

            // Concat togeter States that are part of this solution
            while (enumerator.MoveNext())
            {
                statesBuilder.Append(string.Format("{0},", enumerator.Current.Value));
            }

            var jasonSolution = new JObject();
            jasonSolution["States"] = statesBuilder.ToString();
            return jasonSolution;
        }
    }
}
