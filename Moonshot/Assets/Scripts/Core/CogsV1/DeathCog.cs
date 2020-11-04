using Core.Common;
using Core.Pooling;
using Core.Utility;
using UnityEngine;

namespace Core.CogsV1 {
    public class DeathCog : CogAdapter {
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
                mRenderer = GetComponent<SpriteRenderer>();
                mInitialColor = mRenderer.color;
            }
        }

        public override void Activate(object sender, CogData data) {            
            mStillAlive = false;
            mTimer.Start(this.DeathDuration);
        }

        private void Update() {
            if (!mStillAlive) {
                mTimer.Update();
                DoDying();
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
