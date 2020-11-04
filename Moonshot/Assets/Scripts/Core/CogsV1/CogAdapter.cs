using Core.Behaviors.Functional;
using UnityEngine;

namespace Core.CogsV1 {
    public abstract class CogAdapter : MonoBehaviour, ICog {
        /// <summary>
        /// When this cog receives an activation from another source, you can optionally
        /// specify another cog that this cog will automatically trigger.
        /// </summary>
        public CogAdapter TriggerOnActivate;

        /// <summary>
        /// When this cog receives a deactivation from another source, you can optionally
        /// specify another cog that this cog will automatically trigger.
        /// </summary>
        public CogAdapter TriggerOnDeactivate;

        public virtual CogData Output {
            get { return null; }
        }

        public virtual void Activate(object source, CogData data) {
            if (this.TriggerOnActivate != null) {
                this.TriggerOnActivate.Activate(this, data);
            }
        }

        public virtual void Deactivate(object source, CogData data) {
            if (this.TriggerOnDeactivate != null) {
                this.TriggerOnDeactivate.Deactivate(this, data);
            }
        }

        public virtual void Mesh(ICog cog) {
            Debug.Log("Cog Mesh attempt on base type not allowed");
        }

        protected CogDataProvider FetchCogDataProvider(GameObject source) {
            if (source != null) {
                return source.GetComponent<CogDataProvider>();
            }
            else {
                return null;
            }
        }
    }
}
