using Core.Common;
using UnityEngine;

namespace Core.Utility {
    /// <summary>
    /// Creates a timer that easily keeps track of elapsed time. This is useful for timing when
    /// used in an update-cycle structure.
    /// </summary>
    public class UpdateTimer {
        private TimeScale mTimeScale;
        private float mStartTime;
        private float mCurrentTime;
        private float mTargetTime;
        private bool mEnabled;

        /// <summary>
        /// Gets whether the specified duration has been reached. If the timer has not started,
        /// a false is returned.
        /// </summary>
        public bool TimeReached {
            get { return mEnabled && mCurrentTime >= mTargetTime; }
        }

        /// <summary>
        /// Gets the amount of time that has elapsed since the timer start.
        /// </summary>
        public float TimeElapsed {
            get {
                return mEnabled ? this.CurrentTime - mStartTime : 0f;
            }
        }

        /// <summary>
        /// Gets the amount of time that has elapsed since the timer start as a normalized value
        /// between 0 and 1. If the elapsed time has exceeded the specified duration, then 1 is returned.
        /// </summary>
        public float TimeElapsedNormalized {
            get {
                if (mEnabled) {
                    float difference = (mTargetTime - mStartTime);
                    if (difference != 0) {
                        return this.CurrentTime <= mTargetTime ? this.TimeElapsed / difference : 1f;
                    }
                    else {
                        return 1f;
                    }
                }
                else {
                    return 0f;
                }
            }
        }

        /// <summary>
        /// Gets whether the timer has started or not.
        /// </summary>
        public bool Enabled {
            get { return mEnabled; }
        }

        /// <summary>
        /// Gets the current elapsed game time based on the configured time scale.
        /// </summary>
        private float CurrentTime {
            get { return mTimeScale == TimeScale.Scaled ? Time.time : Time.unscaledTime; }
        }

        /// <summary>
        /// Initializes a new instance of the UpdateTimer class.
        /// <param name="scale">Specifies which time scale to use</param>
        /// </summary>
        public UpdateTimer(TimeScale scale) {
            mTimeScale = scale;
            Stop();
        }

        /// <summary>
        /// Starts the timer with the specified duration.
        /// </summary>
        /// <param name="durationSeconds">The duration in seconds that the timer should track</param>
        public void Start(float durationSeconds) {
            if (durationSeconds <= 0f) {
                durationSeconds = 0f;
            }

            mEnabled = true;
            mStartTime = mCurrentTime = this.CurrentTime;            
            mTargetTime = mCurrentTime + durationSeconds;
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public void Stop() {
            mEnabled = false;
            mStartTime = 0f;
            mCurrentTime = 0f;
            mTargetTime = float.MaxValue;
        }
        
        /// <summary>
        /// Call this each frame to maintain timer functionality.
        /// </summary>
        public void Update() {
            if (mEnabled && mCurrentTime < mTargetTime) {
                mCurrentTime += (mTimeScale == TimeScale.Scaled ? Time.deltaTime : Time.unscaledDeltaTime);
            }
        }

        public void AddTime(float seconds)
        {
            mTargetTime += seconds;
        }
    }
}
