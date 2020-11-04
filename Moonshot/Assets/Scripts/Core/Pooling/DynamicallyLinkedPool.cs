using System;
using UnityEngine;

namespace Core.Pooling {
    /// <summary>
    /// The DynamicallyLinkedPool itself is not a real pool, but instead wraps another object pool at runtime
    /// by finding one via a specified entity name and proxying calls to it. This "object pool" is best used
    /// for objects that are dynamically spawned without the opportunity to be given a pool at design time.
    /// The catch is that the pool (or rather, entity containing an ObjectPool component) must be known ahead
    /// of time.
    /// 
    /// How to use:
    /// - In your world, create a root-level GameObject with a well known name. Add any other ObjectPool component to it.
    ///   It must only contain one ObjectPool component!
    /// - Configure that object pool as normal.
    /// - In your prefab, add an ObjectPoolLink component as well as this component to the object at root level.
    /// - Link this DynamicallyLinkedPool to the ObjectPoolLink's LinkedObjectPool property. You may leave the resource
    ///   instance name field blank as we will use the one in the well-known pool.
    /// - Configure this pool's WellKnownPoolProviderName to the name of the entity in the first step.
    /// - In your behavior code, check for an ObjectPoolLink and pull an entity from the pool as needed.
    /// </summary>
    public class DynamicallyLinkedPool : ObjectPool {
        public string WellKnownPoolProviderName = "DynamicPoolProvider";
        public bool CanCreateOwnPoolIfNeeded = false;           // Currently unused

        private ObjectPool mCachedObjectPool;

        public override ObjectPool RootObjectPool => mCachedObjectPool;

        public void Awake() {
            // Find the pool using the specified pool name
            GameObject pool = GameObject.Find(this.WellKnownPoolProviderName);
            if (pool != null) {
                mCachedObjectPool = pool.GetComponent<ObjectPool>();
                if (mCachedObjectPool == null) {
                    throw new InvalidOperationException("The well known named object is not a valid ObjectPool host: " + this.WellKnownPoolProviderName);
                }
            }
            else {
                throw new InvalidOperationException("Unable to find the specified object pool by the name: " + this.WellKnownPoolProviderName);
            }
        }

        public override GameObject GetObject() {
            return mCachedObjectPool.GetObject();
        }

        public override void ReleaseObject(GameObject o) {
            mCachedObjectPool.ReleaseObject(o);
        }

        public override bool CanAcquireObject {
            get {
                return mCachedObjectPool.CanAcquireObject;
            }
        }
    }
}
