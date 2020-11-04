using Core.Utility;
using UnityEngine;

namespace Core.Behaviors {
    public class Rotator : MonoBehaviour {
        public Vector3 UnitAxis = new Vector3(0f, 0f, 1f);
        public Vector3 TargetAngle = new Vector3(0f, 0f, 0f);
        public float CycleLengthSeconds = 1f;
        public int LoopCount = 0;

        private bool mEnabled;
        private int mCurrentLoop;
        private UpdateTimer mCycleTimer;
        private Vector3 mStartAngle;

        private void Start() {
            mEnabled = true;
            mCurrentLoop = this.LoopCount;
            mCycleTimer = new UpdateTimer(Common.TimeScale.Scaled);
            mStartAngle = this.transform.localEulerAngles;

            mCycleTimer.Start(this.CycleLengthSeconds);
        }

        private void Update() {
            if (mEnabled) {
                mCycleTimer.Update();
                this.transform.localEulerAngles = Vector3.Lerp(mStartAngle, this.TargetAngle, mCycleTimer.TimeElapsedNormalized);

                if (mCycleTimer.TimeReached) {
                    if (mCurrentLoop > 0) {
                        // Still some loops left
                        mCurrentLoop--;
                        mCycleTimer.Start(this.CycleLengthSeconds);
                        this.transform.localEulerAngles = mStartAngle;
                    }
                    else if (mCurrentLoop == 0) {
                        mEnabled = false;                        
                    }
                    else {
                        // Otherwise, less-than-zero is infinite loop; just reset the angle
                        this.transform.localEulerAngles = mStartAngle;
                        mCycleTimer.Start(this.CycleLengthSeconds);
                    }
                }
            }
        }

    }
}
