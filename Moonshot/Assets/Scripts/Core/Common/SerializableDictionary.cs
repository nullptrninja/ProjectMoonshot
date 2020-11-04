using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Core.Common {
    /// <summary>
    /// A serializable dictionary. No root defined, use this within other serializable containers.
    /// </summary>
    [Serializable]
    public class SerializableDictionary<T1, T2> {
        [Serializable]
        public struct KVP<K, V> {
            [XmlAttribute("Key")]
            public K Key { get; set; }

            [XmlAttribute("Value")]
            public V Value { get; set; }
        }

        private Dictionary<T1, T2> mDict;

        public SerializableDictionary() {
            mDict = new Dictionary<T1, T2>();
        }

        public bool HasKey(T1 key) {
            return mDict.ContainsKey(key);
        }

        public void Add(T1 key, T2 val) {
            mDict.Add(key, val);
        }

        public void Remove(T1 key) {
            mDict.Remove(key);
        }

        public void Update(T1 key, T2 newValue) {
            mDict[key] = newValue;
        }

        public void Clear() {
            mDict.Clear();
        }

        // -- Properties -----

        [XmlIgnore()]
        public int Count {
            get { return mDict.Count; }
        }

        [XmlIgnore()]
        public T2 this[T1 key] {
            get {
                if (mDict.ContainsKey(key)) {
                    return mDict[key];
                }
                else {
                    throw new KeyNotFoundException("Key was not found");
                }
            }

            set {
                if (this.HasKey(key)) {
                    this.Update(key, value);
                }
                else {
                    this.Add(key, value);
                }
            }
        }

        [XmlIgnore()]
        public Dictionary<T1, T2>.KeyCollection Keys {
            get {
                return mDict.Keys;
            }
        }

        /// <summary>
        /// Primarily for serialization use but you can also use it to enumerate through
        /// all entries. This will create a deep copy of the dictionary so do not use
        /// this at runtime!
        /// </summary>
        [XmlArray("Entries"), XmlArrayItem("Entry")]
        public KVP<T1, T2>[] EntryList {
            get {
                KVP<T1, T2>[] list = new KVP<T1, T2>[mDict.Count];
                int i = 0;
                foreach (KeyValuePair<T1, T2> kvp in mDict) {
                    KVP<T1, T2> o = new KVP<T1, T2>();
                    o.Key = kvp.Key;
                    o.Value = kvp.Value;
                    list[i++] = o;
                }

                return list;
            }
            set {
                mDict = new Dictionary<T1, T2>();
                foreach (KVP<T1, T2> kvp in value) {
                    if (kvp.Key != null && kvp.Value != null) {
                        mDict.Add(kvp.Key, kvp.Value);
                    }
                }
            }
        }
    }
}
