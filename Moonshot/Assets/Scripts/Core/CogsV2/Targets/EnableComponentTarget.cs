using UnityEngine;

namespace Core.CogsV2.Targets
{
    public class EnableComponentTarget : CogTriggerTarget
    {
        public MonoBehaviour EnableTarget;

        public override bool Activate(CogTriggerSource source, CommonTargetAction action, TriggerData triggerData)
        {
            if (action == CommonTargetAction.Activate)
            {
                this.EnableTarget.enabled = true;
                return true;
            }
            else if (action == CommonTargetAction.Deactivate)
            {
                this.EnableTarget.enabled = false;
                return true;
            }
            else if (action == CommonTargetAction.Toggle)
            {
                this.EnableTarget.enabled = !this.EnableTarget.enabled;
                return true;
            }

            return false;
        }
    }
}
