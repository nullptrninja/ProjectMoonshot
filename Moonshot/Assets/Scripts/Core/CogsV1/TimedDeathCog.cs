using Core.Common;
using Core.Pooling;
using Core.Utility;
using UnityEngine;

namespace Core.CogsV1 {
    public class TimedDeathCog : CogAdapter {
        public float TimeToLiveSeconds = 0f;
        public float DeathDuration = 1f;
        public bool EnableSpriteColorChange = false;
        public Color TargetColor = Color.white;

        private bool mStillAlive;
        private UpdateTimer mTimer;
        private Color mInitialColor;
        private SpriteRenderer mRenderer;

        private void Awake() {
            mTimer = new UpdateTimer(TimeScale.Scaled);

            if (this.EnableSpriteColorChange) {
                mRenderer = this.GetComponent<SpriteRenderer>();
                mInitialColor = mRenderer.color;
            }
        }

        public override void Activate(object sender, CogData data) {
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
            if (this.EnableSpriteColorChange && mRenderer != null) {
                mRenderer.color = Color.Lerp(mInitialColor, this.TargetColor, mTimer.TimeElapsedNormalized);
            }

            if (mTimer.TimeReached) {
                DestroyObject();
            }
        }

        private void DestroyObject() {
            ObjectPoolLink poolLink = this.GetComponent<ObjectPoolLink>();
            if (poolLink != null && poolLink.HasObjectPool) {
                poolLink.LinkedObjectPool.ReleaseObject(this.gameObject);
            }
            else {
                Destroy(this.gameObject);
            }
        }
    }
}
