using Core.State;
using Core.Systems.Messaging;
using System.Linq;

namespace Core.CogsV2.Targets {
    public class MessageBroadcasterTarget : CogTriggerTarget {
        public static readonly string GroupIdNamedProp = "GroupId";
        public static readonly string SubIdNamedProp = "SubId";

        public int ChannelId;

        public override bool Activate(CogTriggerSource source, CommonTargetAction action, TriggerData triggerData) {
            if (action == CommonTargetAction.Activate || action == CommonTargetAction.DefaultActivation) {                
                var groupId = triggerData.NamedProperties.FirstOrDefault(o => o.Key.Equals(GroupIdNamedProp));
                var subId = triggerData.NamedProperties.FirstOrDefault(o => o.Key.Equals(SubIdNamedProp));

                // Cannot execute without appropriate parameters
                if (groupId == null || subId == null) {
                    return false;
                }

                var msg = new GenericMessage(groupId.Value, subId.Value);
                msg.ObjectData = triggerData.UnityTransformData;
                msg.StringData = triggerData.StringData;

                var hook = GlobalObjectsUtility.FromStateHook<BaseStateHook>();
                hook.GameMessageRouter.Send(this.ChannelId, msg);
                return true;
            }

            return false;
        }
    }
}
