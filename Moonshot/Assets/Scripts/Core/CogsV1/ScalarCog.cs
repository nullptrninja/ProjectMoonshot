using Core.Utility;
using UnityEngine;

namespace Core.CogsV1 {
    /// <summary>
    /// Allows you to easily perform scaling animations with alpha support on any GameObject containing a SpriteRenderer.
    /// Note that this component is both standalone as well as has COG functionality.
    /// </summary>
    public class ScalarCog : CogAdapter {

        public Vector3 DeactivatedScale = new Vector3(1f, 1f, 1f);
        public Vector3 ActivatedScale = new Vector3(1f, 1f, 1f);
        public Color ActivatedColor = Color.white;
        public Color DeactivatedColor = Color.white;        
        public float DurationSeconds = 1f;
        public bool UseRealtime = false;

        private Vector3 mInitialScale;
        private Vector3 mTargetScale;

        private Color mInitialColor;
        private Color mTargetColor;

        private SpriteRenderer mRenderer;
        private UpdateTimer mTimer;

        private void Start() {
            mRenderer = this.GetComponent<SpriteRenderer>();

            // Set initial state
            this.transform.localScale = this.DeactivatedScale;
            mInitialScale = this.DeactivatedScale;
            mTargetScale = this.ActivatedScale;
            mRenderer.color = this.DeactivatedColor;

            if (this.UseRealtime) {
                mTimer = new UpdateTimer(Common.TimeScale.Unscaled);
            }
            else {
                mTimer = new UpdateTimer(Common.TimeScale.Scaled);
            }
        }

        private void Update() {
            if (mTimer.Enabled) {
                float lerpTime = mTimer.TimeElapsedNormalized;

                mRenderer.color = Color.Lerp(mInitialColor, mTargetColor, lerpTime);
                this.transform.localScale = Vector3.Lerp(mInitialScale, mTargetScale, lerpTime);
            }
        }

        public override void Activate(object source, CogData data) {
            this.transform.localScale = this.DeactivatedScale;
            mInitialScale = this.DeactivatedScale;
            mTargetScale = this.ActivatedScale;
            mInitialColor = this.DeactivatedColor;
            mTargetColor = this.ActivatedColor;
            mTimer.Start(this.DurationSeconds);
        }

        public override void Deactivate(object source, CogData data) {
            this.transform.localScale = this.ActivatedScale;
            mInitialScale = this.ActivatedScale;
            mTargetScale = this.DeactivatedScale;
            mInitialColor = this.ActivatedColor;
            mTargetColor = this.DeactivatedColor;
            mTimer.Start(this.DurationSeconds);
        }

    }
}
