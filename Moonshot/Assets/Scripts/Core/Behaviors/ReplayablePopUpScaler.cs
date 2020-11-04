using Core.Common;
using Core.Utility;
using UnityEngine;

namespace Core.Behaviors {
    public class ReplayablePopUpScaler : MonoBehaviour {
        private const float MaximumAngle = 90;
        private const float TargetAngle = 180f;

        private enum State {
            Idle,
            Growing,
            Shrinking
        }

        public float HalfDurationSeconds;
        public float OverScaleFactor = 1.2f;

        private Vector3 mBaseScale;
        private TrigWaveGenerator mWavePattern;
        private State mState;
        private float mSignX;
        private float mSignY;

        public void Awake() {
            ResetState();
        }

        public void Pop() {
            GoToGrowing();
        }

        public void ResetState() {
            mWavePattern = new TrigWaveGenerator(TimeScale.Scaled);
            float signX = this.transform.localScale.x >= 0f ? 1f : -1f;
            float signY = this.transform.localScale.x >= 0f ? 1f : -1f;

            mBaseScale = this.transform.localScale;

            mSignX = signX;
            mSignY = signY;
        }

        private void Update() {
            switch (this.mState) {
                case State.Growing:
                    DoGrowing();
                    break;

                case State.Shrinking:
                    DoShrinking();
                    break;
            }
        }

        private void ProcessWaveScaling() {
            mWavePattern.Update();

            if (!this.mWavePattern.Finished) {
                float scalar = this.OverScaleFactor * mWavePattern.CurrentValue;
                this.transform.localScale = new Vector3(mBaseScale.x + scalar * mSignX,
                                                        mBaseScale.y + scalar * mSignY,
                                                        1f);
            }
        }

        private void GoToIdle() {
            this.mState = State.Idle;
            this.transform.localScale = Vector3.one;
        }

        private void GoToGrowing() {
            mState = State.Growing;
            this.transform.localScale = mBaseScale;
            
            mWavePattern.Start(TrigWaveGenerator.WaveType.Sine, this.HalfDurationSeconds, 0f, MaximumAngle);
        }

        private void DoGrowing() {
            ProcessWaveScaling();

            if (this.mWavePattern.Finished) {
                GoToShrinking();
            }
        }

        private void GoToShrinking() {
            mState = State.Shrinking;
            mWavePattern.Start(TrigWaveGenerator.WaveType.Sine, this.HalfDurationSeconds, MaximumAngle, TargetAngle);
        }

        private void DoShrinking() {
            ProcessWaveScaling();

            if (this.mWavePattern.Finished) {
                GoToIdle();
            }
        }
    }
}
