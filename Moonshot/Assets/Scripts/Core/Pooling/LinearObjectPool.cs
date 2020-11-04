using Core.Types;
using System;
using UnityEngine;

namespace Core.Pooling {
    /// <summary>
    /// Object pool that provides high performance with the sole restriction that each object
    /// from the pool MUST have exactly the same predictable life spans. Objects from this pool must not return
    /// to the pool out of order in any way. This pool is useful for large volumes of non-interactive objects
    /// and performs up to 50% faster than dynamic list-based object pools such as RandomAccessObjectPool.
    /// Supports Minimize and Precache memory strategies.
    /// 
    /// The Minimize strategy will attempt to keep the amount of memory used by the pool at a minimum and should
    /// only grow when capacity is not sufficient for demand. This is useful if the lifetime of each object is fairly
    /// short and memory usage may be a concern.
    /// 
    /// Precache strategy will fill the entire specified pool size to capacity at Awake-time, thereby reducing
    /// the chances of runtime instantiation hiccups due to object creation. This is useful if your objects are
    /// not performant during creation.
    /// </summary>
    public class LinearObjectPool : ObjectPool {
        private GameObject mTemplate;
        private LinearWrappingArray<GameObject> mPool;
        private int mTotalInstancesCreated;

        private void Awake() {
            mTotalInstancesCreated = 0;
            mPool = new LinearWrappingArray<GameObject>(this.MaxObjects);

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

        private void PrecacheAllSlots() {
            for (int i = 0; i < this.MaxObjects - mPool.Count; i++) {
                GameObject go = GameObject.Instantiate(mTemplate) as GameObject;
                go.SetActive(false);
                mTotalInstancesCreated++;
                mPool.Add(go);
            }
        }

        private GameObject AcquireObject() {
            GameObject g = null;

            if (mPool.Count == 0) {
                if (mTotalInstancesCreated < this.MaxObjects) {
                    // Nothing in the pool but we have room to grow
                    CreateInstance();
                }
                // Else: nothing available for us.
            }

            if (mPool.Count > 0) {
                g = mPool.FetchNext();

                if (this.SetActiveOnAcquire) {
                    g.SetActive(true);
                }
            }

            return g;
        }

        private GameObject CreateInstance() {
            // Precodition: our total instances does not equal or exceed maxobjects
            GameObject go = GameObject.Instantiate(mTemplate);
            mTotalInstancesCreated++;
            mPool.Add(go);

            return go;
        }

        private void CheckInObject(GameObject o) {
            mPool.MoveNext();
        }

        public override bool CanAcquireObject {
            get {
                return mPool.Count > 0 || mTotalInstancesCreated <= this.MaxObjects;
            }
        }
    }
}
