using System.Collections;
using UnityEngine;

namespace Core.Behaviors.Functional {
    public class PhysicsForceApplier : MonoBehaviour {
        public Rigidbody Body;
        public float ApplyForcesAfterTime;

        public bool ConstantlyApplyRotation;
        public Vector3 RotationalForce;

        public bool ConstantlyApplyTranslation;
        public Vector3 TranslationForce;

        public void Awake() {
            StartCoroutine(ApplyForceAtTimeRoutine(this.ApplyForcesAfterTime));
        }

        public void Update() {
            if (this.ConstantlyApplyRotation) {
                this.Body.AddTorque(this.RotationalForce, ForceMode.Force);
            }

            if (this.ConstantlyApplyTranslation) {
                this.Body.AddForce(this.TranslationForce, ForceMode.Force);
            }
        }

        private IEnumerator ApplyForceAtTimeRoutine(float timeSeconds) {
            yield return new WaitForSeconds(timeSeconds);

            this.Body.AddTorque(this.RotationalForce, ForceMode.Impulse);
            this.Body.AddForce(this.TranslationForce, ForceMode.Impulse);
        }
    }
}
