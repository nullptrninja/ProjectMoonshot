using UnityEngine;
using System.Collections;

namespace Core.Behaviors {
    /// <summary>
    /// Always sets this object's orientation to be facing the camera. By default, the main camera is selected if no other
    /// camera is specified.
    /// </summary>
    public class BillboardSprite : MonoBehaviour {

        /// <summary>
        /// If true, will only adjust the billboarding once. Improves performance if the camera remains statically aimed.
        /// </summary>
        public bool LookAtCameraOnce = true;

        /// <summary>
        /// Multiplies the object's current local scale with these values once on start
        /// </summary>
        public Vector3 ScaleAdjustment = Vector3.one;

        /// <summary>
        /// The camera to look at. If null, will use the default camera in the scene.
        /// </summary>
        public Camera LookAtCamera = null;

        private void Start() {
            if (this.LookAtCamera == null) {
                this.LookAtCamera = Camera.main;
            }

            Vector3 scale = this.transform.localScale;
            this.transform.localScale = new Vector3(scale.x * this.ScaleAdjustment.x, 
                                                    scale.y * this.ScaleAdjustment.y,
                                                    scale.z * this.ScaleAdjustment.z);
            
            this.transform.LookAt(this.transform.position + this.LookAtCamera.transform.rotation * Vector3.back,
                                  this.LookAtCamera.transform.rotation * Vector3.up);
        }

        private void Update() {
            // No need to do the same thing each frame, camera is set once
            if (!this.LookAtCameraOnce) {
                this.transform.LookAt(this.transform.position + this.LookAtCamera.transform.rotation * Vector3.back,
                                      this.LookAtCamera.transform.rotation * Vector3.up);
            }
        }
    }
}