using Assets.Scripts.Game.Data;
using Assets.Scripts.Game.Environment;
using UnityEngine;

namespace Assets.Scripts.Game.Managers {
    public class WindManager : MonoBehaviour {
        public BoxCollider WindVolume;
        public WindSettings Settings;
        public LayerMask InteractionLayerMask;

        public void OnTriggerEnter(Collider other) {
            var body = other.attachedRigidbody;
            CreateWindEmitter(body);
        }

        public void OnTriggerExit(Collider other) {
            var body = other.attachedRigidbody;
            DestroyWindEmitter(body);
        }

        private void CreateWindEmitter(Rigidbody target) {
            var emitterObj = new GameObject("wind_" + target.gameObject.name);
            var emitter = emitterObj.AddComponent<WindEmitter>();

            emitter.WindSpeedImpulse = this.Settings.WindSpeed;
            emitter.WindDirection = this.Settings.Direction;
            emitter.TargetBody = target;
            emitter.ImpulseDurationSeconds = this.Settings.ImpulseDurationSeconds;
            emitter.LayerMask = this.InteractionLayerMask;
        }

        private void DestroyWindEmitter(Rigidbody outgoingTarget) {
            var emitter = GameObject.Find("wind_" + outgoingTarget.gameObject.name);
            if (emitter != null) {
                GameObject.Destroy(emitter.gameObject);
            }
        }
    }
}
