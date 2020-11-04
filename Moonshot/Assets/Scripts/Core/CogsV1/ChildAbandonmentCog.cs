using UnityEngine;

namespace Core.CogsV1 {
    /// <summary>
    /// When triggered, all specified "ReleaseTargets" will be unparented (parent = null). You can configure
    /// this effect to be delayed for a few moments after activation, as well as if the release event will
    /// trigger the Cogs available on the released children. When the release is complete, this component
    /// will automatically disable itself.
    /// 
    /// You can also use this as a standalone component by clearing the OnlyReleaseOnTrigger flag. In this case,
    /// this component behaves as a timed release, following the TimeBeforeRelease value immediately after
    /// spawning into the level.
    /// </summary>
    public class ChildAbandonmentCog : CogAdapter {
        public float TimeBeforeRelease = 0.1f;
        public Transform[] ReleaseTargets;
        public CogAdapter[] TriggerAfterRelease;
        public bool AbandonChildrenToCurrentParent = false; // If true, the child's parent will be this node's parent (rather than at global level)
        public bool OnlyReleaseOnTrigger = false;           // If true, this cog will release its children only when Activated
        public bool ActivateChildCogsOnRelease = true;      // If true, all child cogs that are release will also be Activated
        public bool DestroySelfOnRelease = false;           // If true, the host object this is attached to will be destroyed after release

        private float mElapsedTime = 0f;

        public void Update() {
            if (!this.OnlyReleaseOnTrigger && this.TimeBeforeRelease != 0f) {
                mElapsedTime += Time.deltaTime;

                if (mElapsedTime >= this.TimeBeforeRelease) {
                    ReleaseChildren();
                    ActivateAfterRelease();
                    Shutdown();
                }
            }
        }

        public override void Activate(object source, CogData data) {
            if (this.OnlyReleaseOnTrigger) {
                ReleaseChildren();
                ActivateAfterRelease();

                base.Activate(source, data);
                Shutdown();
            }
        }

        private void ReleaseChildren() {
            if (this.ReleaseTargets != null) {                
                for (int i = 0; i < this.ReleaseTargets.Length; i++) {
                    var t = this.ReleaseTargets[i];

                    var targetParent = this.AbandonChildrenToCurrentParent ? this.transform.parent : null;
                    t.SetParent(targetParent);

                    if (this.ActivateChildCogsOnRelease) {
                        CogAdapter[] cogs = t.gameObject.GetComponents<CogAdapter>();
                        if (cogs != null && cogs.Length > 0) {                            
                            for (int c = 0; c < cogs.Length; c++) {
                                cogs[c].Activate(this.gameObject, null);                                
                            }
                        }
                    }
                }

                this.ReleaseTargets = null;
            }
        }

        private void ActivateAfterRelease() {
            if (this.TriggerAfterRelease != null) {
                var cogDataIfAvailable = this.gameObject.GetComponent<CogDataProvider>()?.Data ?? null;

                for (var i = 0; i < this.TriggerAfterRelease.Length; i++) {
                    this.TriggerAfterRelease[i].Activate(this, cogDataIfAvailable);
                }
            }
        }

        private void Shutdown() {
            // Shut down on done, or destroy
            if (this.DestroySelfOnRelease) {
                GameObject.Destroy(this.gameObject);
            }
            else {
                this.enabled = false;
            }
        }
    }
}
