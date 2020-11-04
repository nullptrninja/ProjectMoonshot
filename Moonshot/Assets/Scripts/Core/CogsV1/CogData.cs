using System;
using UnityEngine;

namespace Core.CogsV1 {
    /// <summary>
    /// Wraps a grouping of possible data sources for Cog input/output
    /// </summary>    
    [Serializable]
    public class CogData {
        [SerializeField]
        public float FloatData;

        [SerializeField]
        public int IntData;

        [SerializeField]
        public string StringData;

        [SerializeField]
        public object ObjectData;

        [SerializeField]
        public Vector2 VectorData;

        public CogData() {
            this.FloatData = 0f;
            this.IntData = 0;
            this.StringData = null;
            this.ObjectData = null;
            this.VectorData = Vector2.zero;
        }

        public CogData(int intData) {
            this.IntData = intData;
        }

        public CogData(float floatData) {
            this.FloatData = floatData;
        }

        public CogData(string strData) {
            this.StringData = strData;
        }

        public CogData(object objectData) {
            this.ObjectData = objectData;
        }

        public CogData(Vector2 vectorData) {
            this.VectorData = vectorData;
        }
    }
}
