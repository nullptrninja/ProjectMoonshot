using System;
using Core.CogsV1;
using Core.Utility;
using UnityEngine;

namespace Core.Behaviors.Functional.Sensors {
    /// <summary>
    /// Provides functionality to determine if an object has stopped moving within a specified time span.
    /// 
    /// TODO: Moving platform support not fully implemented yet
    /// </summary>
    public class MotionDetector : MonoBehaviour {
        public float TimeToSettle = 3f;         // Time in seconds allowed for the object to settle. The longer the time is, the
                                                // longer it will take for this component to fire the "finished" event

        public float ReanchorThreshold = 1f;    // Essentially how "sensitive" the detector is in world units. If the host moves
                                                // more than this configured unit of distance, then we consider the host to still be moving.

        public float CheckIntervalSeconds = 1f;  // How often the detector polls the host for movement data in seconds. Keep this at a reasonable
                                                 // rate to minimize performance impact. This can only be set once and is applied at Awake-time.

        public CogAdapter ActivateOnSettle;     // Optionally triggers a COG everytime this host comes to a stop.

        public bool CogTriggersOnceOnly;        // If true, the linked COG will only ever be triggered once on first settle event.

        private UpdateTimer mTimer;
        private FrequencyTimer mFTimer;
        private Vector3 mAnchorPosition;
        private Vector3 mLastPosition;
        private bool mBodyHasMoved;
        private bool mCogTriggered;

        public bool IsOnMovingPlatform { get; set; }

        public Action OnFinishedCallback { get; set; }

        private void Awake() {
            this.mAnchorPosition = Vector3.zero;
            this.mLastPosition = Vector3.zero;
            this.mBodyHasMoved = true;
            this.mTimer = new UpdateTimer(Common.TimeScale.Scaled);
            this.mFTimer = new FrequencyTimer(Common.TimeScale.Scaled);

            this.mFTimer.Tick += mFTimer_Tick;
            this.mFTimer.Start(this.CheckIntervalSeconds);

            this.mAnchorPosition = this.transform.position;
        }

        private void mFTimer_Tick() {
            // Every tick we check if we've moved a significant amount based on our set threshold. If we have,
            // then update the anchor point.

            // Check the delta between last and anchor
            float sqrLenDelta = Math.Abs((mAnchorPosition - mLastPosition).sqrMagnitude);

            if (sqrLenDelta > this.ReanchorThreshold) {
                mAnchorPosition = mLastPosition;
                mBodyHasMoved = true;
            }
            else {
                mBodyHasMoved = false;
            }
        }

        public void Update() {
            mFTimer.Update();

            // If we've detected that we have not moved much relative to the last anchor point,
            // start a timer. If the timer finishes then we will assume the host hasn't moved much
            // in the last X seconds. If we detect a movement, stop and reset the timer.
            // ALTERNATE: If host is on a moving platform, we can start counting as well but that flag
            // must be set externally by the platform itself.
            if (!mBodyHasMoved || this.IsOnMovingPlatform) {
                if (!mTimer.Enabled) {
                    mTimer.Start(this.TimeToSettle);
                }
                else {
                    mTimer.Update();

                    if (mTimer.TimeReached) {
                        this.OnFinishedCallback?.Invoke();
                        
                        if (this.ActivateOnSettle != null && ((this.CogTriggersOnceOnly && !mCogTriggered) || (!this.CogTriggersOnceOnly))) {
                            this.ActivateOnSettle.Activate(this, null);

                            mCogTriggered = true;
                        }
                    }
                }
            }
            else if (mTimer.Enabled) {
                mTimer.Stop();
            }

            // Record the last position
            mLastPosition = this.transform.position;
        }
    }
}
