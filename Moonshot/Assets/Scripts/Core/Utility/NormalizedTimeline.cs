using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Utility {
    /// <summary>
    /// Provides a virtual "timeline" in which you can specify a series of objects and a time-frame in which they should be
    /// "selected" based on a global time code between 0 to 1. As you scrub through the time line, accessing the ActiveObjects
    /// collection will return a collection of objects that are selected within that time frame.
    /// To update the current time frame, call the Update() method only when you're about to call into ActiveObjects, otherwise it
    /// would be a waste of CPU time.
    /// </summary>
    public class NormalizedTimeline {
        private class TimeEntry {
            public GameObject ReferenceObject;
            public float StartTime;
            public float EndTime;
        }

        private List<TimeEntry> mTimeEntries;
        private List<TimeEntry> mActiveEntries;

        public NormalizedTimeline() {
            mTimeEntries = new List<TimeEntry>();
            mActiveEntries = new List<TimeEntry>();
        }

        public void AddEntry(GameObject referenceObject, float triggerTime, float localDuration) {
            mTimeEntries.Add(new TimeEntry()
            {
                ReferenceObject = referenceObject,
                StartTime = triggerTime,
                EndTime = triggerTime + localDuration
            });

            if (mTimeEntries.Count > 1) {
                SortTimeline();
            }
        }

        public void Reset() {
            mTimeEntries.Clear();
            mActiveEntries.Clear();
        }

        public void Update(float time) {
            RemoveFinishedEntries(time);
            AddActiveEntries(time);
        }

        private static bool IsTimeInRange(TimeEntry entry, float time) {
            return entry.StartTime <= time && entry.EndTime >= time;
        }

        private void RemoveFinishedEntries(float time) {
            for (int i = 0; i < mActiveEntries.Count; i++) {
                if (!IsTimeInRange(mActiveEntries[i], time)) {
                    mActiveEntries.RemoveAt(i--);
                }
            }
        }

        private void AddActiveEntries(float time) {
            foreach (TimeEntry t in mTimeEntries) {
                if (IsTimeInRange(t, time) && !mActiveEntries.Contains(t)) {
                    mActiveEntries.Add(t);
                }
            }
        }

        private void SortTimeline() {
            mTimeEntries.Sort((a, b) => a.StartTime.CompareTo(b.StartTime));
        }

        public IEnumerable<GameObject> ActiveObjects {
            get {
                return mActiveEntries.Select(o => o.ReferenceObject);
            }
        }
    }
}
