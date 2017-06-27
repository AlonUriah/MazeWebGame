using Newtonsoft.Json.Linq;

namespace SearchAlgorithmsLib.Interfaces
{
    /// <summary>
    /// ISolutionBuilder Interface.
    /// Exposes JToken indexers to give JObject feel-like
    /// and ToJason().
    /// </summary>
    public interface ISolutionJasonBuilder
    {
        /// <summary>
        /// Sets/Gets JObject property
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns>JToken of JObject to return</returns>
        JToken this[string propertyName] { set; get; }
        
        /// <summary>
        /// Returns JObject that has been built using this Builder
        /// </summary>
        /// <returns>JObject with selected properties</returns>
        JObject ToJason();
    }
}
