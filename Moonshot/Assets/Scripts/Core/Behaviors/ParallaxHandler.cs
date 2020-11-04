using UnityEngine;

namespace Core.Behaviors {
    /// <summary>
    /// Adds parallax scaled movment relative to a specified camera. The best way to use this component is to group
    /// each parallaxable layer into a root object. This root object will contain an instance of this ParallaxHandler
    /// component. From there you can configure this component as needed, representing a layer in the overall
    /// parallax layer set.
    /// </summary>
    public class ParallaxHandler : MonoBehaviour {

        /// <summary>
        /// The camera that this handler will respond to.
        /// </summary>
        public Camera InputCamera;

        /// <summary>
        /// How much this object moves in relation to the camera's movement (multiplied against
        /// the camera's movement vector)
        /// </summary>
        public float ParallaxScale = -0.2f;

        /// <summary>
        /// If true, horizontal movement is prevented.
        /// </summary>
        public bool LockHorizontalMovement = false;

        /// <summary>
        /// If true, vertical movement is prevented.
        /// </summary>
        public bool LockVerticalMovement = false;

        private Vector3 mLastCameraPosition;        

        private void Start() {   
            if (InputCamera == null) {
                InputCamera = Camera.main;
            }
            mLastCameraPosition = this.InputCamera.transform.position;
        }

        private void Update() {
            Vector3 delta = GetParallaxDelta();

            if (this.LockHorizontalMovement) {
                this.transform.position += new Vector3(0f, delta.y, delta.z);
            }

            if (this.LockVerticalMovement) {
                this.transform.position += new Vector3(delta.x, 0f, delta.z);
            }

            if (!this.LockHorizontalMovement && !this.LockVerticalMovement) {
                this.transform.position += delta;
            }
            
            mLastCameraPosition = this.InputCamera.transform.position;
        }

        private Vector3 GetParallaxDelta() {
            return (this.InputCamera.transform.position - mLastCameraPosition) * this.ParallaxScale;
        }
    }
}
