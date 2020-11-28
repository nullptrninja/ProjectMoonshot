using System;
using Core.Utility;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Game.Spacecraft {
    public abstract class BaseRocketMotor : MonoBehaviour {
        private static readonly Color BaseColor = new Color(0.5f, 0.5f, 1f, 1f);

        public Rigidbody ActOnBody;                         // Which body this motor will apply forces on. This can be a local body that's part of a composite object        
        public float ThrustDistance;                        // The height of the affect-cone that the motor will push against. The higher the value, the further the rocket's forces will reach
        public float PeakThrustForce;                       // Maximum thrust force this motor can apply in Newtons        
        public float ThrustAccelerationPerSec;              // All motors have a linear acceleration profile; this controls how quickly we ramp up the PeakThrustRaysPerSec over time. Scales with throttle
        public float ThrottleDeadZone = 0.05f;              // Minimum amount of throttle before we fire the motor
        public float ThrustRaysInteral = 0.015f;            // The number of seconds that must elapse before a thrust ray is emitted. only affects ground effects.        
        public LayerMask LayerMask;                         // Which layer mask the force rays that fire will collide with

        protected float mElapsedTime;
        protected float mCurrentThrust;

        protected float CurrentThrottle { get; set; }

        protected void DoRocketFixedUpdate() {
            if (this.CurrentThrottle >= this.ThrottleDeadZone) {
                mElapsedTime += Time.deltaTime;
                mCurrentThrust += this.CurrentThrottle * this.ThrustAccelerationPerSec * Time.deltaTime;
                mCurrentThrust = Math.Min(mCurrentThrust, this.PeakThrustForce);

                var thrustVectorForLift = GetThrustUnitVector() * mCurrentThrust;
                var thrustVectorForRepel = -thrustVectorForLift;

                // Emit thrust rays so we can kick up ground particles if contact is made
                //if (mElapsedTime >= this.ThrustRaysInteral) {
                //    var numberOfRays = mElapsedTime / this.ThrustRaysInteral;
                //    mElapsedTime -= this.ThrustRaysInteral * numberOfRays;

                //    for (var i = 0; i < numberOfRays; ++i) {
                //        FireThrustRay(invThrustVect);
                //    }
                //}

                if (this.ActOnBody != null) {
                    // Apply LIFTING force
                    this.ActOnBody.AddForceAtPosition(thrustVectorForLift, this.transform.position);
                }
            }
            else {
                mElapsedTime = 0f;
                mCurrentThrust = 0f;
            }
        }

        protected void FireThrustRay(Vector3 thrustVector) {
            var isHit = TestThrustRayHit(thrustVector, out var hitDetail);
            if (isHit) {
                //var contactPt = hitDetail.point;
            }
        }

        protected Vector3 GetThrustUnitVector() {
            return GetThrustVector(1f);
        }

        protected Vector3 GetThrustVector(float scalar) {
            var targetPt = this.transform.rotation * new Vector3(0f, this.ThrustDistance, 0f);
            var pointingVec = VectorUtility.GetPointingVector3(Vector3.zero, targetPt);

            return pointingVec * scalar;
        }

        protected Vector3 GetThrustVector() {
            return GetThrustVector(this.ThrustDistance);
        }

        protected bool TestThrustRayHit(Vector3 testVector, out RaycastHit hit) {
            return Physics.Raycast(this.transform.position, testVector, out hit, this.ThrustDistance, this.LayerMask, QueryTriggerInteraction.Ignore);
        }

#if UNITY_EDITOR_WIN
        public void OnDrawGizmos() {
            DrawThrustCone();
        }

        private void DrawThrustCone() {
            var startPt = this.transform.position;
            var endPt = startPt - GetThrustVector();             // The thrust vect is pointing "up", but visually it makes sense to show it downwards as a repeling force

            Gizmos.color = BaseColor;
            Gizmos.DrawLine(startPt, endPt);

            Handles.color = BaseColor;
            Handles.DrawWireDisc(endPt, this.transform.up, MiscUtils.Clamp(this.PeakThrustForce * 0.1f, 0.5f, 15f));
        }
    }
#endif
}
