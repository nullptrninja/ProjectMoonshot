using Core.CogsV1;
using UnityEngine;

namespace Core.CogsV1 {
    /// <summary>
    /// SpriteBagCog allows a randomly chosen sprite to appear on the host's sprite renderer everytime
    /// this COG is activated. There are additional options to facilitate a few usage scenarios (such as
    /// muzzle flashes).
    /// </summary>
    public class SpriteBagCog : CogAdapter {
        public SpriteRenderer HostSpriteRenderer;
        public Sprite[] PossibleSprites;

        // Options
        public float DelayBeforeAutoHideSeconds = -1.0f;       // If >= 0f, how many millis before the sprite automatically goes away
        
        // State
        private bool mIsShowingSprite;
        private float mElapsedTimeSeconds;
        private bool mEnableAutoHide;

        public void Start() {
            mIsShowingSprite = false;
            mElapsedTimeSeconds = 0.0f;
            mEnableAutoHide = this.DelayBeforeAutoHideSeconds >= 0.0f;
        }
                
        public override void Activate(object source, CogData data) {
            if (!mIsShowingSprite) {
                this.HostSpriteRenderer.sprite = GetRandomSprite();
                mElapsedTimeSeconds = 0.0f;
                mIsShowingSprite = true;
                
                // Here we only activate linked COGs if we've displayed a sprite.
                base.Activate(source, data);
            }
        }

        public override void Deactivate(object source, CogData data) {
            if (mIsShowingSprite) {
                this.HostSpriteRenderer.sprite = null;
                mIsShowingSprite = false;
                base.Deactivate(source, data);
            }
        }

        public void Update() {
            if (mIsShowingSprite && mEnableAutoHide) {
                mElapsedTimeSeconds += Time.deltaTime;

                if (mElapsedTimeSeconds >= this.DelayBeforeAutoHideSeconds) {
                    this.HostSpriteRenderer.sprite = null;
                    mIsShowingSprite = false;
                }
            }
        }

        private Sprite GetRandomSprite() {
            int index = UnityEngine.Random.Range(0, this.PossibleSprites.Length);
            return this.PossibleSprites[index];
        }
    }
}
