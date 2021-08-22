using System;
using System.Linq;

namespace SubjectModel.Scripts.Subject.Chemistry
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

        public string symbol; //元素符号
        public int[] valences; //v项，表示由低到高的所有化合价
        public float[][] potential; //2 * (v-1)项，一维表示环境（酸为0碱为1，下同），二维第n项表示从第n个化学价升至第n+1个化学价电势的抬升

        public float[][]
            combination; //2 * v项，一维表示环境，二维第n位为正时表示第n个化合价的该种元素一般粒子的氧结合数（平均每个中心原子），为负时绝对值表示氢结合数，为零时不结合任何原子

        public Buff[] buffType; //v项，第n项表示第n个价态提供的效果
        public float[][] buffParam; //2 * ?项，一维表示环境，二维表示效果的参数（无效果时为DefaultParam）
        public int[][] state; //2 * v项，一维表示环境，二维第n位表示第n个价态此种元素构成物质的状态

        /**
         * <summary>
         * 判断元素是否具有某化合价
         * <param name="valence">要判断的化合价</param>
         * <returns>是否具有此化合价</returns>
         * </summary>
         */
        public bool HasValence(int valence)
        {
            return valences.Any(v => v == valence);
        }

        /**
         * <summary>
         * 得到某化合价的索引
         * 例如Mn有+II +IV +VII价
         * 则+II价的索引为0，+IV的索引为1，+VII价的索引为2
         * <param name="valence">要判断的化合价</param>
         * <returns>此化合价的索引</returns>
         * </summary>
         */
        public int GetIndex(int valence)
        {
            return Array.IndexOf(valences, valence);
        }

        /**
         * <summary>
         * 判断元素的某化合价是否能被氧化
         * <param name="index">化合价的索引</param>
         * <returns>能否被氧化</returns>
         * </summary>
         */
        public bool CanBeOxidized(int index)
        {
            return index >= 0 && index < valences.Length - 1;
        }

        /**
         * <summary>
         * 判断元素的某化合价是否能被还原
         * <param name="index">化合价的索引</param>
         * <returns>能否被还原</returns>
         * </summary>
         */
        public bool CanBeReduced(int index)
        {
            return CanBeOxidized(index - 1);
        }

        /**
         * <summary>
         * 得到某化合价被氧化到下一价时抬升的电势
         * <param name="index">化合价的索引</param>
         * <param name="properties">环境</param>
         * <param name="p">输出，抬升的电势</param>
         * <returns>能否被氧化</returns>
         * </summary>
         */
        public bool GetOxidizedPotential(int index, int properties, out float p)
        {
            p = float.PositiveInfinity;
            if (CanBeOxidized(index)) p = potential[properties][index];
            return CanBeOxidized(index);
        }

        /**
         * <summary>
         * 得到某化合价被还原到上一价时降落的电势
         * <param name="index">化合价的索引</param>
         * <param name="properties">环境</param>
         * <param name="p">输出，降落的电势</param>
         * <returns>能否被还原</returns>
         * </summary>
         */
        public bool GetReducedPotential(int index, int properties, out float p)
        {
            return GetOxidizedPotential(index - 1, properties, out p);
        }

        /**
         * <summary>
         * 得到某化合价被氧化时氧化剂的系数
         * 注意应当先判断该化合价能否被氧化
         * <param name="index">化合价的索引</param>
         * <returns>氧化剂的系数</returns>
         * </summary>
         */
        public float GetOxidizedCoefficient(int index)
        {
            return valences[index + 1] - valences[index];
        }

        /**
         * <summary>
         * 得到某化合价被还原时还原剂的系数
         * 注意应当先判断该化合价能否被还原
         * <param name="index">化合价的索引</param>
         * <returns>还原剂的系数</returns>
         * </summary>
         */
        public float GetReducedCoefficient(int index)
        {
            return valences[index] - valences[index - 1];
        }

        /**
         * <summary>
         * 得到某化合价一般粒子的带电荷数
         * <param name="index">化合价的索引</param>
         * <param name="properties">环境</param>
         * <returns>带电荷数</returns>
         * </summary>
         */
        public int GetCharge(int index, int properties)
        {
            var c = combination[properties][index];
            return c > .0f
                ? valences[index] - Utils.ToInteger(c * 2f)
                : valences[index] + Utils.ToInteger(c);
        }
    }
}