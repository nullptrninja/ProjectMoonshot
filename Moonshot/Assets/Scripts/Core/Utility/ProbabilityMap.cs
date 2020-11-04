using System;
using System.Collections.Generic;

namespace Core.Utility {
    /// <summary>
    /// Allows you to build a map of different chances of an object being selected.
    /// </summary>
    public class ProbabilityMap<T> where T : class {
        private class Section<T1> where T1 : class {
            public int Probability;
            public float NormalizedProbabilityCeiling;
            public float NormalizedProbabilityFloor;
            public T1 SelectedObject;
        }

        private List<Section<T>> mSections;
        private int mTotalProbability;

        public ProbabilityMap() {
            mSections = new List<Section<T>>();
            mTotalProbability = 0;
        }

        public void AddProbability(T selectableObject, int probability) {
            mTotalProbability += probability;
            mSections.Add(new Section<T>()
            {
                SelectedObject = selectableObject,
                Probability = probability
            });
        }

        public T SelectRandomObject(int chanceModifier) {
            if (mSections.Count == 0) {
                return null;
            }

            mSections.Sort(new Comparison<Section<T>>((l, r) =>
            {
                // Sort descending
                if (l.Probability < r.Probability) {
                    return 1;
                }
                else if (l.Probability > r.Probability) {
                    return -1;
                }
                else {
                    return 0;
                }
            }));

            ComputeNormalizedProbabilities();
            float random = UnityEngine.Random.Range(0 + chanceModifier, 1001) / 1000f;

            foreach (Section<T> s in mSections) {
                if (s.NormalizedProbabilityFloor >= random && s.NormalizedProbabilityCeiling < random) {
                        return s.SelectedObject;
                }
            }
            
            // Error condition is probably here, if nothing matched, return the choice with the greatest possibility (the first one)
            return mSections[0].SelectedObject;
        }

        private void ComputeNormalizedProbabilities() {
            // Normalize all the probabilities
            for (int i = 0; i < mSections.Count; i++) {
                if (i == 0) {
                    mSections[i].NormalizedProbabilityFloor = 0f;
                }
                else {
                    mSections[i].NormalizedProbabilityFloor = mSections[i - 1].NormalizedProbabilityCeiling;
                }

                mSections[i].NormalizedProbabilityCeiling = mSections[i].NormalizedProbabilityFloor + ((float)mSections[i].Probability / mTotalProbability);
            }
        }
    }
}
