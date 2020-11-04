using Core.Common;
using System;
using UnityEngine;

namespace Core.CogsV2 {
    [Serializable]
    public class TriggerData {
        public static TriggerData Empty = new TriggerData();

        public int DataTypeId;
        public string StringData;
        public Transform UnityTransformData;
        public Transform UnityTransformTriggerSource;
        public int IntData;
        public StringIntPair[] NamedProperties;

        /// <summary>
        /// In some cases we are internally passing raw object references around. Use this to
        /// hold the reference and specify the type-casting semantics via DataTypeId (e.g: use
        /// a unique DataTypeId that is well-known to denote how the raw object needs to be cast).
        /// 
        /// This is specified as a property to avoid Unity from exposing it in the editor.
        /// </summary>
        public object RawObjectData { get; set; }
    }
}
