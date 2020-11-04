using Core.Common;
using Core.Utility;
using System;
using System.Collections;
using UnityEngine;

namespace Core.Behaviors {
    /// <summary>
    /// Modifies the host object's local scaling to perform a scaling from an initial scale to a target scale with an
    /// additional over-scale effect (like a rubber band). If IsShrinking is set, then this will behave in reverse -
    /// which will over-scale the object before shrinking it down to the target scalar.
    /// This component will also attempt to preserve the object's initial scalar sign as well but some adjustment
    /// may be needed in the InitialScale if something doesn't look right.
    /// The intended usage of this is for scaling in one direction (but can be flipped around by toggling IsShrinking-
    /// and calling ResetState manually); it will work best in that manner. If you want to leave the component enabled
    /// but not trigger automatically on start, turn on ManualActivationOnly and in code, call Begin() to trigger.
    /// </summary>
    public class PopUpScaler : MonoBehaviour {
        private const float MaximumAngle = 90;
        private const float TargetAngle = 120f;

        public bool IsFinished => mState == State.Idle;

        private enum State {
            Idle,
            Growing,
            Shrinking,
            GrowingReverse,
            ShrinkingReverse
        }

        public Action Finished;

        public float SectionDuration = 0.3f;
        public float TargetScalar = 1f;
        public float OverScaleFactor = 1.2f;
        public Vector3 InitialScale = new Vector3(1f, 1f, 1f);
        public bool IsShrinking = false;
        public bool ManualActivationOnly = false;
        public bool KillOnFinish = false;
        public bool UseRandomStartDelay = false;        
        public float MinRandomStartDelay = 0f;
        public float MaxRandomStartDelay = 0.15f;        

        private Transform mHost;
        private TrigWaveGenerator mWavePattern;
        private State mState;        
        private float mSignX;
        private float mSignY;
        private float mMaximumScalar;

        public void Awake() {
            mHost = GetComponent<Transform>();
            mWavePattern = new TrigWaveGenerator(TimeScale.Scaled);
            ResetState();
        }

        public void ResetState() {            
            float signX = MiscUtils.GetSign(mHost.localScale.x);
            float signY = MiscUtils.GetSign(mHost.localScale.y);

            if (!this.IsShrinking) {
                //this.mMaximumScalar = this.TargetScalar / Mathf.Sin(TargetAngle);
                mMaximumScalar = this.TargetScalar * OverScaleFactor;
            }
            else {
                // For reversal situations, divide the two axis of the initial scale by 3 to derive a maximum scalar
                mMaximumScalar = ((this.InitialScale.x + this.InitialScale.y) / 3f) / Mathf.Sin(TargetAngle);
            }


            this.InitialScale = new Vector3(signX * this.InitialScale.x,
                                            signY * this.InitialScale.y,
                                            this.InitialScale.z);

            mSignX = signX;
            mSignY = signY;
            mState = State.Idle;

            mHost.transform.localScale = this.InitialScale;
        }

        public void Start() {
            if (!this.ManualActivationOnly) {
                Begin();
            }
        }

        public void Begin() {
            mState = State.Idle;        // We don't use GoToIdle here because that signifies we've finished transforming.
                                        // Instead we set this so that we don't retrip the timing while things are
                                        // processing the randomized start delay

            Action targetState = GoToGrowing;
            if (this.IsShrinking) {
                targetState = GoToGrowingReverse;
            }

            var minDelay = this.UseRandomStartDelay ? this.MinRandomStartDelay : 0f;
            var maxDelay = this.UseRandomStartDelay ? this.MaxRandomStartDelay : 0f;
            StartCoroutine(DelayedStartRoutine(minDelay, maxDelay, targetState));
        }

        private IEnumerator DelayedStartRoutine(float minDelaySeconds, float maxDelaySeconds, Action stateTransitionTo ) {
            var secs = UnityEngine.Random.Range(minDelaySeconds, maxDelaySeconds);
            yield return new WaitForSeconds(secs);
            stateTransitionTo();
        }

        // Update is called once per frame
        private void Update() {
            switch (this.mState) {
                case State.Growing:
                    DoGrowing();
                    break;

                case State.Shrinking:
                    DoShrinking();
                    break;

                case State.GrowingReverse:
                    DoGrowingReverse();
                    break;

                case State.ShrinkingReverse:
                    DoShrinkingReverse();
                    break;
            }
        }

        private void ProcessWaveScaling() {
            mWavePattern.Update();

            if (!this.mWavePattern.Finished) {
                var scalar = mMaximumScalar * mWavePattern.CurrentValue;
                mHost.localScale = new Vector3(scalar * mSignX,
                                               scalar * mSignY,
                                               1f);
            }
        }

        private void FireFinishedEvent() {
            this.Finished?.Invoke();
        }

        private void GoToIdle() {
            this.mState = State.Idle;
            this.enabled = false;

            mHost.localScale = new Vector3(this.TargetScalar, this.TargetScalar, this.TargetScalar);

            FireFinishedEvent();

            if (this.KillOnFinish) {
                GameObject.Destroy(this);
            }
        }

        private void GoToGrowing() {
            mState = State.Growing;
            mHost.localScale = this.InitialScale;

            mWavePattern.Start(TrigWaveGenerator.WaveType.Sine, this.SectionDuration, 0f, MaximumAngle);
        }

        private void DoGrowing() {
            ProcessWaveScaling();
            
            if (this.mWavePattern.Finished) {
                GoToShrinking();
            }
        }

        private void GoToShrinking() {
            mState = State.Shrinking;
            mWavePattern.Start(TrigWaveGenerator.WaveType.Sine, this.SectionDuration, MaximumAngle, TargetAngle);
        }

        private void DoShrinking() {
            ProcessWaveScaling();

            if (this.mWavePattern.Finished) {
                GoToIdle();
            }
        }

        private void GoToGrowingReverse() {
            mState = State.GrowingReverse;      

            mWavePattern.Start(TrigWaveGenerator.WaveType.Sine, this.SectionDuration, TargetAngle, MaximumAngle);
        }

        private void DoGrowingReverse() {
            ProcessWaveScaling();

            if (this.mWavePattern.Finished) {
                GoToShrinkingReverse();
            }
        }

        private void GoToShrinkingReverse() {
            mState = State.ShrinkingReverse;
            mWavePattern.Start(TrigWaveGenerator.WaveType.Sine, this.SectionDuration, MaximumAngle, 0.05f);
        }

        private void DoShrinkingReverse() {
            ProcessWaveScaling();

            if (mWavePattern.Finished) {
                GoToIdle();
            }
        }
    }
}
