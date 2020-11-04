using Core.Utility;
using UnityEngine;

namespace Core.Behaviors {
    /// <summary>
    /// Adjusts the color of a 3D light object in a single time stream (e.g.: non-repeating). Use this for
    /// controlling light flashing for short periods of time such as in an explosion or muzzle flash.
    /// </summary>
    public class LightAttenuator : MonoBehaviour {
        public Light TargetLight;
        public ColorTimeEvent[] Events;

        private Timeline<Color> mAttenuationTimeline;

        public void Awake() {
            this.mAttenuationTimeline = new Timeline<Color>(this.Events, this.AdjustLight);
        }

        public void Update() {
            this.mAttenuationTimeline.Update();
        }

        private void AdjustLight(Color c) {
            this.TargetLight.color = c;
        }
    }
}
