using System;

namespace Core.Utility {
    /// <summary>
    /// Capabilities allows you to use enum flags to manage object capability states. It's not assumed
    /// that a cleared state is equal to zero however, you may override the "cleared" state on construction.
    /// This allows you to think about capabilities in terms of either "what you can do" or "what you-
    /// -cannot do" depending on your preference.
    /// 
    /// Note: Internally, the flags are stored as Int32, therefore you can only use up to 31 cap flags.
    /// We don't check for this condition, so don't fuck this up.
    /// </summary>
    public class Capabilities {
        public const int AllBitsSet = Int32.MaxValue;
        public const int NoBitsSet = 0x00000000;

        private int mCurrentCaps;
        private int mClearedValue;

        public Capabilities(int clearedValue) {
            mClearedValue = clearedValue;
            Clear();
        }

        public void Clear() {
            mCurrentCaps = mClearedValue;
        }
                
        public bool HasCaps(int flags) {
            return (mCurrentCaps & flags) > 0;
        }
        
        public void SetCapsExplicit(int flags) {
            mCurrentCaps = flags;
        }

        public void SetCaps(int caps, bool enabled) {
            if (enabled) {
                mCurrentCaps = mCurrentCaps | caps;
            }
            else {
                mCurrentCaps = mCurrentCaps & (~(caps));
            }
        }

        public void SetCap(int singleCap, bool enabled) {
            int nthBitMask = (int)Math.Log(singleCap, 2);
            if (enabled) {
                mCurrentCaps = mCurrentCaps | (1 << nthBitMask);
            }
            else {
                mCurrentCaps = mCurrentCaps & (~(1 << nthBitMask));
            }
        }
    }
}
