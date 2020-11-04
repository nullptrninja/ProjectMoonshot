using System;
using UnityEngine;

namespace Core.Common {    
    public enum TimeScale {
        Scaled,
        Unscaled
    }

    [Serializable]
    public class FloatRange {
        public float Min;
        public float Max;

        public float GetRandomValue() {
            return UnityEngine.Random.Range(this.Min, this.Max);
        }
    }

    public delegate void SimpleDelegate(object sender, string anyString);
    
    public delegate void EmptyDelegate();

    /// <summary>
    /// Certain COGs and behaviours need data to know where/how to emit an object. This is the core
    /// data package that is passed into those components. It's highly recommended that you only
    /// (re)create EmitterData packages when something has changed and therefore should use a cached
    /// version of this instance whenever possible. This is for efficiency as well as the fact that
    /// this data package is designed for that usage.
    /// </summary>
    public class EmitterData {
        public EmitterData(string entityResourceName, Vector2 position, Vector2 magnitude, object context) {
            if (entityResourceName != null) {
                this.TemplateObject = Resources.Load<GameObject>(entityResourceName);
            }

            this.Position = position;
            this.Magnitude = magnitude;
            this.Context = context;
        }

        public GameObject TemplateObject { get; private set; }

        public Vector2 Position { get; set; }

        public Vector2 Magnitude { get; set; }

        public object Context { get; set; }
    }
}
