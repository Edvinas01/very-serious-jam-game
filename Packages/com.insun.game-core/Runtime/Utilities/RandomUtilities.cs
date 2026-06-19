using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace InSun.GameCore.Utilities
{
    public static class RandomUtilities
    {
        private static readonly Random Random = new(Guid.NewGuid().GetHashCode());

        /// <returns>
        /// Random direction vector.
        /// </returns>
        public static Vector3 GetRandomDirection()
        {
            var vector = new Vector3(
                GetRandomFloat(min: -1f, max: +1f),
                GetRandomFloat(min: -1f, max: +1f),
                GetRandomFloat(min: -1f, max: +1f)
            );

            vector.Normalize();

            return vector;
        }

        /// <returns>
        /// Random  <see cref="float"/> value which is withing given <paramref name="range"/>.
        /// </returns>
        public static float GetRandomFloat(this Vector2 range)
        {
            return GetRandomFloat(range.x, range.y);
        }

        /// <returns>
        /// Random <see cref="float"/> value between <paramref name="min"/> and <paramref name="max"/> range.
        /// </returns>
        public static float GetRandomFloat(float min = 0f, float max = 1f)
        {
            return (float)(Random.NextDouble() * (max - min) + min);
        }

        /// <returns>
        /// Random <see cref="int"/> value which is withing given <paramref name="range"/>.
        /// </returns>
        public static int GetRandomInt(this Vector2Int range)
        {
            return GetRandomInt(range.x, range.y);
        }

        /// <returns>
        /// Random <see cref="int"/> value between <paramref name="min"/> and <paramref name="max"/> range.
        /// </returns>
        public static int GetRandomInt(int min = int.MinValue, int max = int.MaxValue)
        {
            return Random.Next(min, max);
        }

        /// <returns>
        /// Random element from <paramref name="enumerable"/>.
        /// </returns>
        public static TValue GetRandom<TValue>(this IEnumerable<TValue> enumerable)
        {
            if (TryGetRandom(enumerable, out var value))
            {
                return value;
            }

            throw new Exception("Could not find a random random element");
        }

        /// <returns>
        /// <c>true</c> if a random <paramref name="value"/> is retrieved from given
        /// <paramref name="enumerable"/> or <c>false</c> otherwise.
        /// </returns>
        public static bool TryGetRandom<TValue>(this IEnumerable<TValue> enumerable, out TValue value)
        {
            return TryGetRandom(enumerable.ToList(), out value);
        }

        /// <returns>
        /// <c>true</c> if a random <paramref name="value"/> is retrieved from given
        /// <paramref name="list"/> or <c>false</c> otherwise.
        /// </returns>
        public static bool TryGetRandom<TValue>(this IReadOnlyList<TValue> list, out TValue value)
        {
            if (list.Count == 0)
            {
                value = default;
                return false;
            }

            var index = Random.Next(list.Count);
            value = list[index];
            return true;
        }

        /// <returns>
        /// Random color from given <paramref name="gradient"/>.
        /// </returns>
        public static Color GetRandomColor(Gradient gradient)
        {
            return gradient.Evaluate(GetRandomFloat());
        }

        /// <returns>
        /// Random color.
        /// </returns>
        public static Color GetRandomColor(bool isRandomizeAlpha = true)
        {
            return new Color(
                GetRandomFloat(min: 0f, max: 1f),
                GetRandomFloat(min: 0f, max: 1f),
                GetRandomFloat(min: 0f, max: 1f),
                isRandomizeAlpha ? GetRandomFloat(min: 0f, max: 1f) : 1f
            );
        }
    }
}
