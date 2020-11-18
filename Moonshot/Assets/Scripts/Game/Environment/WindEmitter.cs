using Assets.Scripts.Game.Common;
using Core.Utility;
using UnityEngine;

namespace Assets.Scripts.Game.Environment {
    public class WindEmitter : MonoBehaviour {
        private static readonly Color BaseColor = new Color(0.5f, 0.5f, 1f, 1f);
        private static readonly Color WindForceVectorColor = new Color(1f, 0.15f, 0.15f, 1f);

        public float WindSpeedImpulse;
        public CompassDirection WindDirection;
        public Rigidbody TargetBody;
        public float EmitterSocialDistance = 15f;        
        public float ImpulseDurationSeconds;
        public float WindVectorRandomRadius = 15f;
        public LayerMask LayerMask;

        private Vector3 mWindVector;
        private float mHitTestDistance;
        private float mElapsedTime;
        private TrigWaveGenerator mWindModel;

        public void Awake() {
            mWindModel = new TrigWaveGenerator(Core.Common.TimeScale.Scaled, true);
        }

        public void Start() {
            mHitTestDistance = this.EmitterSocialDistance * 2f;            
            mWindModel.Start(TrigWaveGenerator.WaveType.Sine, this.ImpulseDurationSeconds, 0f, 180f);

            SocialDistanceFromTarget();
            mWindVector = GetWindVector();
        }

        public void Update() {
            mWindModel.Update();
        }

        public void FixedUpdate() {
            if (this.TargetBody != null) {
                SocialDistanceFromTarget();

                mElapsedTime += Time.deltaTime;
                if (mElapsedTime >= this.ImpulseDurationSeconds) {
                    mWindVector = GetWindVector();
                    mElapsedTime = 0f;
                    //Debug.Log($"Switched wind dir to: {mWindVector}");
                }
    
                // Apply wind from ray cast.                
                if (TestWindRayIntersect(mWindVector, out var hit)) {
                    var scaledWindVect = mWindVector * mWindModel.CurrentValue;
                    this.TargetBody.AddForceAtPosition(scaledWindVect, hit.point);
                }
            }
        }

        private bool TestWindRayIntersect(Vector3 testWindVector, out RaycastHit hit) {
            return Physics.Raycast(this.transform.position, testWindVector, out hit, mHitTestDistance, this.LayerMask, QueryTriggerInteraction.Ignore);
        }

        private void SocialDistanceFromTarget() {            
            var targetPosition = this.TargetBody.position;
            var windSpeedInvRotation = CompassUtility.WindDirToInverseAngle(this.WindDirection);
            var sociallyDistancedPosition = targetPosition + (Vector3.forward * this.EmitterSocialDistance).RotateXZPlanar(windSpeedInvRotation);
            this.transform.position = sociallyDistancedPosition;
        }

        private Vector3 GetWindVector() {
            var xyRandomRot = Random.Range(0f, this.WindVectorRandomRadius) * MiscUtils.RandomSign();
            var windUnitVect = VectorUtility.GetPointingVector3(this.transform, this.TargetBody.transform).RotateXZPlanar(xyRandomRot) * this.WindSpeedImpulse;
            return windUnitVect;
        }

#if UNITY_EDITOR_WIN
        public void OnDrawGizmos() {
            Gizmos.color = BaseColor;
            Gizmos.DrawWireSphere(this.transform.position, 1f);

            Gizmos.color = WindForceVectorColor;
            var windRay = mWindVector;
            Gizmos.DrawRay(this.transform.position, windRay);
        }
#endif
    }
}
