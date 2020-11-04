using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Utility {
    public static class Iterators {
        /// <summary>
        /// Randomizes the given collection. Note that this will materialize the enumerable
        /// internally so take caution when using this against potentially large collections.        
        /// </summary>
        /// <returns>Randomized collection</returns>
        public static IEnumerable<T> AtRandom<T>(this IEnumerable<T> list) {
            var asArray = list.ToArray();

            // Shuffle the array first; we use a Fisher-Yates shuffle here.
            for (var i = asArray.Length - 1; i > 0; --i) {
                var j = UnityEngine.Random.Range(0, i + 1);
                var tmp = asArray[i];
                asArray[i] = asArray[j];
                asArray[j] = tmp;
            }

            return asArray;
        }

        /// <summary>
        /// Picks a specified amount of elements at random from the given collection without duplication.
        /// Note that this will materialize the enumerable internally so take caution
        /// when using this against potentially large collections.
        /// </summary>
        /// <param name="pickCount">Amount of elements to pick</param>
        /// <returns>Subset of elements picked at random</returns>
        public static IEnumerable<T> RandomlyPick<T>(this IEnumerable<T> list, int pickCount) {
            var randomizedList = list.AtRandom();
            return randomizedList.Take(pickCount);
        }

        /// <summary>
        /// Picks one element at random from the given collection.
        /// </summary>        
        /// <returns>One element picked at random</returns>
        public static T RandomlyPickOne<T>(this IEnumerable<T> list) {
            var randomizedList = list.AtRandom();
            return randomizedList.First();
        }

        /// <summary>
        /// Picks a specified amount of elements at random from the given collection without duplication given
        /// a specific predicate function that it must satisfy.
        /// Note that this will materialize the enumerable internally so take caution
        /// when using this against potentially large collections.
        /// </summary>
        /// <param name="pickCount">Amount of elements to pick</param>
        /// <param name="predicate">Prediate function that the element must satisfy in order to be included</param>
        /// <returns>Subset of elements picked at random</returns>
        public static IEnumerable<T> RandomlyPick<T>(this IEnumerable<T> list, int pickCount, Func<T, bool> predicate) {
            var randomizedList = list.AtRandom();
            var picked = new List<T>(pickCount);

            // The alternative to this method is to simply use AtRandom().Where().Take(n) however the
            // Where() causes us to iterate over the entire collection again which may not be great if the
            // collection is sufficiently large and we're trying to save a few cycles.
            foreach (var element in randomizedList) {
                if (predicate(element)) {
                    picked.Add(element);

                    if (picked.Count >= pickCount) {
                        return picked;
                    }
                }
            }

            return picked;
        }
    }
}
