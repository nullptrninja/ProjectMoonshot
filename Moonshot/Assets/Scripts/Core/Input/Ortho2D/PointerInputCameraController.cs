using System;
using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace Core.Input.Ortho2D {
    // Handles both mouse and touch input to control the camera.
    //
    // Adapted from https://kylewbanks.com/blog/unity3d-panning-and-pinch-to-zoom-camera-with-touch-and-mouse-input
    public class PointerInputCameraController : MonoBehaviour {
        public Camera TargetCamera;
        public bool EnablePanning = true;
        public float PanSpeed = 30f;
        public int MouseButtonIndexForPan = 0;

        public Action<Vector3> OnBeginPanAction { get; set; }
        public Action<Vector3> OnFinishedPanAction { get; set; }

        private Vector3 mPanningPosition;
        private bool mIsPanning;

        public void Update() {
            HandleMouseInput();
        }

        private void HandleMouseInput() {
            if (this.EnablePanning) {
                if (UnityInput.GetMouseButtonDown(this.MouseButtonIndexForPan)) {
                    mPanningPosition = UnityInput.mousePosition;
                }
                else if (UnityInput.GetMouseButton(this.MouseButtonIndexForPan)) {
                    // Only trigger this once if we actually moved.
                    // TODO: Determine if we need to add a 'deadzone' for panning difference detection
                    if (!mIsPanning && UnityInput.mousePosition != mPanningPosition) {
                        mIsPanning = true;
                        this.OnBeginPanAction?.Invoke(mPanningPosition);
                    }

                    PanCameraTo(UnityInput.mousePosition);
                }
                else if (UnityInput.GetMouseButtonUp(this.MouseButtonIndexForPan)) {
                    mIsPanning = false;
                    if (this.OnFinishedPanAction != null) {
                        this.OnFinishedPanAction(mPanningPosition);
                    }                    
                }
            }
        }

        private void PanCameraTo(Vector3 position) {
            var offset = this.TargetCamera.ScreenToViewportPoint(mPanningPosition - position);
            var scaledMove = new Vector3(offset.x * this.PanSpeed, offset.y * this.PanSpeed);

            this.TargetCamera.transform.Translate(scaledMove, Space.World);            
            mPanningPosition = position;
        }
    }
}
