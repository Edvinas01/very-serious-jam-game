using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Utilities
{
    public static class MathUtilities
    {
        /// <returns>
        /// Clamped angle within <c>[min, max]</c> range with wrap-around.
        /// </returns>
        public static float ClampAngle(this float angle, float min, float max)
        {
            if (angle < 0f)
            {
                angle = 360 + angle;
            }

            if (angle > 180f)
            {
                return Mathf.Max(angle, 360 + min);
            }

            return Mathf.Min(angle, max);
        }

        /// <returns>
        /// <c>true</c> if given <paramref name="vector"/> is a number and not infinity, <c>false</c> otherwise.
        /// </returns>
        public static bool IsValid(this Vector3 vector)
        {
            if (float.IsNaN(vector.x) || float.IsInfinity(vector.x))
            {
                return false;
            }

            if (float.IsNaN(vector.y) || float.IsInfinity(vector.y))
            {
                return false;
            }

            if (float.IsNaN(vector.z) || float.IsInfinity(vector.z))
            {
                return false;
            }

            return true;
        }

        /// <returns>
        /// <c>true</c> if given <paramref name="rotation"/> is a number and not infinity, <c>false</c> otherwise.
        /// </returns>
        public static bool IsValid(this Quaternion rotation)
        {
            if (float.IsNaN(rotation.x) || float.IsInfinity(rotation.x))
            {
                return false;
            }

            if (float.IsNaN(rotation.y) || float.IsInfinity(rotation.y))
            {
                return false;
            }

            if (float.IsNaN(rotation.z) || float.IsInfinity(rotation.z))
            {
                return false;
            }

            if (float.IsNaN(rotation.w) || float.IsInfinity(rotation.w))
            {
                return false;
            }

            return true;
        }

        /// <returns>
        /// <c>true</c> if given <paramref name="value"/> is a number and not infinity, <c>false</c> otherwise.
        /// </returns>
        public static bool IsValid(this float value)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// More accurate Lerp function, useful in physics calculations.
        /// For more info, see <a href="https://www.rorydriscoll.com/2016/03/07/frame-rate-independent-damping-using-lerp">Frame Rate Independent Damping using Lerp</a>
        /// </summary>
        public static Vector3 ExponentialLerp(Vector3 from, Vector3 to, float deltaTime, float smoothing = 0.75f)
        {
            return new Vector3(
                ExponentialLerp(from: from.x, to: to.x, deltaTime: deltaTime, smoothing: smoothing),
                ExponentialLerp(from: from.y, to: to.y, deltaTime: deltaTime, smoothing: smoothing),
                ExponentialLerp(from: from.z, to: to.z, deltaTime: deltaTime, smoothing: smoothing)
            );
        }

        /// <summary>
        /// More accurate Lerp function, useful in physics calculations.
        /// For more info, see <a href="https://www.rorydriscoll.com/2016/03/07/frame-rate-independent-damping-using-lerp">Frame Rate Independent Damping using Lerp</a>
        /// </summary>
        public static float ExponentialLerp(float from, float to, float deltaTime, float smoothing = 0.75f)
        {
            return Mathf.Lerp(from, to, 1f - Mathf.Exp(-smoothing * deltaTime));
        }
    }
}
