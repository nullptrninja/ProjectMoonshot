using Core.Systems.Messaging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.State {
    public abstract class BaseStateHook : MonoBehaviour {
        // As part of the CORE library, all state hook will provide these minimum services/systems
        // to CORE-library objects. Be sure to add them here instead of bad static patterns.
        public MessageRouter GameMessageRouter { get; protected set; }
        public Vector3 LastFrameCursorPosition { get; private set; }

        public void OnEnable() {
            SceneManager.sceneLoaded += this.OnLevelLoad;
        }

        public void OnDisable() {
            OnLevelUnload();
            SceneManager.sceneLoaded -= this.OnLevelLoad;
        }

        public void BaseLateUpdate() {
            if (UnityEngine.Input.touchCount > 0) {
                this.LastFrameCursorPosition = UnityEngine.Input.touches[0].position;
            }
            else {
                this.LastFrameCursorPosition = UnityEngine.Input.mousePosition;
            }
        }

        protected abstract void OnLevelLoad(Scene scene, LoadSceneMode mode);

        protected abstract void OnLevelUnload();
    }
}
