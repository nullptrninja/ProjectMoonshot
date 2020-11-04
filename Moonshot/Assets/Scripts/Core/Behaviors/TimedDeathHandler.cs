using Core.Pooling;
using Core.Common;
using Core.Utility;
using System;
using UnityEngine;

namespace Core.Behaviors {
    /// <summary>
    /// Adds a timer to the object to allow for a timed destruction.
    /// </summary>
    [Serializable]    
    public class TimedDeathHandler : MonoBehaviour {
        public float TimeToLiveSeconds = 0f;
        public float DeathDuration = 1f;
        public bool EnableColorChange = false;
        public Color TargetColor = Color.white;

        private bool mStillAlive;
        private UpdateTimer mTimer;
        private Color mInitialColor;
        private SpriteRenderer mRenderer;

        private void Awake() {
            mTimer = new UpdateTimer(TimeScale.Scaled);

            if (this.EnableColorChange) {
                mRenderer = GetComponent<SpriteRenderer>();
                mInitialColor = mRenderer.color;
            }
        }

        private void Start() {            
            if (this.TimeToLiveSeconds > 0) {
                mStillAlive = true;
                mTimer.Start(this.TimeToLiveSeconds);
            }
            else {
                mStillAlive = false;
                mTimer.Start(this.DeathDuration);
            }
        }

        private void Update() {
            mTimer.Update();

            if (mStillAlive) {
                DoTTL();
            }
            else {
                DoDying();
            }            
        }

        private void DoTTL() {
            if (mTimer.TimeReached) {
                mTimer.Start(this.DeathDuration);
                mStillAlive = false;
            }
        }

        private void DoDying() {
            if (this.EnableColorChange) {
                mRenderer.color = Color.Lerp(mInitialColor, this.TargetColor, mTimer.TimeElapsedNormalized);
            }

            if (this.mTimer.TimeReached) {
                DestroyObject();
            }
        }

        private void DestroyObject() {
            ObjectPoolLink poolLink = GetComponent<ObjectPoolLink>();
            if (poolLink != null && poolLink.HasObjectPool) {
                poolLink.LinkedObjectPool.ReleaseObject(this.gameObject);
            }
            else {
                Destroy(this.gameObject);
            }
        }
    }
}
