using Assets.Scripts.Game.Data;
using UnityEngine;

namespace Assets.Scripts.Game.Environment {
    public class AtmosphericHandler : MonoBehaviour {
        public AtmosphereDescriptor AtmosphereSetting { get; set; }

        public void OnTriggerEnter(Collider other) {
            var otherRigidBody = other.attachedRigidbody;
            otherRigidBody.drag = this.AtmosphereSetting.Drag;
        }
    }
}
