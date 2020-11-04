using Core.Utility;
using System;
using UnityEngine;

namespace Core.Behaviors {
    /// <summary>
    /// Allows the attached GameObject to exhibit a patterned floating behavior based on trigonometric functions
    /// </summary>
    public class Floater: MonoBehaviour {

        /// <summary>
        /// Specifies which GameObject to follow. THis is optional.
        /// </summary>
        public GameObject FollowHost = null;

        /// <summary>
        /// The amount of world units to move in each direction
        /// </summary>
        public Vector2 FloatingRange = Vector2.zero;

        /// <summary>
        /// The duration in each direction at which this object moves (in seconds) before
        /// changing direction.
        /// </summary>
        public Vector2 FloatingDuration = Vector2.zero;

        /// <summary>
        /// Optional offset for positioning.
        /// </summary>
        public Vector2 PositionOffset = Vector2.zero;

        private TrigWaveGenerator mXTimer;
        private TrigWaveGenerator mYTimer;
        private bool mXState;
        private bool mYState;
        private bool mHasHost;

        private void Start() {
            if (this.FloatingRange.x < 0f || this.FloatingRange.y < 0f) {
                throw new ArgumentOutOfRangeException("Floating Range cannot be negative");
            }

            if (this.FloatingDuration.x < 0f || this.FloatingDuration.y < 0f) {
                throw new ArgumentOutOfRangeException("Floating Duration cannot be negative");
            }

            mXTimer = new TrigWaveGenerator(Common.TimeScale.Scaled);
            mYTimer = new TrigWaveGenerator(Common.TimeScale.Scaled);

            mXState = false;
            mYState = false;
            mHasHost = this.FollowHost != null;

            StartXTimer();
            StartYTimer();
        }

        private void Update() {
            // If we had a host, and now we don't; kill this object.
            if (mHasHost && this.FollowHost == null) {
                Destroy(this.gameObject);
            }

            float moveX = 0f;
            float moveY = 0f;

            if (this.FloatingRange.x > 0f) {
                mXTimer.Update();

                if (mXTimer.Finished) {
                    mXState = !mXState;
                    StartXTimer();
                }

                moveX = (mXTimer.CurrentValue * this.FloatingRange.x) * (mXState ? 1f : -1f) * Time.timeScale;
            }

            if (this.FloatingRange.y > 0f) {
                mYTimer.Update();

                if (mYTimer.Finished) {
                    mYState = !mYState;
                    StartYTimer();
                }

                moveY = (mYTimer.CurrentValue * this.FloatingRange.y) * (mYState ? 1f : -1f) * Time.timeScale;
            }

            Vector3 delta = new Vector3(moveX + this.PositionOffset.x, moveY + this.PositionOffset.y, 0f);

            if (this.FollowHost != null) {
                this.transform.position = this.FollowHost.transform.position + delta;
            }
            else {
                this.transform.Translate(delta);
            }
        }

        private void StartXTimer() {
            if (this.FloatingRange.x > 0f) {
                mXTimer.Start(TrigWaveGenerator.WaveType.Cosine, this.FloatingDuration.x, 0f, 180f);
            }
        }

        private void StartYTimer() {
            if (this.FloatingRange.y > 0f) {
                mYTimer.Start(TrigWaveGenerator.WaveType.Sine, this.FloatingDuration.y, 0f, 180f);
            }
        }
    }
}