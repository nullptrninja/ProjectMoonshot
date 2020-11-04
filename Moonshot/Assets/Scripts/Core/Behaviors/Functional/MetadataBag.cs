using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Behaviors.Functional {
    /// <summary>
    /// Allows easy storage and retrieval of miscellaneous data attached to an object.
    /// Supports Integer, Float, and String types attached to a string key. Key retrievals are
    /// case sensitive.
    /// </summary>
    [Serializable]
    public class MetadataBag : MonoBehaviour {
        [Serializable]
        public class IntTuple   {
            public string Key;
            public int Value;
        }

        [Serializable]
        public class FloatTuple {
            public string Key;
            public float Value;
        }

        [Serializable]
        public class StringTuple {
            public string Key;
            public string Value;
        }

        // Originally had a templated type for Tuples but Unity's editor doesn't support the display of generic types
        public List<IntTuple> IntegerBag;
        public List<FloatTuple> FloatBag;
        public List<StringTuple> StringBag;

        public int GetInteger(string key, int defaultValue = 0) {
            
            if (this.IntegerBag.Any(o => o.Key == key)) {
                return this.IntegerBag.Single(o => o.Key == key).Value;
            }
            else {
                return defaultValue;
            }
        }

        public float GetFloat(string key, float defaultValue = 0f) {            
            if (this.FloatBag.Any(o => o.Key == key)) {
                return this.FloatBag.Single(o => o.Key == key).Value;
            }
            else {
                return defaultValue;
            }
        }

        public string GetString(string key, string defaultValue = "") {
            if (this.StringBag.Any(o => o.Key == key)) {
                return this.StringBag.Single(o => o.Key == key).Value;
            }
            else {
                return defaultValue;
            }
        }

        public virtual bool TryGetInteger(string key, out int val) {
            if (this.IntegerBag.Any(o => o.Key == key)) {
                val = this.IntegerBag.Single(o => o.Key == key).Value;
                return true;
            }
            val = default;
            return false;
        }

        public virtual bool TryGetFloat(string key, out float val) {
            if (this.FloatBag.Any(o => o.Key == key)) {
                val = this.FloatBag.Single(o => o.Key == key).Value;
                return true;
            }
            val = default;
            return false;
        }

        public virtual bool TryGetString(string key, out string val) {
            if (this.StringBag.Any(o => o.Key == key)) {
                val = this.StringBag.Single(o => o.Key == key).Value;
                return true;
            }
            val = default;
            return false;
        }

        public void AddOrUpdateString(string key, string value)
        {        
            // create a new stringbag if none
            if (this.StringBag == null) {
                this.StringBag = new List<StringTuple>();
            }
            
            if (!this.StringBag.Exists(o => o.Key == key))  {                
                this.StringBag.Add(new StringTuple() { Key = key, Value = value });
            }
            else {
                this.StringBag.Find(o => o.Key == key).Value = value;
            }        
        }
        public void AddOrUpdateFloat(string key, float value)
        {
            // create a new stringbag if none
            if (this.FloatBag == null) {
                this.FloatBag = new List<FloatTuple>();
            }
            
            if (!this.FloatBag.Exists(o => o.Key == key)) {
                this.FloatBag.Add(new FloatTuple() { Key = key, Value = value });
            }
            else {
                this.FloatBag.Find(o => o.Key == key).Value = value;
            }
        }

        public void AddOrUpdateInt(string key, int value)
        {
            // create a new stringbag if none
            if (this.IntegerBag == null) {
                this.IntegerBag = new List<IntTuple>();
            }
            
            if (!this.IntegerBag.Exists(o => o.Key == key)) {
                this.IntegerBag.Add(new IntTuple() { Key = key, Value = value });
            }
            else {
                this.IntegerBag.Find(o => o.Key == key).Value = value;
            }
        }

        public void RemoveString(string key) {
            if (this.StringBag != null) {
                var entry = this.StringBag.First(t => t.Key.Equals(key));
                this.StringBag.Remove(entry);
            }
        }

        public void RemoveInteger(string key) {
            if (this.IntegerBag != null) {
                var entry = this.IntegerBag.First(t => t.Key.Equals(key));
                this.IntegerBag.Remove(entry);
            }
        }

        public void RemoveFloat(string key) {
            if (this.IntegerBag != null) {
                var entry = this.FloatBag.First(t => t.Key.Equals(key));
                this.FloatBag.Remove(entry);
            }
        }
    }
}
