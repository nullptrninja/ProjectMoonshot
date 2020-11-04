using System;
using System.Linq;
using UnityEngine;

namespace Core.Behaviors.Functional {
    /// <summary>
    /// Stores a list of keys that allows in-game behavior modification. To use this behavior, attach this component to
    /// any in-game GameObject and add whichever keys you wish to this script. Next, the behaviors that read this object
    /// can first Find it in the game world and check for a particular value and adjust their behavior based on that.
    /// When we're done with development, we can simply remove the DevConfig host object from the scene without permanently
    /// modifying other areas such as code, prefabs, or configuration files; and only just modify the scene. This should
    /// reduce errors introduced by having too much "dev-only" code being constantly modified to speed up the dev process.
    /// </summary>
    [Serializable]
    public class DevConfig : MonoBehaviour {
        public string[] Keys;
        
        public bool ContainsKey(string key) {
            if (!string.IsNullOrEmpty(key) && this.Keys != null) {
                return this.Keys.Any(o => o.Equals(key));
            }
            else {
                return false;
            }
        }
    }
}
