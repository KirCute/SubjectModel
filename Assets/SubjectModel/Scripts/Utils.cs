using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SubjectModel.Scripts
{
    public static class Utils
    {
        /**
         * <summary>映射</summary>
         */
        public static float Map(float sourceBegin, float sourceEnd, float targetBegin, float targetEnd, float source)
        {
            return (targetEnd - targetBegin) * (source - sourceBegin) / (sourceEnd - sourceBegin) + targetBegin;
        }
        
        /**
         * <summary>
         * 将二维向量扩展为三维向量
         * 若不给定z坐标，则设为0F
         * </summary>
         */
        public static Vector3 Vector2To3(Vector2 vec, float z = .0f)
        {
            return new Vector3(vec.x, vec.y, z);
        }

        /**
         * <summary>
         * 将三维向量降为为二维坐标
         * 舍弃z坐标
         * </summary>
         */
        public static Vector2 Vector3To2(Vector3 vec)
        {
            return new Vector2(vec.x, vec.y);
        }

        /**
         * <summary>
         * 将三维向量扩展为四维向量
         * 若不给定w坐标，则设为0F
         * </summary>
         */
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
        
        /**
         * <summary>
         * 得到二维向量差的模长
         * 用于Bolt
         * </summary>
         */
        public static float GetMagnitudeSquare2D(Vector2 a, Vector2 b)
        {
            return (a - b).sqrMagnitude;
        }

        /**
         * <summary>
         * 得到float变量的整数部分
         * 用于Bolt
         * </summary>
         */
        public static int ParseInt(float num)
        {
            return (int) num;
        }

        /**
         * <summary>
         * 通过运动速度得到动画状态
         * 用于Bolt
         * </summary>
         * <param name="velocity">运动速度</param>
         */
        public static int GetAnimatorState(float velocity)
        {
            if (Math.Abs(velocity) < .05) return 0;
            return velocity > .0f ? 1 : -1;
        }

        /**
         * <summary>放缩向量</summary>
         * <param name="direction">源向量，与最终向量方向相同</param>
         * <param name="magnitude">模长</param>
         * <returns>目标向量，与源向量共线，模长等于magnitude</returns>
         */
        public static Vector2 LengthenVector(Vector2 direction, float magnitude)
        {
            return direction.Equals(Vector2.zero) ? Vector2.zero : direction / direction.magnitude * magnitude;
        }

        /**
         * <summary>放缩矢量</summary>
         * <param name="from">源矢量始位置</param>
         * <param name="to">源矢量末位置</param>
         * <param name="magnitude">模长</param>
         * <returns>目标矢量，始位置与源矢量相同，模长等于magnitude</returns>
         */
        public static Vector2 LengthenArrow(Vector2 from, Vector2 to, float magnitude)
        {
            return LengthenVector(to - from, magnitude) + from;
        }
        public static float GenerateGaussian()
        {
            float v1;
            float s;
            do
            {
                v1 = 2.0f * Random.Range(0f, 1f) - 1.0f;
                var v2 = 2.0f * Random.Range(0f, 1f) - 1.0f;
                s = v1 * v1 + v2 * v2;
            } while (s >= 1.0f || Math.Abs(s) < .00001f);

            s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);
            return v1 * s;
        }

        public static float GenerateGaussian(float mean, float standardDeviation)
        {
            return mean + GenerateGaussian() * standardDeviation;
        }

        /**
         * <summary>生成符合正态分布的随机数</summary>
         * <param name="mean">均值</param>
         * <param name="standardDeviation">方差</param>
         * <param name="max">结果与均值的最大差距</param>
         */
        public static float GenerateGaussian(float mean, float standardDeviation, float max)
        {
            float x;
            do
            {
                x = GenerateGaussian(mean, standardDeviation);
            } while (x < mean - max || x > mean + max);

            return x;
        }

        /**
         * <summary>旋转向量</summary>
         * <param name="vec">源向量</param>
         * <param name="radian">弧度（逆时针），不给定时为PI/2</param>
         */
        public static Vector2 GetRotatedVector(Vector2 vec, double radian = Math.PI / 2)
        {
            return new Vector2((float) (vec.x * Math.Cos(radian) - vec.y * Math.Sin(radian)),
                (float) (vec.x * Math.Sin(radian) + vec.y * Math.Cos(radian)));
        }

        /**
         * <summary>
         * 动态中心生成
         * 用于敌人移动的向心轨迹生成
         * </summary>
         * <param name="focus">中心位置</param>
         * <param name="reference">敌人位置</param>
         * <param name="magnitude">玄学参数，看着调</param>
         * <param name="radian">弧度，传入一个变量，可以使用时间(s)与移动速度计算得出</param>
         * <returns>移动方向，应当与远离玩家的运动向量相加</returns>
         */
        public static Vector2 GetRotatedFocus(Vector2 focus, Vector2 reference, float magnitude,
            double radian = Math.PI / 2)
        {
            var origin = LengthenVector(reference - focus, magnitude);
            return GetRotatedVector(origin, radian) + focus;
        }

        /**
         * <summary>是否接近整数</summary>
         */
        public static bool IsInteger(float num)
        {
            num = Math.Abs(num);
            var floor = Math.Floor(num);
            return num - floor < .1 || floor + 1.0 - num < .1;
        }

        /**
         * <summary>将float四舍五入转化为整数</summary>
         */
        public static int ToInteger(float num)
        {
            num = Math.Abs(num);
            var floor = Math.Floor(num);
            if (num - floor < .5f) return (int) floor;
            return (int) floor + 1;
        }
    }
}