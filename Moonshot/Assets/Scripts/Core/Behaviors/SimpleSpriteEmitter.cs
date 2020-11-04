using Core.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Behaviors {
    /// <summary>
    /// Emits sprites according to the specified parameters.
    /// Note, this does not support Object Pooling yet.
    /// </summary>
    public class SimpleSpriteEmitter : MonoBehaviour {

        public bool StartOnAwake = true;
        public bool ApplyScaleFromHost = false;
        public float StartDelay = 0f;       // Delay before start of simulation from when Begin() is called
        public float Duration = 5f;         // In seconds. -1 for infinite
        public float Frequency = 1f;        // How many seconds between particle fires
        public float MinAngle = 0f;
        public float MaxAngle = 359f;
        public float MaxVelocity = 10f;
        public float MinVelocity = 1f;
        public float ParticleTTL = 5f;      // Particle time to live (seconds)
        public string ParticleName = null;

        private UpdateTimer mDurationTimer;
        private FrequencyTimer mFireTimer;
        private bool mStartedEmitting;

        public bool Started {
            get { return mStartedEmitting; }
        }

        public void Stop() {
            mStartedEmitting = false;
            mFireTimer.Stop();
            mDurationTimer.Stop();
        }

        /// <summary>
        /// Begins the emitting process
        /// </summary>
        public void Begin() {
            if (string.IsNullOrEmpty(this.ParticleName)) {
                throw new ArgumentException("Particle name cannot be empty");
            }

            if (this.Duration != -1f) {
                mDurationTimer.Start(this.Duration);
            }

            mFireTimer.Start(this.Frequency);
            mStartedEmitting = true;
        }

        private void Start() {
            if (this.Frequency <= 0f) {
                throw new ArgumentOutOfRangeException("Frequency cannot be less than or equal to zero");
            }

            mStartedEmitting = false;
            mDurationTimer = new UpdateTimer(Common.TimeScale.Scaled);
            mFireTimer = new FrequencyTimer(Common.TimeScale.Scaled);
            mFireTimer.Tick += this.FireParticle;

            if (this.StartOnAwake) {
                Begin();
            }
        }

        private void Update() {
            if (mStartedEmitting) {
                mDurationTimer.Update();
                mFireTimer.Update();

                if (mDurationTimer.TimeReached && this.Duration != -1f) {
                    Stop();
                }
            }            
        }
        
        private void FireParticle() {
            //Debug.Log("Fire!");            
            Vector2 velocityVector = VectorUtility.GetRandomAngledVector(this.MinAngle, this.MaxAngle) * UnityEngine.Random.Range(this.MinVelocity, this.MaxVelocity);
            GameObject g = GameObject.Instantiate(Resources.Load<GameObject>(this.ParticleName));
            g.GetComponent<Rigidbody2D>().velocity = velocityVector;
            g.transform.position = this.gameObject.transform.position;

            if (this.ApplyScaleFromHost) {
                g.transform.localScale = this.transform.localScale;
            }

            // Set the TTL. All particles must support the TimeDeathHandler behavior
            g.GetComponent<TimedDeathHandler>().DeathDuration = this.ParticleTTL;
        }
    }
}
