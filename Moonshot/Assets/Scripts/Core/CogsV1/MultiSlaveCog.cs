namespace Core.CogsV1 {
    /// <summary>
    /// Allows you to set up a COG trigger with slave dependencies. When this cog receives the minimum number
    /// of required triggers from its slaves, it will then trigger the specified Cog in the FinalTriggerTarget
    /// property.
    /// FinalTriggerTarget is functionally separate from the base TriggerOnActivate field as the TriggerOnActivate COGS will
    /// be triggered each time a slave cog triggers this cog, which can be used for other effects and behaviors.
    /// </summary>
    public class MultiSlaveCog : CogAdapter {
        /// <summary>
        /// How many activations this cog must receive before it activates its FinalTriggerTarget cog.
        /// </summary>
        public int RequiredActivations;

        /// <summary>
        /// If true, deactivations from slaves will count against the total required activations. This means
        /// that the "activation progress" can be reversed due to deactivation events.
        /// </summary>
        public bool AllowDeactivations;
        
        /// <summary>
        /// Another cog to trigger after all required slave activations are met.
        /// </summary>
        public CogAdapter FinalTriggerTarget;

        private int mCurrentSlaveActivations;

        private void Start() {
            this.mCurrentSlaveActivations = 0;
        }

        public override void Activate(object source, CogData data) {
            mCurrentSlaveActivations++;
            if (mCurrentSlaveActivations >= this.RequiredActivations) {
                if (this.FinalTriggerTarget != null) {
                    this.FinalTriggerTarget.Activate(source, data);
                }
            }

            base.Activate(source, data);
        }

        public override void Deactivate(object source, CogData data) {
            if (this.AllowDeactivations) {
                mCurrentSlaveActivations--;

                if (mCurrentSlaveActivations < 0) {
                    mCurrentSlaveActivations = 0;
                }
            }

            base.Deactivate(source, data);
        }
    }
}
