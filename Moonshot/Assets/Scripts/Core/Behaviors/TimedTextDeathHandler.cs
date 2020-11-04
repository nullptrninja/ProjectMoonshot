using Core.Pooling;
using Core.Common;
using Core.Utility;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Behaviors {
    /// <summary>
    /// Adds a timer to the text object to allow for a timed destruction.
    /// </summary>
    [Serializable]    
    public class TimedTextDeathHandler : MonoBehaviour {
        public Text TextComponent;
        public float TimeToLiveSeconds = 3f;
        public float DeathDuration = 2f;
        public Color TargetColor = Color.white;

        private bool mStillAlive;
        private UpdateTimer mTimer;
        private Color mInitialColor;

        private void Awake() {
            mTimer = new UpdateTimer(TimeScale.Scaled);
            mInitialColor = this.TextComponent.color;
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
            this.TextComponent.color = Color.Lerp(mInitialColor, this.TargetColor, mTimer.TimeElapsedNormalized);

            if (mTimer.TimeReached) {
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
