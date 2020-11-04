using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Pooling {
    /// <summary>
    /// RandomAccessObjectPool covers the most common pooling scenarios. Uses a fixed-size dynamic collection that
    /// enables pooling of objects with unpredictable life spans. Performance with this type of pool should be
    /// observed carefully as it can get out of hand if you are not careful. Use a LinearObjectPool instead when
    /// you are able to.
    /// Supports Minimize and Precache memory strategies only.
    /// 
    /// The Minimize strategy will attempt to keep the amount of memory used by the pool at a minimum and should
    /// only grow when capacity is not sufficient for demand. This is useful if the lifetime of each object is fairly
    /// short and memory usage may be a concern.
    /// 
    /// Precache strategy will fill the entire specified pool size to capacity at Awake-time, thereby reducing
    /// the chances of runtime instantiation hiccups due to object creation. This is useful if your objects are
    /// not performant to initialize.
    /// </summary>
    public class RandomAccessObjectPool : ObjectPool {
        private GameObject mTemplate;
        private List<GameObject> mAvailablePool;
        private List<GameObject> mActivePool;

        private int mTotalInstancesCreated;

        private void Awake() {
            mTotalInstancesCreated = 0;
            mAvailablePool = new List<GameObject>(this.MaxObjects);     // Start off at MaxObjects. Do not exceed this size.
            mActivePool = new List<GameObject>(this.MaxObjects);

            mTemplate = Resources.Load<GameObject>(this.ResourceInstanceName);
            if (mTemplate == null) {
                throw new InvalidOperationException(string.Format("Template {0} could not be established", this.ResourceInstanceName));
            }

            if (this.PoolStrategy == MemoryStrategy.Precache) {
                PrecacheAllSlots();
            }
        }

        public override GameObject GetObject() {
            return AcquireObject();
        }

        public override void ReleaseObject(GameObject o) {
            CheckInObject(o);
        }
                
        private GameObject AcquireObject() {
            GameObject g = null;
                        
            if (mAvailablePool.Count > 0) {                
                // Use available resources first before instantiating
                //Debug.Log("RandomAccessObjectPool reused an instance of " + this.ResourceInstanceName);
                g = CheckOutObject();
            }
            else if (mAvailablePool.Count == 0 && mTotalInstancesCreated < this.MaxObjects) {
                // Create an instance because we're out of free objects but still have room to grow
                //Debug.Log("RandomAccessObjectPool created a new instance of " + this.ResourceInstanceName);
                g = CreateInstance();
            }
            // else - nothing is available and we've maxed out all of our resources
            
            return g;
        }

        private GameObject CreateInstance() {
            // Precondition: our total instances does not equal or exceed maxobjects
            GameObject go = GameObject.Instantiate(mTemplate);
            mTotalInstancesCreated++;
            mActivePool.Add(go);

            return go;
        }
        
        private GameObject CheckOutObject() {
            // Precondition: there's enough objects in the available pool to call this method
            int lastIndex = mAvailablePool.Count - 1;
            GameObject go = mAvailablePool[lastIndex];
            mAvailablePool.RemoveAt(lastIndex);
            mActivePool.Add(go);

            if (this.SetActiveOnAcquire) {
                go.SetActive(true);
            }

            return go;
        }

        private void PrecacheAllSlots() {
            // We should only precache at the start of the pool's lifetime but it's easy
            // enough to support precaching anytime during execution; not that we'd actually do it though.
            int objectsNeeded = this.MaxObjects - (mActivePool.Count + mAvailablePool.Count);

            for (int i = 0; i < objectsNeeded; i++) {
                GameObject go = GameObject.Instantiate(mTemplate) as GameObject;
                go.SetActive(false);
                mTotalInstancesCreated++;
                mAvailablePool.Add(go);
            }
        }

        private void CheckInObject(GameObject o) {
            if (mActivePool.Contains(o)) {
                o.SetActive(false);
                mActivePool.Remove(o);
                mAvailablePool.Add(o);
            }
        }

        public override bool CanAcquireObject {
            get {
                return mAvailablePool.Count > 0 || mTotalInstancesCreated < this.MaxObjects;
            }
        }
    }
}
