using UnityEngine;

namespace Core.Behaviors.Functional {
    /// <summary>
    /// Describes a GameObject stored in the Resource folder for use with BaseResourceLibrary. This allows you
    /// to precache certain objects at start up for easy spawning (at the cost of memory of course)
    /// </summary>
    public class ResourceDescriptor : MonoBehaviour {
        public int ResourceId;                  // An ID for the resource. This ID must be unique within a given subdirectory.
        public bool IsPreCacheable = true;      // If true, the resource library will cache this object
    }
}
