using Core.Common;
using UnityEngine;

namespace Core.State {
    /// <summary>
    /// The StateHook object (as well as other state hooks potentially) serves as a globally known
    /// singleton object within the level that provides references to global dependencies
    /// that are necessary for the game to function. All CORE-supported dependencies are specified under
    /// BaseStateHook-type. Your GAME code can fetch the GAME-specific State Hook type to find GAME-specific
    /// dependencies as needed.
    /// Since some objects will need access to the state hook, this utility library serves as a way to
    /// commonly access components in the state hook as well as other state hook related helpers. You may
    /// cache the state hook reference as long as the lifetime of the object does not span in-between scenes.
    /// </summary>
    public static class GlobalObjectsUtility {
        /// <summary>
        /// Finds the "StateHook"-tagged object and retrieves the specified component.
        /// </summary>
        public static T FromStateHook<T>() where T : class {
            return GameObject.FindGameObjectWithTag(WellKnownStrings.StateHookTag).GetComponent<T>();
        }

        public static T FromBaseStateHook<T>() where T : class {
            var baseHook = FromStateHook<BaseStateHook>() as T;
            return baseHook;
        }

        public static T FromMainCameraProxy<T>() {
            var camera = GameObject.FindGameObjectWithTag(WellKnownStrings.MainCameraProxy);
            return camera.gameObject.GetComponent<T>();
        }

        public static GameObject[] FindPlayers() {
            return GameObject.FindGameObjectsWithTag(WellKnownStrings.PlayerTag);
        }

        public static GameObject FindPlayer() {
            return GameObject.FindGameObjectWithTag(WellKnownStrings.PlayerTag);
        }
    }
}
