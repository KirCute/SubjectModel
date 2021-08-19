using System;
using System.Linq;

namespace SubjectModel.Scripts.Chemistry
{
    /**
     * <summary>
     * 元素
     * 此类的每一个实例代表一种元素。
     * 原则上该类的实例只能从Elements.json中读取，由资源加载器创建。
     * 禁止在游戏运行过程中创建该类的实例对象，或改变已有实例对象的成员变量值。
     * </summary>
     */
    [Serializable]
    public class Element
    {
        public const int Acid = 0;
        public const int Bases = 1;
        public const int Solid = 0;
        public const int Aqua = 1;
        public const int Gas = 2;

        public string symbol;
        public int[] valences;
        public float[][] potential;
        public float[][] combination;
        public Buff[] buffType;
        public float[][] buffParam;
        public int[][] state;

        public bool HasValence(int valence)
        {
            return valences.Any(v => v == valence);
        }

        public int GetIndex(int valence)
        {
            return Array.IndexOf(valences, valence);
        }

        public bool CanBeOxidized(int index)
        {
            return index >= 0 && index < valences.Length - 1;
        }

        public bool CanBeReduced(int index)
        {
            return CanBeOxidized(index - 1);
        }

        public bool GetOxidizedPotential(int index, int properties, out float p)
        {
            p = float.PositiveInfinity;
            if (CanBeOxidized(index)) p = potential[properties][index];
            return CanBeOxidized(index);
        }

        public bool GetReducedPotential(int index, int properties, out float p)
        {
            return GetOxidizedPotential(index - 1, properties, out p);
        }

        public float GetOxidizedCoefficient(int index)
        {
            return valences[index + 1] - valences[index];
        }

        public float GetReducedCoefficient(int index)
        {
            return valences[index] - valences[index - 1];
        }

        public int GetCharge(int index, int properties)
        {
            var c = combination[properties][index];
            return c > .0f
                ? valences[index] - Utils.ToInteger(c * 2f)
                : valences[index] + Utils.ToInteger(c);
        }
    }
}