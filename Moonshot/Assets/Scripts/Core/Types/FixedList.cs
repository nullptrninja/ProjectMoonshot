using System;

namespace Core.Types {
    /// <summary>
    /// FixedList contains the usability of List<T> but with a fixed capacity. Best time to use this is when
    /// we need to add or fetch a large amount of data but our capacity upper-limit is known.
    /// 
    /// Performance gains over List<T> over 1 million calls:
    /// - 40% faster Adds (8.01ms vs 13.34ms)
    /// - 50% faster Fetches (1ms vs 2ms)
    /// 
    /// Performance losses over List<T> over 1000 calls:
    /// - 600% slower in removal (6s vs 1s)
    /// 
    /// </summary>
    /// <typeparam name="T">Reference-type to store</typeparam>
    public class FixedList<T> where T : class {
        private const int DefaultCapacity = 20;

        private readonly int mCapacity;
        private T[] mList;
        private int mInsertAtNextIndex;

        public int Length { get { return Math.Max(mInsertAtNextIndex, 0); } }

        public T this[int index] {
            get { return mList[index]; }
        }

        public FixedList() :
            this(DefaultCapacity) { }

        public FixedList(int capacity) {
            mList = new T[capacity];
            mCapacity = capacity;
            mInsertAtNextIndex = 0;
        }

        public void Add(T obj) {
            if (mInsertAtNextIndex < mCapacity) {
                mList[mInsertAtNextIndex++] = obj;
            }
        }

        public bool AddAndConfirm(T obj) {
            if (mInsertAtNextIndex < mCapacity) {
                mList[mInsertAtNextIndex++] = obj;                
                return true;
            }

            return false;
        }

        public void Remove(T obj) {
            for (int i = 0; i < mList.Length; i++) {
                if (mList[i].Equals(obj)) {
                    ShiftLeftOverwrite(i);
                    mInsertAtNextIndex--;
                }
            }
        }

        public void RemoveAt(int index) {
            if (index < 0 || index >= mCapacity) {
                throw new IndexOutOfRangeException();
            }

            if (mList[index] != null) {
                ShiftLeftOverwrite(index);
                mInsertAtNextIndex--;
            }
        }

        public void Clear() {
            for (int i = 0; i < mList.Length; i++) {
                mList[i] = null;
            }
            mInsertAtNextIndex = 0;
        }

        private void ShiftLeftOverwrite(int overwritingIndex) {
            for (int i = overwritingIndex + 1; i < mList.Length; i++) {
                mList[i - 1] = mList[i];
            }
            mList[mList.Length - 1] = null;
        }
    }
}
