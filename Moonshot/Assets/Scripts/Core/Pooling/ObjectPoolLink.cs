using UnityEngine;

namespace Core.Pooling {
    /// <summary>
    /// Holds a reference to an ObjectPool. While components can natively contain a reference to
    /// a specific type of ObjectPool, this may not always be ideal if the prefab is used in both pooled
    /// and non-pooled scenarios. With an ObjectPoolLink, you can add this component to a prefab and
    /// optionally specify a reference to a pool. Core components that support pooling will know to look for
    /// an ObjectPoolLink component to see if a pool is being used before attempting to spawn
    /// or destroy objects. For prefabs that can also serve in non-pooled scenarios, you can leave
    /// this component in but leave the LinkedObjectPool member set to null.
    /// 
    /// Optionally, you can set "AutoDestroyIfUnused" to True to remove this component at start if
    /// no ObjectPool is linked at runtime. This can save some CPU cycles in certain scenarios.
    /// 
    /// How to use:
    /// - If a prefab you have is supposed to create game objects from within its own behavior code,
    ///   you can give it an object pool instead and spawn objects from that pool. However, a prefab
    ///   may exist outside of design-time (spawned dynamically), this is where this link comes in.
    /// - Simply add this ObjectPoolLink to your prefab. If you're in design-time, specify a pool
    ///   reference within the LinkedObjectPool field.
    /// - Your own code can look for the ObjectPoolLink and see if there's a pool specified. If so
    ///   then use the pool to spawn objects, otherwise do it yourself. Your ObjectPool can come
    ///   from anywhere, on the host object itself or from somewhere in the world.
    /// </summary>
    public class ObjectPoolLink : MonoBehaviour {
        public ObjectPool LinkedObjectPool;
        public bool AutoDestroyIfUnused = false;

        public void Awake() {
            if (this.AutoDestroyIfUnused && this.LinkedObjectPool == null) {
                GameObject.Destroy(this);
            }
        }

        public bool HasObjectPool {
            get {
                return this.LinkedObjectPool != null;
            }
        }
    }
}
