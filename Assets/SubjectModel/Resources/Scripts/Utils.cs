using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SubjectModel
{
    public static class Utils
    {
        public static float Map(float sourceBegin, float sourceEnd, float targetBegin, float targetEnd, float source)
        {
            return (targetEnd - targetBegin) * (source - sourceBegin) / (sourceEnd - sourceBegin) + targetBegin;
        }
        public static Vector3 Vector2To3(Vector2 vec, float z = .0f)
        {
            return new Vector3(vec.x, vec.y, z);
        }

        public static Vector2 Vector3To2(Vector3 vec)
        {
            return new Vector2(vec.x, vec.y);
        }

        public static Vector4 Vector3To4(Vector3 vec, float w = .0f)
        {
            return new Vector4(vec.x, vec.y, vec.z, w);
        }
        
        public static float GetMagnitudeSquare2D(Vector3 a, Vector3 b)
        {
            return GetMagnitudeSquare2D(Vector3To2(a), Vector3To2(b));
        }

        public static float GetMagnitudeSquare2D(Vector3 a, Vector2 b)
        {
            return GetMagnitudeSquare2D(Vector3To2(a), b);
        }

        public static float GetMagnitudeSquare2D(Vector2 a, Vector2 b)
        {
            return (a - b).sqrMagnitude;
        }

        public static int ParseInt(float num)
        {
            return (int) num;
        }

        public static int GetAnimatorState(float velocity)
        {
            if (Math.Abs(velocity) < .05) return 0;
            return velocity > .0f ? 1 : -1;
        }

        public static Vector2 LengthenVector(Vector2 direction, float magnitude)
        {
            return direction / direction.magnitude * magnitude;
        }

        public static Vector2 LengthenArrow(Vector2 from, Vector2 to, float magnitude)
        {
            return LengthenVector(to - from, magnitude) + from;
        }
        public static float GenerateGaussian() {
            float v1;
            float s;
            do {
                v1 = 2.0f * Random.Range(0f, 1f) - 1.0f;
                var v2 = 2.0f * Random.Range(0f, 1f) - 1.0f;
                s = v1 * v1 + v2 * v2;
            } while (s >= 1.0f || Math.Abs(s) < .0000001f);
            s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);
            return v1 * s;
        }
        public static float GenerateGaussian(float mean, float standardDeviation) {
            return mean + GenerateGaussian() * standardDeviation;
        }

        public static float GenerateGaussian(float mean, float standardDeviation, float max)
        {
            float x;
            do
            {
                x = GenerateGaussian(mean, standardDeviation);
            } while (x < mean - max || x > mean + max);

            return x;
        }

        public static Vector2 GetRotatedForce(Vector2 force, Vector2 reference, float magnitude, double radian = Math.PI / 2)
        {
            var origin = LengthenVector(reference - force, magnitude);
            return new Vector2((float) (origin.x * Math.Cos(radian) - origin.y * Math.Sin(radian)), 
                               (float) (origin.x * Math.Sin(radian) + origin.y * Math.Cos(radian))) + force;
        }
    }
}