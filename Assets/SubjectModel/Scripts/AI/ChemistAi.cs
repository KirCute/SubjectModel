using System.Collections.Generic;
using SubjectModel.Scripts.InventorySystem;
using SubjectModel.Scripts.Subject.Chemistry;
using UnityEngine;

namespace SubjectModel.Scripts.AI
{
    /**
     * <summary>用于构建炼金术敌人AI的工具类，其中的方法多为Bolt调用</summary>
     */
    public static class ChemistAi
    {
        /**
         * <summary>
         * 生成炼金术敌人的物品栏状态
         * <param name="self">敌人的GameObject</param>
         * <example>
         * 使用如下代码给敌人的物品栏中添加一个新的玻封药品
         * <code>
         * inventory.Add(new DrugStack
         * (
         *     "玻封药品在物品栏中的名称",
         *     new List<IonStack>
         *     {
         *         new IonStack
         *         {
         *             Element = Elements.Get("第一种粒子的元素符号"), Index = Elements.Get("第一种粒子的元素符号").GetIndex(化合价),
         *             Amount = 物质的量f, Concentration = 浓度f
         *         },
         *         第二种粒子
         *     },
         *     药品的环境（Element.Acid或Element.Bases),
         *     数量
         * ));
         * </code>
         * </example>
         * </summary>
         */
        public static void GenerateChemist(GameObject self)
        {
            if (!self.TryGetComponent<Inventory>(out var inventory)) return; //获取物品栏inventory
            inventory.Add(new Sling()); //添加弹弓作为武器
            inventory.Add(new DrugStack
            (
                "CuCl2",
                new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.Get("Cu"), Index = Elements.Get("Cu").GetIndex(2),
                        Amount = 1f, Concentration = 1f
                    },
                    new IonStack
                    {
                        Element = Elements.Get("Cl"), Index = Elements.Get("Cl").GetIndex(-1),
                        Amount = 2f, Concentration = 3f
                    }
                },
                Element.Acid,
                10000
            )); //添加玻封药品CuCl2用于对玩家造成伤害
            inventory.Add(new DrugStack
            (
                "CoSO4",
                new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.Get("Co"), Index = Elements.Get("Co").GetIndex(2),
                        Amount = 1f, Concentration = 1f
                    }
                },
                Element.Acid,
                10000
            )); //添加玻封药品CuCl2用于治疗
            inventory.SwitchTo(0); //装备位于物品栏0号位的弹弓
        }
    }
}