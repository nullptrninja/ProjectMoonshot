using UnityEngine;

namespace Core.CogsV2.Sources {
    /// <summary>    
    /// When an entity enters the bounding volume, the target is triggered.
    /// </summary>
    public class ZoneTrigger : CogTriggerSource {        
        public Collider BoundingVolume;
        public bool IgnoreTriggers;

        public void OnTriggerEnter(Collider other) {
            if (this.TriggerTarget != null) {
                if (other.isTrigger && this.IgnoreTriggers) {
                    return;
                }

                this.TriggerTarget.Activate(this, this.TargetAction, this.DataOnTrigger);
            }
        }
    }
}
