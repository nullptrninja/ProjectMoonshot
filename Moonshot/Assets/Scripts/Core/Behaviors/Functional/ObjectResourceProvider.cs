using System.Collections.Generic;
using UnityEngine;

namespace Core.Behaviors.Functional {
    /// <summary>
    /// Provides dependency injection for components within a game object.
    /// 
    /// Have you wanted to share a POCO-based interface with a Unity component but didn't want to make
    /// an inherited abstract class? Well now you can!
    /// 
    /// Here's how to use this:
    /// - As high as you can, add this as a component to the game object
    /// - Components in the game object that implement the interface you want to share with other components should
    ///   reference this component and call RegisterResource() to register themselves using their interface type. Do this
    ///   during Awake()
    /// - Components that need interfaces should reference ObjectResourceProvider and call FindResource() during their
    ///   Start() call. It's recommended you cache those components locally as this isn't performant for real time use.
    /// </summary>
    public class ObjectResourceProvider : MonoBehaviour {
        private Dictionary<string, object> mResourceMap;

        public void Awake() {
            mResourceMap = new Dictionary<string, object>();
        }

        public void RegisterResource<T>(T resource) where T : class{
            var key = typeof(T).Name;
            if (mResourceMap.ContainsKey(key)) {
                mResourceMap.Remove(key);
            }

            mResourceMap.Add(key, resource);
        }

        public T FindResource<T>() where T : class {
            var key = typeof(T).Name;
            if (mResourceMap.ContainsKey(key)) {
                return mResourceMap[key] as T;
            }

            return null;
        }
    }
}
