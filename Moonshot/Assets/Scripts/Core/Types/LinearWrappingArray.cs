namespace Core.Types {
    /// <summary>
    /// A fixed-sized array that adds linearly and will wrap around to the beginning if the next add-index exceeds
    /// our capacity. This is a very specialized type used for pooling; as such once capacity is reached, no new
    /// instances can be added to the array, however access to each object is provided in a continuously wrapping loop.
    /// </summary>
    /// <typeparam name="T">Type to store</typeparam>
    public class LinearWrappingArray<T> where T : class {
        private T[] mList;
        private int mNextAddIndex;
        private int mNextRemoveIndex;
        private int mCount;

        public LinearWrappingArray(int capacity) {
            mList = new T[capacity];
            mNextAddIndex = 0;
            mNextRemoveIndex = 0;
            mCount = 0;
        }

        /// <summary>
        /// Advances the state of the array to the next element for adding and fetching.
        /// </summary>
        public void MoveNext() {
            if (mNextAddIndex < mList.Length) {
                mNextAddIndex++;
                mCount++;

                // If remove pointer is stuck at the end, wrap it now
                if (mNextRemoveIndex >= mList.Length) {
                    mNextRemoveIndex = 0;
                }
            }

            // Wrap the add pointer around if we can
            if (mNextAddIndex >= mList.Length && mNextRemoveIndex > 0) {
                mNextAddIndex = 0;
            }
        }

        /// <summary>
        /// Adds the object to the array into the "newest" object slot. If there's
        /// no more room, the object is not added and false is returned;
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if the object was added</returns>
        public bool Add(T obj) {
            if (mNextAddIndex < mList.Length) {
                mList[mNextAddIndex++] = obj;
                mCount++;

                // If remove pointer is stuck at the end, wrap it now
                if (mNextRemoveIndex >= mList.Length) {
                    mNextRemoveIndex = 0;
                }

                return true;
            }

            // Wrap the add pointer around if we can
            if (mNextAddIndex >= mList.Length && mNextRemoveIndex > 0) {
                mNextAddIndex = 0;
            }

            return false;
        }

        /// <summary>
        /// Returns the next object in the array; if the end is reached, the index is looped back. You can
        /// also think of this as "get the oldest object in the array".
        /// </summary>
        /// <returns></returns>
        public T FetchNext() {
            if (mCount == 0) {
                return null;
            }

            T obj = null;
            if (mNextRemoveIndex < mList.Length) {
                obj = mList[mNextRemoveIndex++];
                mCount--;

                // If add pointer is stuck at the end, wrap it now
                if (mNextAddIndex >= mList.Length) {
                    mNextAddIndex = 0;
                }
            }

            // Wrap the remove pointer around if we can
            if (mNextRemoveIndex >= mList.Length && mNextAddIndex > 0) {
                mNextRemoveIndex = 0;
            }

            return obj;
        }

        public int Count {
            get {
                return mCount;
            }
        }
    }
}
