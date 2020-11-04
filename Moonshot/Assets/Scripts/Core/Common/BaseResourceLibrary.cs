using System;
using System.Collections.Generic;
using Core.Behaviors.Functional;
using UnityEngine;

namespace Core.Common {
    /// <summary>
    /// A common pattern that's cropped up is having these library classes that serve up objects stored
    /// within the Resources folder under a subdirectory. This provides functionality to quickly instantiate
    /// those objects. All objects in a given subdirectory must include a ResourceDescriptor component in order
    /// to be accessible by this library.
    /// Currently the default (and only) behavior is to precache all items in that directory if their precache
    /// flag is enabled. Uncached resources are currently not accessible by this library.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseResourceLibrary<T> where T : MonoBehaviour {
        private readonly String ResourceSubDirectory;

        private Dictionary<int, GameObject> mResourceObjects;

        public BaseResourceLibrary(string subDirectory, bool preCacheAllResources) {
            this.ResourceSubDirectory = subDirectory;
            mResourceObjects = new Dictionary<int, GameObject>();
            
            LoadAllResources(preCacheAllResources);
        }

        public GameObject CreateInstance(int resourceId) {
            return GameObject.Instantiate<GameObject>(mResourceObjects[resourceId]);
        }

        public T GetResource(int resourceId) {
            return mResourceObjects[resourceId].GetComponent<T>();
        }

        private void LoadAllResources(bool preCacheAllResources) {
            GameObject[] availableResources = Resources.LoadAll<GameObject>(ResourceSubDirectory);
            foreach (GameObject o in availableResources) {
                var obj = o.GetComponent<ResourceDescriptor>();
                if (obj != null && preCacheAllResources && obj.IsPreCacheable) {
                    mResourceObjects.Add(obj.ResourceId, o);
                }
            }
        }
    }
}
