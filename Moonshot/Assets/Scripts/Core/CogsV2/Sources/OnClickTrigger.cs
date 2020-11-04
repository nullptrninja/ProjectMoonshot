using Core.State;
using Core.Systems.Messaging;
using Core.Utility;
using UnityEngine;

namespace Core.CogsV2.Sources {
    /// <summary>
    /// When this entity receives a MouseUp event, the trigger target will be activated
    /// </summary>
    public class OnClickTrigger : CogTriggerSource, IMessageListener {
        public float CooldownTime;
        public float ClickDriftTolerance = 250f;         // In squared pixel distance

        private CooldownTimer mCooldownTimer;
        private bool mMouseDownSelected;
        private Vector3 mMouseDownPt;

        public void Start() {
            mCooldownTimer = new CooldownTimer(this.CooldownTime);
            var hook = GlobalObjectsUtility.FromStateHook<BaseStateHook>();
            hook.GameMessageRouter.AddListener(Common.Messaging.GameChannel, this);
        }

        public void Update() {
            mCooldownTimer.Update();
        }

        public void OnMouseDown() {
            mMouseDownSelected = true;
            mMouseDownPt = UnityEngine.Input.mousePosition;
            var hook = GlobalObjectsUtility.FromStateHook<BaseStateHook>();
            hook.GameMessageRouter.Send(Common.Messaging.GameChannel,
                new GenericMessage(Common.Messaging.GameChannel, Common.Messaging.MouseDownEventSubId) {
                    ObjectData = this
                });
        }

        public void OnMouseUp() {
            var samePositionDiff = (UnityEngine.Input.mousePosition - mMouseDownPt).sqrMagnitude;
            var withinTolerance = samePositionDiff <= this.ClickDriftTolerance;

            if (mMouseDownSelected && this.TriggerTarget != null && withinTolerance && mCooldownTimer.CooledDown) {
                this.TriggerTarget.Activate(this, this.TargetAction, this.DataOnTrigger);
                mCooldownTimer.BeginCooldown();
                mMouseDownSelected = false;
            }
        }

        public MessageRouter.Result ReceiveMessage(BaseMessage message) {
            if (message.GroupId == Common.Messaging.MouseEventGroupId) {
                if (message.SubId == Common.Messaging.MouseDownEventSubId) {
                    // This helps us ensure that the object initially clicked on
                    // is the same as the one we're releasing on. If it isn't we
                    // clear the internal tracking flag
                    if (message.ObjectData as OnClickTrigger != this) {
                        mMouseDownSelected = false;
                        return MessageRouter.Result.AcceptedPassive;
                    }
                }
            }

            return MessageRouter.Result.Ignored;
        }
    }
}
