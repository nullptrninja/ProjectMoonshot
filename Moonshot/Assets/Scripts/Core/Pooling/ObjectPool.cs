using System;
using UnityEngine;

namespace Core.Pooling {
    public abstract class ObjectPool : MonoBehaviour {
        public enum MemoryStrategy {
            Minimize,
            Precache
        }

        public int MaxObjects = 50;        
        public string ResourceInstanceName;
        public bool SetActiveOnAcquire = true;
        public MemoryStrategy PoolStrategy;

        public abstract GameObject GetObject();
        public abstract void ReleaseObject(GameObject o);

        public virtual bool CanAcquireObject {
            get { throw new NotImplementedException("CanAcquireObject Abstract method not available"); }
        }

        public virtual ObjectPool RootObjectPool => this;
    }
}
