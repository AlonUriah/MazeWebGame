using System;

namespace SearchAlgorithmsLib
{
    /// <summary>
    /// Class representing State.
    /// State can be desribed by any T object.
    /// It has cost and a Parent (which is a nullable State).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class State<T> //where T : IComparable
    {
        private T _state;
        public double Cost { set; get; }
        public State<T> Parent { set; get; }

        /// <summary>
        /// Returns this State's value
        /// </summary>
        public T Value { get {return _state;} }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="state"></param>
        public State(T state)
        {
            _state = state;
            Cost = 0;
            Parent = null;
        }
        
        /// <summary>
        /// Equals implementation.
        /// Comparing inner _state of two objects.
        /// If obj is not a State, automatically return false.
        /// </summary>
        /// <param name="obj">Other object to compare with</param>
        /// <returns>True if obj is another State with the same inner T state</returns>
        public override bool Equals(object obj)
        {
            var stateObj = obj as State<T>;
            
            // If object is not a T state, automatically return false.
            if (stateObj == null) return false;

            // Compare by inner _states
            return _state.Equals(stateObj.Value);
        }

        /// <summary>
        /// IComparable implementation.
        /// Comparing this State to another object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            IComparable compState = _state as IComparable;

            /* 
             * If inner state is not IComparable, just compare HashCodes
             * Alternative was to throw an Exception. 
             */
            if(compState == null)
            {
                return GetHashCode().CompareTo(obj.GetHashCode());
            }

            // Compare by inner State
            return compState.CompareTo(obj);
        }

        /// <summary>
        /// States string representation
        /// is their inner _state T string representation
        /// </summary>
        /// <returns>String representation of this State</returns>
        public override string ToString()
        {
            return _state.ToString();
        }
        
        /// <summary>
        /// States HashCode are their inner _states T
        /// Hash codes
        /// </summary>
        /// <returns>Hashcode for this state</returns>
        public override int GetHashCode()
        {
            return _state.GetHashCode();
        }
    }
}
