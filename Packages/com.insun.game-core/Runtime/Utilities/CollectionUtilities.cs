using System;
using System.Collections.Generic;

namespace InSun.GameCore.Utilities
{
    public static class CollectionUtilities
    {
        private static readonly Random Random = new(Guid.NewGuid().GetHashCode());

        /// <returns>
        /// <c>true</c> if a random <paramref name="index"/> is retrieved from given
        /// <paramref name="list"/> or <c>false</c> otherwise.
        /// </returns>
        public static bool TryGetRandomIndex<TValue>(this IReadOnlyList<TValue> list, out int index)
        {
            if (list.Count == 0)
            {
                index = 0;
                return false;
            }

            index = Random.Next(list.Count);
            return true;
        }

        /// <summary>
        /// Shuffle given list in place.
        /// </summary>
        public static BufferedList<TValue> Shuffle<TValue>(this BufferedList<TValue> list)
        {
            var remaining = list.Count;
            while (remaining > 1)
            {
                remaining--;

                var index = Random.Next(remaining + 1);
                (list[index], list[remaining]) = (list[remaining], list[index]);
            }

            return list;
        }

        /// <summary>
        /// Shuffle given list in place.
        /// </summary>
        public static IList<TValue> Shuffle<TValue>(this IList<TValue> list)
        {
            var remaining = list.Count;
            while (remaining > 1)
            {
                remaining--;

                var index = Random.Next(remaining + 1);
                (list[index], list[remaining]) = (list[remaining], list[index]);
            }

            return list;
        }

        /// <summary>
        /// Shuffle given span in place.
        /// </summary>
        public static Span<TValue> Shuffle<TValue>(this Span<TValue> span)
        {
            var remaining = span.Length;
            while (remaining > 1)
            {
                remaining--;

                var index = Random.Next(remaining + 1);
                (span[index], span[remaining]) = (span[remaining], span[index]);
            }

            return span;
        }
    }
}
