using Core.Common;
using System;
using UnityEngine;

namespace Core.Utility {
    /// <summary>
    /// Generates values based on trigonometric functions over a specified time period and value range.
    /// </summary>
    public class TrigWaveGenerator {
        private const float PIover180 = (float)Math.PI / 180f;

        public enum WaveType    {
            Sine,
            Cosine
        }

        private WaveType mWaveType;
        private TimeScale mTimeScale;
        private float mDuration;        
        private float mElapsedTime;
        private float mStart;
        private float mEnd;
        private bool mContinuous;

        /// <summary>
        /// Gets whether this generator has reached the end of its duration
        /// </summary>
        public bool Finished {
            get { return mElapsedTime >= mDuration; }
        }

        /// <summary>
        /// Gets the current function value based on the current time position in radians
        /// </summary>
        public float CurrentValue {
            get { return CalculateWaveValue(); }
        }

        /// <summary>
        /// Gets the current delta game time based on the configured time scale.
        /// </summary>
        private float CurrentDelta {
            get { return mTimeScale == TimeScale.Scaled ? Time.deltaTime : Time.unscaledDeltaTime; }
        }
        
        /// <summary>
        /// Creates a new instance of the WaveGenerator class
        /// <param name="scale">Specifies which time scale to use</param>
        /// </summary>
        public TrigWaveGenerator(TimeScale scale, bool isContinuous = false)  {
            mTimeScale = scale;
            mContinuous = isContinuous;
        }

        /// <summary>
        /// Configures the wave generator
        /// </summary>
        /// <param name="waveType">Type of wave function to follow</param>
        /// <param name="durationSeconds">How long the generator should last; this controls the granularity of the values generated. Cannot be 0</param>
        /// <param name="startAngle">Start angle value to begin at (inclusive), in degrees</param>
        /// <param name="endAngle">End angle value to stop at (inclusive), in degrees</param>
        /// <param name="startAtTimePosition">Value between 0 and 1 (inc) to jump to immediately</param>
        public void Start(WaveType waveType, float durationSeconds, float startAngle, float endAngle, float startAtTimePosition = 0f) {
            if (durationSeconds <= 0f) {
                throw new ArgumentException("Cannot have zero or less duration");
            }

            mWaveType = waveType;
            mDuration = durationSeconds;
            mStart = startAngle * PIover180;
            mEnd = endAngle * PIover180;
            mElapsedTime = startAtTimePosition;
        }

        /// <summary>
        /// Updates the generator. This must be called each cycle to maintain functionality
        /// </summary>
        public void Update() {
            if (mContinuous || !this.Finished) {
                mElapsedTime += this.CurrentDelta;
            }
        }

        /// <summary>
        /// Calculates an arbitrary value using the currently selected trig model
        /// </summary>
        /// <param name="angle">Angle to calculate in degrees</param>
        /// <returns>Wave value of the angle. Float.MinValue if invalid.</returns>
        public float CalculateWaveValueByAngle(float angle)    {
            switch (mWaveType) {
                case WaveType.Sine:
                    return (float)Math.Sin(angle);

                case WaveType.Cosine:
                    return (float)Math.Cos(angle);
            }

            return float.MinValue;
        }

        private float CalculateWaveValue() {
            if (mContinuous && this.Finished) {
                mElapsedTime = 0f;
            }

            float position = mElapsedTime / mDuration;
            float currentAngleRads = ((mEnd - mStart) * position) + mStart;

            switch (mWaveType) {
                case WaveType.Sine:
                    return (float)Math.Sin(currentAngleRads);

                case WaveType.Cosine:
                    return (float)Math.Cos(currentAngleRads);

                default:
                    return 0f;
            }
        }
    }
}
