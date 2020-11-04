using System;
using System.Collections.Generic;

namespace Core.Utility {
    public static class MiscUtils {
        public static bool HasFlag(int flags, int targetFlag) {
            return (flags & targetFlag) > 0;
        }

        public static bool IsSameSign(float v1, float v2) {
            return v1 < 0f && v2 < 0f || v1 >= 0f && v2 >= 0f;
        }

        public static float GetSign(float v) {
            return v >= 0f ? 1f : -1f;
        }

        public static float Clamp(float actualValue, float min, float max) {
            if (actualValue < min) {
                return min;
            }
            else if (actualValue > max) {
                return max;
            }
            else {
                return actualValue;
            }
        }

        public static int Clamp(int actualValue, int min, int max) {
            if (actualValue < min) {
                return min;
            }
            else if (actualValue > max) {
                return max;
            }
            else {
                return actualValue;
            }
        }

        public static int PickOneOrNegativeOne() {
            return UnityEngine.Random.Range(0, 2) == 1 ? 1 : -1;
        }

        public static bool IsAlmostEqualTo(this float me, float other, float tolerance = 0.001f) {
            var diff = Math.Abs(me * tolerance);
            return Math.Abs(me - other) <= diff;
        }

        /// <summary>
        /// Selects items from an array of possible choices based on whether the
        /// value in the inclusion map at the same index is True or False. Returns
        /// a condensed array of references to the selected items.
        /// </summary>
        /// <typeparam name="T">Type of elements to select</typeparam>
        /// <param name="possibleItems">List of items that can be included in the condensed list</param>
        /// <param name="inclusionMap">Map of values of whether or not to include the item at the same index</param>
        /// <returns>Condensed list of items</returns>
        public static T[] Pick<T>(T[] possibleItems, bool[] inclusionMap) {
            List<T> choices = new List<T>(possibleItems.Length);
            for (int i = 0; i < possibleItems.Length; i++) {
                if (inclusionMap[i]) {
                    choices.Add(possibleItems[i]);
                }
            }

            return choices.ToArray();
        }

        /// <summary>
        /// Creates a shallow copy of the input array with its elements rearranged in
        /// a random positions.
        /// </summary>
        /// <typeparam name="T">Type of the elements in the array</typeparam>
        /// <param name="array">Input array</param>
        /// <returns>Reference copy to array</returns>
        public static T[] RandomizeArrayOrder<T>(T[] array) {
            int len = array.Length;
            for (int i = 0; i < len; i++) {
                int index0 = UnityEngine.Random.Range(0, len - 1);
                int index1 = UnityEngine.Random.Range(0, len - 1);
                T tmp = array[index0];
                array[index0] = array[index1];
                array[index1] = tmp;
            }

            T[] copy = new T[len];
            Array.Copy(array, copy, len);

            return copy;
        }
    }
}