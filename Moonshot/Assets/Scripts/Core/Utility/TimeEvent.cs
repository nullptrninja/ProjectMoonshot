using System;

namespace Core.Utility {
    /// <summary>
    /// Used in conjunction with Timeline, describes a value to be used when a particular time marking
    /// has been hit within the Timeline instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class TimeEvent<T> {
        public T Value;
        public float TimeMark;
    }
}
