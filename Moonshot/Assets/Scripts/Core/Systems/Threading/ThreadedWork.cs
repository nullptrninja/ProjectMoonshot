using Core.Common;
using System;

namespace Core.Systems.Threading {
    /// <summary>
    /// ThreadedWork represents a "job" that needs to be done as well as who gets notified
    /// when the work is complete. Derive your work class from this base abstract type in
    /// order to submit it into the ThreadPool.
    /// </summary>
    public abstract class ThreadedWork {

        public Action FinishedWorkCallback { get; protected set; }

        public ThreadedWork(Action finishedWorkCallback) {
            Precondition.IsNotNull(finishedWorkCallback);

            this.FinishedWorkCallback = finishedWorkCallback;
        }

        public abstract void DoWork();
    }
}
