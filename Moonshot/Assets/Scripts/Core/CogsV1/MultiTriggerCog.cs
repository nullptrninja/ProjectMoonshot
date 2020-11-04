namespace Core.CogsV1 {
    /// <summary>
    /// Allows you to set up a single point of entry trigger that will activate multiple cogs. Treat this as a manifold
    /// where, for example, pressing one button will trigger this cog, which will in turn, trigger three other objects at
    /// the same time.
    /// </summary>
    public class MultiTriggerCog : CogAdapter {
        /// <summary>
        /// If true, deactivations against this cog will also propagate to all specified trigger targets.
        /// </summary>
        public bool AllowDeactivations = false;

        /// <summary>
        /// A list of all Cogs that will be activated upon this cog being activated.
        /// </summary>
        public CogAdapter[] TriggerTargets;
        
        public override void Activate(object source, CogData data) {
            for (int i = 0; i < this.TriggerTargets.Length; i++) {            
                this.TriggerTargets[i].Activate(this, data);
            }

            base.Activate(source, data);
        }

        public override void Deactivate(object source, CogData data) {
            if (this.AllowDeactivations) {
                for (int i = 0; i < this.TriggerTargets.Length; i++) {
                    this.TriggerTargets[i].Deactivate(this, data);
                }
            }
            base.Deactivate(source, data);
        }
    }
}
