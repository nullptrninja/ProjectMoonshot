using Core.Utility;
using UnityEngine;

namespace Core.Behaviors {
    /// <summary>
    /// Allows you to easily perform scaling animations with alpha support on any GameObject containing a SpriteRenderer
    /// </summary>
    public class Scalar : MonoBehaviour {

        public Vector3 InitialScale = new Vector3(1f, 1f, 1f);
        public Vector3 TargetScale = new Vector3(1f, 1f, 1f);
        public Color InitialColor = Color.white;
        public Color TargetColor = Color.white;        
        public float DurationSeconds = 1f;
        public int Loops = 0;
        public bool ReverseOnLoop = true;
        public bool UseRealtime = false;

        private SpriteRenderer mRenderer;
        private UpdateTimer mTimer;
        private int mCurrentLoopIteration;
        private bool mReverseMotion;

        private void Start() {
            mCurrentLoopIteration = 0;
            mReverseMotion = false;
            mRenderer = GetComponent<SpriteRenderer>();

            // Set initial state
            this.transform.localScale = this.InitialScale;            
            mRenderer.color = this.InitialColor;

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
                mTimer.Update();
                float lerpTime = mTimer.TimeElapsedNormalized;

                if (!mReverseMotion) {
                    mRenderer.color = Color.Lerp(this.InitialColor, this.TargetColor, lerpTime);
                    this.transform.localScale = Vector3.Lerp(this.InitialScale, this.TargetScale, lerpTime);
                }
                else {
                    mRenderer.color = Color.Lerp(this.TargetColor, this.InitialColor, lerpTime);
                    this.transform.localScale = Vector3.Lerp(this.TargetScale, this.InitialScale, lerpTime);
                }

                if (mTimer.TimeReached) {
                    mTimer.Stop();
                }
            }
            else {
                if (this.Loops == -1 || mCurrentLoopIteration < this.Loops) {
                    mCurrentLoopIteration++;

                    if (this.ReverseOnLoop) {
                        mReverseMotion = !mReverseMotion;
                    }

                    mTimer.Start(this.DurationSeconds);
                }
                else {
                    Destroy(this);
                }
            }
        }

    }
}
