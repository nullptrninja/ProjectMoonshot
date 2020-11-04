using UnityEngine;

namespace Core.Behaviors.Functional.Sensors {
    public class GroundSensor : MonoBehaviour {
        public float MaxTestDistance = 10f;
        public float CloseDistanceThreadhold = 5f;
        public float PositiveDeadZoneDistance = 0.05f;
        public int LayerMask = 0;

        private int mFixedLayerMask;        // Cache, no need to reshift each time

        private void Awake() {
            mFixedLayerMask = 1 << this.LayerMask;
        }

        /// <summary>
        /// Returns true if the sensor is near the ground based on the specified threshold. Note that
        /// being "close" can also mean "is touching the ground".
        /// </summary>
        /// <returns>True if the sensor distance is less than or equal to the threshold</returns>
        public virtual bool IsCloseToGround() {
            HitTestGround(this.MaxTestDistance, out var hit);
            return hit.distance > this.PositiveDeadZoneDistance && hit.transform != null;
        }

        /// <summary>
        /// Returns true if the sensor is near the ground based on the specified threshold BUT it is
        /// also not touching the ground.
        /// </summary>
        /// <returns>True if the sensor is near but not touching the ground</returns>
        public virtual bool IsCloseToGroundButNotTouching() {            
            HitTestGround(this.MaxTestDistance, out var hit);
            return hit.distance > this.PositiveDeadZoneDistance && hit.distance <= this.CloseDistanceThreadhold && hit.transform == null;
        }

        /// <summary>
        /// Returns true if the sensor is 0 units away from the ground.
        /// </summary>
        /// <returns>True if sensor touches the ground</returns>
        public virtual bool HasCollidedWithGround() {            
            HitTestGround(this.MaxTestDistance, out var hit);
            return hit.distance <= this.PositiveDeadZoneDistance && hit.transform != null;
        }


        private bool HitTestGround(float maxDistance, out RaycastHit hit) {
            return Physics.Raycast(this.transform.position, Vector3.down, out hit, maxDistance, mFixedLayerMask);
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.05f);
            Gizmos.DrawLine(transform.position, transform.position + (Vector3.down * this.MaxTestDistance));
        }
    }
}
