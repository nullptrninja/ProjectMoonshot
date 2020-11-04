using UnityEngine;

namespace Core.CogsV2 {
    /// <summary>
    /// Cogs V2 features a more precise trigger -> event model compared to V1. V2 introduces the idea of triggering
    /// specific actions on the target Cog - configurable at the Source rather than the Target. In Cog V1 the source has no idea
    /// what's on the other end of the trigger; with V2 there's a little less ambiguity as setting up the trigger action assumes
    /// some small level of knowledge about what the target can possibly do by specifying the action to be performed on activation.
    /// Secondly, multiple source triggers can trigger the same target to perform different actions if desired, where as this wasn't
    /// elegantly data-driven with V1 Cogs.
    /// 
    /// CogTriggerSource provides base abstract functionality for all trigger sources, you only need to implement a specific
    /// type of trigger vector and derive from this class.
    /// 
    /// To Use:    
    /// - Implement your own bevvy of CogTriggerSource-derived triggers.
    /// - Depending on how your trigger works, make a call to the TriggerTarget's Activate() method.
    /// </summary>
    public abstract class CogTriggerSource : MonoBehaviour {
        public CogTriggerTarget TriggerTarget;
        public CommonTargetAction TargetAction;
        public TriggerData DataOnTrigger;

        public CogTriggerSource() {
            this.DataOnTrigger = new TriggerData();
        }
        
        public void OnDrawGizmos() {            
            Gizmos.color = this.TriggerTarget == null ? Color.red : Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.25f);            

            if (this.TriggerTarget != null) {
                Gizmos.DrawLine(transform.position, this.TriggerTarget.transform.position);
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(this.TriggerTarget.transform.position, 0.5f);
            }
        }
    }
}
