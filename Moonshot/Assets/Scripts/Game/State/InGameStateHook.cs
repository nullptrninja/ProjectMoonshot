using Core.State;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Game.State {
    /// <summary>
    /// State hooks allow us to run code that don't have a physical existance in the scene but need running once per frame.
    /// </summary>
    public class InGameStateHook : BaseStateHook {
        // The public scriptableobject fields are the injectable references
        public DependencyContainer Dependencies;

        protected override void OnLevelLoad(Scene scene, LoadSceneMode mode) {
            // Setup Dependency injections from DependencyContainer

            // Any other On-Level-Load events occur here
        }

        protected override void OnLevelUnload() {
        }

        public void Update() {
            // Regular game update events here
        }

        public void LateUpdate() {
            // LastUpdateOnce() is only executed once per frame, however it may
            // receive multiple executions from different sources since we cannot
            // guarantee that this state hook's LateUpdate() is the first one called.
        }
    }
}