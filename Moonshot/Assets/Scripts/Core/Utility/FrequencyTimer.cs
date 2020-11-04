using System;
using UnityEngine;
using Core.Common;

namespace Core.Utility {
    /// <summary>
    /// Creates a timer that notifies you when a particular time interval has elapsed.
    /// </summary>
    public class FrequencyTimer {
        public delegate void FrequencyDelegate();
        public event FrequencyDelegate Tick;

        private TimeScale mTimeScale;
        private float mElapsedInterval;
        private float mTargetInterval;
        private bool mEnabled;
        
        /// <summary>
        /// Gets whether the timer has started or not.
        /// </summary>
        public bool Enabled {
            get { return mEnabled; }
        }

        /// <summary>
        /// Initializes a new instance of the FrequencyTimer class.
        /// <param name="scale">Specifies which time scale to use</param>
        /// </summary>
        public FrequencyTimer(TimeScale scale) {
            mTimeScale = scale;
            mEnabled = false;
            Stop();
        }

        /// <summary>
        /// Starts the timer with the specified frequency.
        /// </summary>
        /// <param name="frequency">The frequency in seconds</param>
        public void Start(float frequency) {
            if (frequency <= 0f) {
                throw new ArgumentException("Frequency cannot be less-than or equal to 0");
            }

            mEnabled = true;
            mTargetInterval = frequency;
            mElapsedInterval = 0f;
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public void Stop() {
            mEnabled = false;
            mElapsedInterval = 0f;
            mTargetInterval = 0f;
        }

        /// <summary>
        /// Call this each frame to maintain timer functionality.
        /// </summary>
        public void Update() {
            if (mEnabled) {
                mElapsedInterval += (mTimeScale == TimeScale.Scaled ? Time.deltaTime : Time.unscaledDeltaTime);

                if (mElapsedInterval >= mTargetInterval) {
                    mElapsedInterval -= mTargetInterval;

                    if (this.Tick != null) {
                        this.Tick();
                    }
                }
            }
        }
    }
}
