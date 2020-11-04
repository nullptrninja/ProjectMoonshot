using Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Core.Systems.Threading {
    /// <summary>
    /// Provides a queueing-enabled multi-threaded work processor and orchestrator. Given a number of work to perform
    /// off of the main thread, the ThreadPool processes these work items as well as takes requests for new
    /// work onto a queue for flow control. The pool has a fixed limit on how many concurrent threads
    /// can be running at a given time as well as how many items can be in the backlog, so be sure to account
    /// for the possibility that your job may not be able to be processed in a given frame.
    /// 
    /// The ThreadPool instance itself lives on the main thread and isn't thread safe. As such, it should only
    /// be called to do work from the main thread. Your work item functions should never attempt to enqueue more
    /// work on to the ThreadPool while they are running off-thread.
    /// </summary>
    public class ThreadPool {
        private readonly int mMaxConcurrency;
        private readonly int mMaxQueueSize;

        private Thread[] mThreads;
        private Worker[] mWorkers;
        private Queue<ThreadedWork> mPendingTasks;              // Investigate if we need a lighter weight queue later.        
        private bool mEnableWorkerCallbacks;
        private bool mEnableDoWorkImmediatelyOnCompletion;

        public ThreadPool(int maxThreads, int maxQueueSize) {
            Precondition.Check(maxThreads > 0, "Cannot have less than 1 maxThread");
            Precondition.Check(maxQueueSize > 0, "Cannot have less than 1 maxQueueSize");

            mMaxConcurrency = maxThreads;
            mMaxQueueSize = maxQueueSize;
            Init();
        }

        /// <summary>
        /// Enqueues work to be done. Note that this must be called from the main thread,
        /// it is not thread-safe. The pool only supports a maximum number of work that can
        /// be done concurrently as well as work that is pending. If both pipelines are at
        /// capacity then the function will return false and the work won't be enqueued.
        /// </summary>
        /// <returns>True if the work was enqueued.</returns>
        public bool EnqueueWork(ThreadedWork workRequest) {
            if (mPendingTasks.Count < mMaxQueueSize) {
                mPendingTasks.Enqueue(workRequest);

                // Signal that there's work afoot. This may or may not dequeue the
                // next item however. In either case, we return true since we could
                // only have hoped to enqueue it.
                TryToPickupNextWork();
                return true;
            }

            return false;
        }

        public void ClearPendingWork() {
            mPendingTasks.Clear();
        }

        /// <summary>
        /// Re-starts the ThreadPool after a Stop() has been called. You only need to call this if
        /// Stop() was called previously, otherwise the ThreadPool starts automatically on instantiation.
        /// 
        /// Note that calling this will clear all pending work in the queue.
        /// </summary>
        public void Start() {
            ClearPendingWork();
            mEnableWorkerCallbacks = true;
            mEnableDoWorkImmediatelyOnCompletion = true;
        }
                
        /// <summary>
        /// Attempts to stop further work from being processed as well as callbacks from happening on work
        /// that is in the process of completing. If a work item has already finished, this will try to prevent
        /// the original work submitter from being signaled as well. It is recommended that you avoid calling
        /// Stop() right at the tailend of scene where a callback might occur against an object that no longer exists.
        /// 
        /// Note that nothing except clearing of the pending work queue is guaranteed, this makes a best effort to
        /// prevent callbacks and future tasks from being processed.
        /// </summary>
        public void Stop() {
            ClearPendingWork();
            mEnableWorkerCallbacks = false;
            mEnableDoWorkImmediatelyOnCompletion = false;
        }

        /// <summary>
        /// Note that this is a complete shutdown of the threadpool as it will abort all threads.
        /// Once called, you need to re-create the threadpool. If you wish to re-use the threadpool
        /// call Stop() instead.
        /// </summary>
        public void HardStop() {
            ClearPendingWork();
            for (int i = 0; i < mThreads.Length; i++) {
                //mThreads[i].Abort();
                mWorkers[i].Stop();
            }
        }
                
        private void Init() {
            mThreads = new Thread[mMaxConcurrency];
            mWorkers = new Worker[mMaxConcurrency];

            for (int i = 0; i < mThreads.Length; i++) {
                mWorkers[i] = new Worker(this.WorkerFinishedHandler);
                mThreads[i] = new Thread(new ThreadStart(mWorkers[i].DoWork)) { IsBackground = true };

                mThreads[i].Start();
            }

            mPendingTasks = new Queue<ThreadedWork>(mMaxQueueSize);
            mEnableWorkerCallbacks = true;
            mEnableDoWorkImmediatelyOnCompletion = true;
        }
        
        private void TryToPickupNextWork() {
            if (mPendingTasks.Count > 0) {
                var worker = FindNextAvailableWorker();
                if (worker != null) {
                    var workItem = mPendingTasks.Dequeue();
                    worker.AssignWork(workItem);
                }
            }
        }

        private Worker FindNextAvailableWorker() {
            return mWorkers.FirstOrDefault(w => !w.IsWorking);            
        }

        private void WorkerFinishedHandler(Worker worker, ThreadedWork finishedWorkItem) {
            // TODO: This might be a bug.
            // Investigate if we need to marshal this back onto the main thread if it's not already
            // on the main thread. Otherwise we break the restriction that we cannot enqueue new work from
            // off-thread

            // Signal the original caller that the requested work is complete. We guarantee
            // that the just-finished worker instance is immediately ready for the next task
            // once you've reached this callback.
            if (mEnableWorkerCallbacks) {
                finishedWorkItem.FinishedWorkCallback();
            }

            // Now that a worker has been freed up, we can tell the thread pool to try
            // and pick up the next task.
            if (mEnableDoWorkImmediatelyOnCompletion) {
                TryToPickupNextWork();
            }
        }
    }

    class Worker {
        private ManualResetEvent mSignal;
        private ThreadedWork mActiveWorkItem;
        private Action<Worker, ThreadedWork> mOnFinishedCallback;
        private bool mIsWorking;
        private bool mKeepAlive;

        public bool IsWorking { get; private set; }

        public Worker(Action<Worker, ThreadedWork> onFinishedCallback) {
            mSignal = new ManualResetEvent(false);
            mIsWorking = false;
            mOnFinishedCallback = onFinishedCallback;
            mKeepAlive = true;
        }

        public void Stop() {
            mKeepAlive = false;
        }

        // Called from single-threaded context
        public void AssignWork(ThreadedWork workToDo) {
            Precondition.IsNotNull(workToDo);
            
            if (!mIsWorking) {
                mIsWorking = true;
                mActiveWorkItem = workToDo;

                // Start work processing
                mSignal.Set();
            }
        }
        
        public void DoWork() {
            try {
                while (mKeepAlive) {
                    mSignal.WaitOne();

                    mActiveWorkItem.DoWork();
                    
                    var finishedWorkItem = mActiveWorkItem;
                    mActiveWorkItem = null;

                    // Signal the thread pool that this work is complete. We will then
                    // call the work item's callback method to signal the original caller.
                    mOnFinishedCallback(this, finishedWorkItem);

                    // Wait for next work
                    mSignal.Reset();
                    mIsWorking = false;
                }
            }
            catch (ThreadAbortException t) {
                Debug.LogWarning("A worker thread has been aborted in the thread pool: " + t.Message);
            }
            finally {
                mActiveWorkItem = null;
                mSignal.Close();
            }
        }
    }
}
