using Core.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Behaviors.UI {
    /// <summary>
    /// Allows you to easily perform scaling animations with alpha support on any GameObject containing a Unity UI Image
    /// </summary>
    public class ScalarImageUI : MonoBehaviour {

        public Vector3 InitialScale = new Vector3(1f, 1f, 1f);
        public Vector3 TargetScale = new Vector3(1f, 1f, 1f);
        public Color InitialColor = Color.white;
        public Color TargetColor = Color.white;
        public float DurationSeconds = 1f;
        public bool UseRealtime = false;

        private Image mImage;
        private UpdateTimer mTimer;

        private void Start() {
            mImage = GetComponent<Image>();

            // Set initial state
            this.transform.localScale = this.InitialScale;
            mImage.color = this.InitialColor;

            if (this.UseRealtime) {
                mTimer = new UpdateTimer(Common.TimeScale.Unscaled);
            }
            else {
                mTimer = new UpdateTimer(Common.TimeScale.Scaled);
            }

            mTimer.Start(this.DurationSeconds);
        }

        private void Update() {
            if (mTimer.Enabled) {
                float lerpTime = mTimer.TimeElapsedNormalized;

                mImage.color = Color.Lerp(this.InitialColor, this.TargetColor, lerpTime);
                this.transform.localScale = Vector3.Lerp(this.InitialScale, this.TargetScale, lerpTime);
            }
        }

    }
}
