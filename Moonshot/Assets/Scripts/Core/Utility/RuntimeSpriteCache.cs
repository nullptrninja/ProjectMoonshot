using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Utility {
    /// <summary>
    /// Provides a singleton cache during runtime for loading individual sprites from the resources folder by offset.
    /// This library is not attached to any runtime objects that you may instantiate within your game; as such, use this
    /// at your own discretion with regards to memory management.
    /// There is a maintenance function that you can call occasionally to help cull out lesser used sprite collections and
    /// is configurable through various properties within this class.
    /// </summary>
    public class RuntimeSpriteCache  {
        public const float DefaultAccessFrequencyCutOffLimit = 0.0083f;         // Approximately 5 accesses within 10 minutes. Any less than that and this resource may be released.
        public const float DefaultTotalUsePercentageCutOffLimit = 0.05f;        // If resource is used less than 5% in the maintenance interval, it may be released.
        public const float DefaultLastUseTimeThreshold = 360f;                  // If you haven't used this resource in this many seconds, it may be released.
                
        private static RuntimeSpriteCache SingletonCache = new RuntimeSpriteCache();
        public static RuntimeSpriteCache Instance
        {
            get {
                return RuntimeSpriteCache.SingletonCache;
            }
        }

        private class CachedSpriteGroup {
            private Sprite[] mSprites;

            public CachedSpriteGroup(string assetName, Sprite[] collection) {
                this.AssetName = assetName;
                mSprites = collection;
                ResetUsageStats();
            }

            public Sprite GetSprite(int offset) {
                this.Accesses++;
                this.LastAccessTime = Time.realtimeSinceStartup;
                return mSprites[offset];
            }

            public void ResetUsageStats() {
                this.Accesses = 0;
                this.FirstAccessTime = Time.realtimeSinceStartup;
                this.LastAccessTime = Time.realtimeSinceStartup;
            }

            public string AssetName {
                get;
                private set;
            }

            public int Accesses {
                get;
                private set;
            }

            public float LastAccessTime {
                get;
                private set;
            }

            public float FirstAccessTime {
                get;
                private set;
            }

            public float AccessFrequency {
                get {
                    return this.Accesses / (Time.realtimeSinceStartup - this.FirstAccessTime);
                }
            }
        }

        public enum CullMode {
            None,
            ByAccessFrequency,
            ByUsePercentage,
            ByLastUseTime,
        }

        private Dictionary<string, CachedSpriteGroup> mSpriteCache;
        private int mTotalFetches = 0;

        private RuntimeSpriteCache() {
            mSpriteCache = new Dictionary<string, CachedSpriteGroup>();

            // Defaults
            this.CullingMode = CullMode.ByAccessFrequency;
            this.AccessFrequencyCutOffLimit = RuntimeSpriteCache.DefaultAccessFrequencyCutOffLimit;
            this.TotalUsePercentageCutOffLimit = RuntimeSpriteCache.DefaultTotalUsePercentageCutOffLimit;
            this.LastUseTimeThreshold = RuntimeSpriteCache.DefaultLastUseTimeThreshold;
        }

        /// <summary>
        /// Clears the sprite cache. Occasionally, this could be useful inbetween sessions or when we hit certain memory limits.
        /// </summary>
        public void ClearCache() {
            Debug.Log("RuntimeSpriteCache: Cleared cache");
            mSpriteCache.Clear();
            mTotalFetches = 0;
        }

        public void RunMaintenance() {
            switch (this.CullingMode) {
                case CullMode.ByAccessFrequency:
                    DoAccessFrequencyCull();
                    break;

                case CullMode.ByUsePercentage:
                    DoTotalPercentageCull();
                    break;

                case CullMode.ByLastUseTime:
                    DoLastUseTimeCull();
                    break;
            }
        }

        public Sprite FetchSprite(string assetName, int offset) {
            CachedSpriteGroup collection = this.FetchSpriteGroup(assetName);
            
            // Don't check for offset bounds explicitly.
            if (collection != null) {
                mTotalFetches++;
                return collection.GetSprite(offset);
            }
            else {
                Debug.Log(String.Format("RuntimeSpriteCache: Unable to fetch sprite from asset: {0}, index: {1}", assetName, offset));
                return null;
            }
        }

        private float CalculateUsagePercentage(CachedSpriteGroup group) {
            if (mTotalFetches > 0) {
                return group.Accesses / mTotalFetches;
            }
            else {
                return 0f;
            }
        }

        private CachedSpriteGroup FetchSpriteGroup(string assetName) {
            if (!mSpriteCache.ContainsKey(assetName)) {
                Sprite[] collection = Resources.LoadAll<Sprite>(assetName);
                if (collection != null) {
                    mSpriteCache.Add(assetName, new CachedSpriteGroup(assetName, collection));
                }
                else {
                    return null;
                }
            }

            return mSpriteCache[assetName];            
        }

        private void DoAccessFrequencyCull() {
            List<string> removeThese = new List<string>(mSpriteCache.Count);

            foreach (CachedSpriteGroup csg in mSpriteCache.Values) {
                if (csg.AccessFrequency < this.AccessFrequencyCutOffLimit) {
                    removeThese.Add(csg.AssetName);
                }
            }

            foreach (string s in removeThese) {
                Debug.Log(String.Format("RuntimeSpriteCache: Culled {0} from sprite cache due to infrequent access", s));
                mSpriteCache.Remove(s);
            }
        }

        private void DoTotalPercentageCull() {
            List<string> removeThese = new List<string>(mSpriteCache.Count);

            foreach (CachedSpriteGroup csg in mSpriteCache.Values) {
                if (CalculateUsagePercentage(csg) < this.TotalUsePercentageCutOffLimit) {
                    removeThese.Add(csg.AssetName);
                }
            }

            foreach (string s in removeThese) {
                Debug.Log(String.Format("RuntimeSpriteCache: Culled {0} from sprite cache due to total usage below threshold", s));
                mSpriteCache.Remove(s);
            }
        }

        private void DoLastUseTimeCull() {
            List<string> removeThese = new List<string>(mSpriteCache.Count);

            foreach (CachedSpriteGroup csg in mSpriteCache.Values) {
                if (Time.realtimeSinceStartup - csg.LastAccessTime > this.LastUseTimeThreshold) {
                    removeThese.Add(csg.AssetName);
                }
            }

            foreach (string s in removeThese) {
                Debug.Log(String.Format("RuntimeSpriteCache: Culled {0} from sprite cache due to last access time past threshold", s));
                mSpriteCache.Remove(s);
            }
        }

        public CullMode CullingMode {
            get;
            set;
        }

        public float AccessFrequencyCutOffLimit {
            get;
            set;
        }

        public float TotalUsePercentageCutOffLimit {
            get;
            set;
        }

        public float LastUseTimeThreshold {
            get;
            set;
        }
    }
}