using UnityEngine;

namespace Core.Behaviors.Functional.Sensors
{
    /// <summary>
    /// Detects when a stair-step has been approached and if so, provides a new Y-offset that can be applied to the host's
    /// world position in order to position it atop the step.
    /// 
    /// How this works:
    /// The sensor fires two raycasts, one at sensor-level, and one at ground (or near-ground) level towards the forward
    /// vector of the host.
    /// The host must first be colliding with the ground, so this sensor needs access to a GroundSensorNetwork. Once you
    /// are on the ground to checks to make sure that the "ground"-level ray collides with a Collider within its layer mask.
    /// Next it checks to ensure that the ray at sensor-level does not collide with an obstacle. If these conditions are
    /// all true then the Y-translation distance is fetched and returned.
    /// 
    /// How to setup this sensor:
    /// 1. The sensor's position within the host is effectively the "maximum stair height" that a step can be to be climbable.
    /// 2. Configure the sensor's DistanceFromGroundOffset to be just above the ground to allow some tolerance in step detection.
    /// 3. Configure the DetectForwardDistance just long enough to clear any on-host collision bounds that can block the sensor from
    ///    hitting anything. You don't need to go much further beyond the collision boundary; if you have trouble consistently climbing
    ///    stairs, then tweak this value.
    /// 4. Configure the LayerMask to match the value on your Unity Layer that you want to include in the collision check.
    /// </summary>
    [ExecuteInEditMode]
    public class StairDetectionSensor : MonoBehaviour {        
        public float DetectForwardDistance = 0.3f;
        public float DistanceFromGroundOffset = 0.01f;
        public int LayerMask;
        public SensorMap Sensors;           // Requires Ground sensors

        private int mFixedLayerMask;        // Cache, no need to reshift each time

        public void Awake() {
            mFixedLayerMask = 1 << this.LayerMask;
        }

        public bool ShouldClimbStair(out float yOffset) {
            var collidingWithGround = this.Sensors.GroundSensors.HasCollidedWithGround();

            if (collidingWithGround && CheckIsNextToStep(out var stepHit)) {
                var stepClearance = CheckCanClearStep();
                if (stepClearance) {                    
                    var newYPos = stepHit.collider.bounds.extents.y;
                    yOffset = newYPos;
                    return true;
                }
            }

            yOffset = 0f;
            return false;
        }

        private bool CheckIsNextToStep(out RaycastHit hit) {            
            return HitTestForStep(out hit);
        }

        private bool CheckCanClearStep() {            
            // Note the inversion. For check stair clearance we want to ensure nothing is in that particular spot
            return !HitTestForClearanceAboveStep();            
        }

        private bool HitTestForStep(out RaycastHit hit) {
            var positionAtGround = new Vector3(this.transform.position.x, this.DistanceFromGroundOffset, this.transform.position.z);
            return Physics.Raycast(positionAtGround, this.transform.forward, out hit, this.DetectForwardDistance, mFixedLayerMask);
        }

        private bool HitTestForClearanceAboveStep() {
            return Physics.Raycast(this.transform.position, this.transform.forward, out var _, this.DetectForwardDistance, mFixedLayerMask);
        }

        public void OnDrawGizmos() {
            var positionAtGround = new Vector3(this.transform.position.x, this.DistanceFromGroundOffset, this.transform.position.z);

            var clearanceCheck = this.transform.position + (this.transform.forward * this.DetectForwardDistance);
            var stepCheck = positionAtGround + (this.transform.forward * this.DetectForwardDistance);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.05f);
            Gizmos.DrawLine(transform.position, clearanceCheck);

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(positionAtGround, stepCheck);
        }
    }
}
