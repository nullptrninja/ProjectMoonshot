using System;
using UnityEngine;

namespace Core.Behaviors.Functional.Sensors
{
    /// <summary>
    /// Performs basic single-object sensing/tracking.
    /// 
    /// Implementation:
    /// - Add trigger collider and this sensor to its own node
    /// </summary>
    public class OverlapSensor : MonoBehaviour
    {
        public bool EnableLayerMask;
        public int LayerMask;
        public bool IncludeTriggers;
        public bool EnableAutoForceDetection;       // If this sensor doesn't seem to be tracking overlaps correctly you can enable this
                                                    // option to brute force-search for colliders on each MeasureBinary() call. Use sparingly or
                                                    // adjust the detection interval to lessen the performance impact.

        public int ForceDetectionInterval = 15;

        public GameObject OverlappingObject { get; private set; }

        private Collider mColliderCache;
        private Type mFilterByType;
        private int mCurrentFrameInterval;

        public void Awake() {            
            mColliderCache = this.gameObject.GetComponent<Collider>();
        }

        public void SetTypeFilter(Type t)   {
            mFilterByType = t;
        }

        private void Update() {
            if (this.EnableAutoForceDetection) {
                mCurrentFrameInterval++;

                if (mCurrentFrameInterval >= this.ForceDetectionInterval) {
                    mCurrentFrameInterval = 0;
                    ForceRefresh();
                }
            }
        }

        public void ForceRefresh() {
            this.OverlappingObject = null;

            // Totally a hack.
            var colliders = Physics.OverlapBox(mColliderCache.transform.position, mColliderCache.bounds.extents);
            for (var i = 0; i < colliders.Length; i++) {
                if (mColliderCache != colliders[i] && colliders[i].transform.root != this.transform.root) {
                    mColliderCache.SendMessage("OnTriggerEnter", colliders[i]);
                }
            }
        }

        public void OnTriggerEnter(Collider other) {
            if ((this.EnableLayerMask && other.gameObject.layer != this.LayerMask) || 
                (other.isTrigger && !this.IncludeTriggers)) {
                return;
            }

            if (mFilterByType != null) {
                var foundComponent = other.gameObject.GetComponent(mFilterByType);
                if (foundComponent != null) {
                    this.OverlappingObject = other.gameObject;
                }
            }
            else {
                this.OverlappingObject = other.gameObject;
            }
        }

        public void OnTriggerExit(Collider other) {
            if (other.gameObject == this.OverlappingObject) {
                this.OverlappingObject = null;
            }
        }
    }
}
