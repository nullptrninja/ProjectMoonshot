using System;

namespace Core.Common {
    /// <summary>
    /// Unity doesn't like to serialize something generic like a KVP but it will
    /// serialize a concrete type based on that KVP. Here we have a bunch of pairs
    /// that we may need most often.
    /// </summary>
    [Serializable]
    public class StringIntPair {
        public string Key;
        public int Value;
    }

    [Serializable]
    public class StringStringPair {
        public string Key;
        public string Value;
    }

    [Serializable]
    public class RowColumnPair {
        public int Row;
        public int Col;
    }

    [Serializable]
    public class IntIntPair {
        public int Key;
        public int Value;
    }
}
