using UnityEngine;

namespace Core.Behaviors {
    public class Translator : MonoBehaviour {        
        public Vector3 TranslateDistance = Vector2.zero;
        public float DurationSeconds = 0f;
        public bool UseSlerp = true;
        public bool SelfDestructHost = false;

        private Vector3 mTargetPosition;
        private Vector3 mStartPosition;
        private float mElapsedTime;
        private float mElapsedTimeNormalized;
        private bool mEnabled;

        private void Start() {
            mTargetPosition = this.transform.position + this.TranslateDistance;
            mStartPosition = this.transform.position;
            mElapsedTime = 0f;
            mElapsedTimeNormalized = 0f;
            mEnabled = true;
        }

        private void Update() {
            if (mEnabled) {
                if (this.UseSlerp) {
                    this.transform.position = Vector3.Slerp(mStartPosition, mTargetPosition, mElapsedTimeNormalized);
                }
                else {
                    this.transform.position = Vector3.Lerp(mStartPosition, mTargetPosition, mElapsedTimeNormalized);
                }

                mElapsedTime += Time.deltaTime;
                mElapsedTimeNormalized = mElapsedTime / this.DurationSeconds;

                if (mElapsedTimeNormalized > 1.0f) {
                    mEnabled = false;
                }
            }
            else if (this.SelfDestructHost) {
                Destroy(this.gameObject);
            }
        }
    }
}
