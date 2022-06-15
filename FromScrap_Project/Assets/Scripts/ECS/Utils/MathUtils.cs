using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace VertexFragment
{
    /// <summary>
    /// Collection of utility math operations.
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        /// Determines if the two float values are equal to each other within the range of the epsilon.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static bool FloatEquals(float a, float b, float epsilon = 0.000001f)
        {
            return Mathf.Abs(a - b) <= epsilon;
        }
        
        public static float3 AddSprayToDir(float3 dir, float spray, int r)
        {
            var random = Random.CreateFromIndex((uint) r);
            var randomDir = random.NextFloat3Direction();

            randomDir = new float3(
                math.clamp(math.sign(randomDir.x) * (math.abs(randomDir.x) - math.abs(dir.x)), -1, 1),
                math.clamp(math.sign(randomDir.y) * (math.abs(randomDir.y) - math.abs(dir.y)), -1, 1),
                math.clamp(math.sign(randomDir.z) * (math.abs(randomDir.z) - math.abs(dir.z)), -1, 1));

            return math.normalize(dir + randomDir * (spray / 90));
        }

        /// <summary>
        /// Returns true if the specified float value is <c>0.0</c> (or within the given epsilon).
        /// </summary>
        /// <param name="a"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static bool IsZero(float a, float epsilon = 0.0000001f)
        {
            return FloatEquals(a, 0.0f, epsilon);
        }

        /// <summary>
        /// Returns true if all components of the specified vector are <c>0.0</c> (or within the given epsilon).
        /// </summary>
        /// <param name="v"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static bool IsZero(float3 v, float epsilon = 0.000001f)
        {
            return (IsZero(v.x, epsilon) && IsZero(v.y, epsilon) && IsZero(v.z, epsilon));
        }

        /// <summary>
        /// Sets the components of the specified vector to <c>0.0</c> if they are less than the given epsilon.
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static float3 ZeroOut(float3 vec, float epsilon = 0.001f)
        {
            vec.x = math.abs(vec.x) < epsilon ? 0.0f : vec.x;
            vec.y = math.abs(vec.y) < epsilon ? 0.0f : vec.y;
            vec.z = math.abs(vec.z) < epsilon ? 0.0f : vec.z;

            return vec;
        }

        public enum LoopType
        {
            None,
            Loop,
            PingPong
        }
        
        public static float LoopValue(LoopType type, float value, float period)
        {
            switch (type)
            {
                case LoopType.None:
                    return value;
                case LoopType.Loop:
                    return value % period;
                case LoopType.PingPong:
                    return ((int) (value / period) % 2f > 0) ? value % period : period - value % period;
                default:
                    return value;
            }
        }
        
        public static int Factorial(int f)
        {
            if(f == 0)
                return 1;
            else
                return f * Factorial(f-1); 
        }
    }
}
