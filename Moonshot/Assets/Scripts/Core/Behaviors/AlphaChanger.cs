using Core.Pooling;
using Core.Common;
using Core.Utility;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Behaviors {
    /// <summary>
    /// Adds a timer to the object to change it's color over a duration. Optionally, you can destroy the object
    /// after it has finished changing colors.
    /// </summary>
    [Serializable]
    public class AlphaChanger : MonoBehaviour {
        public Graphic TargetSprite;                // Anything with a color component
        public float PreChangeSeconds = 2f;
        public bool DestroyAfterChange = false;
        public float ChangeDurationSeconds = 1f;
        public Color TargetColor = Color.white;

        private bool mStillAlive;
        private UpdateTimer mTimer;
        private Color mInitialColor;

        private void Awake() {
            mTimer = new UpdateTimer(TimeScale.Scaled);
            mInitialColor = this.TargetSprite.color;
        }

        private void Start() {
            if (this.PreChangeSeconds > 0) {
                mStillAlive = true;
                mTimer.Start(this.PreChangeSeconds);
            }
            else {                
                mStillAlive = false;
                mTimer.Start(this.ChangeDurationSeconds);
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
                mTimer.Start(this.ChangeDurationSeconds);
                mStillAlive = false;                
            }
        }

        private void DoDying() {
            this.TargetSprite.color = Color.Lerp(mInitialColor, this.TargetColor, mTimer.TimeElapsedNormalized);

            if (this.mTimer.TimeReached) {
                if (this.DestroyAfterChange) {
                    DestroyObject();
                }
                else {
                    // Disable this component if we don't want to kill it
                    this.enabled = false;
                }
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
