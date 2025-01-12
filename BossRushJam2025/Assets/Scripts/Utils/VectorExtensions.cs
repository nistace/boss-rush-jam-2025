using UnityEngine;

namespace Utils
{
    public enum EAxis
    {
        X = 0,
        Y = 1,
        Z = 2,
    }

    public static class VectorExtensions
    {
        public static Vector2 ToVector2(this Vector3 vector, EAxis ignored_axis = EAxis.Z)
        {
            return ignored_axis switch
            {
                EAxis.X => new Vector2(vector.y, vector.z),
                EAxis.Y => new Vector2(vector.x, vector.z),
                EAxis.Z => new Vector2(vector.x, vector.y),
                _ => Vector2.zero,
            };
        }
        public static Vector3 ToVector3(this Vector2 vector, EAxis ignored_axis = EAxis.Z)
        {
            return ignored_axis switch
            {
                EAxis.X => new Vector3(0f, vector.x, vector.y),
                EAxis.Y => new Vector3(vector.x, 0f, vector.y),
                EAxis.Z => new Vector3(vector.x, vector.y, 0f),
                _ => Vector3.zero,
            };
        }
    }
}