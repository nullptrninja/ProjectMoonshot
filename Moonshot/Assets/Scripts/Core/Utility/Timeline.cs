using System;
using UnityEngine;

namespace Core.Utility {
    /// <summary>
    /// Implements a time line that fires events with static data after specific time marks have elapsed.
    /// In the editor, you specify each time event and what data to send to the callback. The time marks are
    /// in seconds.
    /// This is not at all like NormalizedTimeline. Refer to LightAttenuator for an example on how this is used.
    /// </summary>
    /// <typeparam name="T">The parameter type that is sent as part of the triggering callback. The type must be
    /// serializable via Unity's serializer</typeparam>
    [Serializable]
    public class Timeline<T> {
        private readonly TimeEvent<T>[] mEvents;

        private int mCurrentEventIndex;
        private float mElapsedTime;
        private readonly Action<T> mCallbackAction;

        public Timeline(TimeEvent<T>[] events, Action<T> callback) {
            mEvents = events;
            mCallbackAction = callback;
        }

        public void Update() {
            if (mCurrentEventIndex >= 0 && mCurrentEventIndex < mEvents.Length) {
                // IF there's a next event...
                if (mCurrentEventIndex + 1 < mEvents.Length) {
                    var nextEvent = mEvents[mCurrentEventIndex + 1];
                    if (mElapsedTime >= nextEvent.TimeMark) {
                        mCallbackAction(mEvents[mCurrentEventIndex].Value);
                        mCurrentEventIndex++;
                    }
                }

                // Increment time post-event check so we don't miss the first
                // potential event if it's time mark is lower than the delta time
                mElapsedTime += Time.deltaTime;
            }
        }

    }    
}
