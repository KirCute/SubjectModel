using System;
using System.Collections.Generic;
using System.Linq;
using Bolt;
using UnityEngine;

namespace SubjectModel.Scripts.Chemistry
{
    /**
     * <summary>
     * 处理作战单位沾染情况和效果的脚本。
     * 需要手动挂载在作战单位上。
     * 除非作战单位确实既免疫一切效果又无视炼金术攻击（此种情况几乎不存在），所有作战单位都必须挂载此脚本。
     * 若作战单位只免疫效果而不免疫炼金术攻击，则只需将immune设为true即可。
     * </summary>
     */
    [RequireComponent(typeof(Variables))]
    public class BuffRenderer : MonoBehaviour
    {
        public static float StainTime = 10.0f; //沾染粒子保持沾染的时间
        public static float MotiveDamageCoefficient = 30.0f; //动力伤害系数，反应生成气体时与气体量相乘作为动力伤害，最终伤害由气体动力伤害与防御相间得出

        public static float
            ThermalDamageCoefficient = 50.0f; //热力伤害系数，氧化还原反应大于MinimumDamagePotential的部分与之相成作为热力伤害，最终伤害等于热力伤害

        public static float MinimumDamagePotential = .4f; //造成热力伤害的最小电势差
        public readonly List<IBuff> Buffs = new List<IBuff>(); //正在作用于作战单位的效果
        public readonly IList<IonStack> Stain = new List<IonStack>(); //正沾染于作战单位的粒子
        public bool immune;

        /// <value>是否免疫效果</value>
        private void Update()
        {
            for (var i = 0; i < Buffs.Count; i++)
            {
                var buff = Buffs[i];
                buff.Update(gameObject); //效果的逐帧更新
                if (!buff.Ended(gameObject)) continue; //（含以下两句）处理效果的终止
                Remove(buff);
                i--;
            }

            for (var i = 0; i < Stain.Count; i++) //更新沾染粒子的脱落
            {
                Stain[i].DropTime -= Time.deltaTime;
                if (!(Stain[i].DropTime <= .0f)) continue;
                Remove(Stain[i]);
                i--;
            }
        }

        /**
         * <summary>
         * 添加某种效果（包含堆叠处理）
         * <param name="buff">要添加的效果</param>
         * </summary>
         */
        private void Add(IBuff buff)
        {
            foreach (var b in Buffs.Where(b => b.GetType() == buff.GetType())) //寻找所有的同名效果，理论上只会执行1次或0次
            {
                if (b.GetLevel() > buff.GetLevel()) return; //若新效果等级更低，不做任何操作

                if (buff.GetLevel() - b.GetLevel() < .0001f) //若新效果与原效果同级
                {
                    if (b.GetTotalTime() - b.GetRemainedTime() <= buff.GetTotalTime())
                        b.Append(buff.GetTotalTime()); //若新效果剩余时间更长则延时
                    return;
                }

                b.LevelUp(gameObject, buff.GetLevel()); //（和下句）若新效果等级更高，直接覆盖原效果
                b.Append(buff.GetTotalTime());
                return;
            }

            if (immune) return; //没有找到同名效果，若不免疫，则添加新效果
            Buffs.Add(buff);
            buff.Appear(gameObject);
        }

        /**
         * <summary>
         * 终止某个效果
         * <param name="buff">要终止的效果</param>
         * </summary>
         */
        private void Remove(IBuff buff)
        {
            buff.Destroy(gameObject);
            Buffs.Remove(buff);
        }

        /**
         * <summary>清除所有效果</summary>
         */
        public void ClearBuff()
        {
            foreach (var buff in Buffs) buff.Destroy(gameObject);
            Buffs.Clear();
        }

        /**
         * <summary>
         * 沾染某种物质
         * 此方法会考虑物质的状态，会处理效果的添加
         * <param name="stack">要添加的粒子堆</param>
         * <param name="properties">添加时的环境，用来得到物质状态和处理复分解</param>
         * </summary>
         */
        private void Add(IonStack stack, int properties)
        {
            switch (stack.Element.state[properties][stack.Index]) //判断沾染物质的状态
            {
                case Element.Solid:
                    return; //若为固态，不做处理
                case Element.Gas: //若为气态
                    var v = GetComponent<Variables>().declarations;
                    var damage = Math.Max(
                        stack.Amount * MotiveDamageCoefficient -
                        (v.IsDefined("Defence") ? v.Get<float>("Defence") : 0f), 0f); //通过减法公式计算动力伤害
                    v.Set("Health", v.Get<float>("Health") - damage); //扣血
                    if (stack.Element.buffType[stack.Index] != Buff.Empty) //若物质存在效果
                        Add((IBuff) Activator.CreateInstance(
                            DrugDictionary.GetTypeOfBuff(stack.Element.buffType[stack.Index]),
                            stack.Amount * stack.Element.buffParam[stack.Index][0],
                            stack.Concentration * stack.Element.buffParam[stack.Index][1])); //添加物质所带效果
                    return;
                case Element.Aqua:
                    //Debug.LogFormat("Insert: {0}({1}) {2}mol {3}mol/L", stack.Element.symbol,
                    //    stack.Element.valences[stack.Index], stack.Amount, stack.Concentration);
                    if (stack.Element.buffType[stack.Index] != Buff.Empty) //若物质存在效果
                        Add((IBuff) Activator.CreateInstance(
                            DrugDictionary.GetTypeOfBuff(stack.Element.buffType[stack.Index]),
                            stack.Amount * stack.Element.buffParam[stack.Index][0],
                            stack.Concentration * stack.Element.buffParam[stack.Index][1])); //添加物质所带效果
                    foreach (var ion in Stain.Where(ion => ion.Element == stack.Element && ion.Index == stack.Index)
                    ) //寻找相同粒子进行堆叠，理论上只会执行1次或0次
                    {
                        ion.Amount += stack.Amount;
                        ion.DropTime = StainTime; //更新脱落时间
                        return;
                    }

                    stack.DropTime = StainTime; //（和下句）沾染粒子
                    Stain.Add(stack);
                    return;
            }
        }

        /**
         * <summary>
         * 清除某种沾染粒子
         * <param name="ion">要清除的粒子</param>
         * </summary>
         */
        private void Remove(IonStack ion)
        {
            //Debug.LogFormat("Remove: {0}({1})", ion.Element.symbol, ion.Element.valences[ion.Index]);
            Stain.Remove(ion);
        }

        /**
         * <summary>清除所有沾染粒子</summary>
         */
        public void ClearStain()
        {
            Stain.Clear();
        }

        /**
         * <summary>
         * 清理总量过少的沾染粒子和水
         * 在每次对沾染进行大规模，不确定的更改（比如反应）后都应调用。
         * </summary>
         */
        private void Cleanup()
        {
            for (var i = 0; i < Stain.Count; i++) //枚举已沾染粒子
                if (Stain[i].Amount < .1f || //物质的量小于0.1
                    (Stain[i].Element == Elements.Get("O") && Stain[i].Index == 0)) //为水
                {
                    Remove(Stain[i]); //清除
                    i--;
                }
        }

        /**
         * <summary>
         * 反应的起始
         * 禁止除此方法以外的方法调用带ignore参数的React方法。
         * <param name="properties">反应发生的环境</param>
         * </summary>
         */
        private void React(int properties)
        {
            React(properties, new List<IonStack>());
        }

        /**
         * 反应递归方法
         * 禁止React(int)以外的方法调用。
         * <param name="properties">反应发生的环境</param>
         * <param name="ignore">经判断被认为无效的氧化剂</param>
         */
        private void React(int properties, ICollection<IonStack> ignore)
        {
            //找到氧化性最强且没有被忽略的粒子
            var maxPotential = float.NegativeInfinity;
            IonStack oxidizer = null;
            foreach (var drug in Stain.Where(ionStack => !ignore.Contains(ionStack))) //对所有未被忽略的粒子
            {
                if (!drug.Element.GetReducedPotential(drug.Index, properties, out var potential)) continue; //不能被还原，跳过
                if (potential <= maxPotential) continue; //氧化性比已找到的最强氧化剂弱，跳过
                maxPotential = potential; //（和下句）更新已找到的最强氧化剂
                oxidizer = drug;
            }

            if (oxidizer == null) return; //若没有合适的氧化剂，反应体系稳定，反应终止。
            //Debug.Log("Oxidizer: " + oxidizer.Element.symbol + " " + oxidizer.Element.valences[oxidizer.Index] +
            //          " " + oxidizer.Amount);

            //找到还原性最强的还原剂
            IonStack reducer = null;
            var minPotential = float.PositiveInfinity;
            foreach (var drug in Stain.Where(drug => !Equals(drug, oxidizer))) //对不是氧化剂的所有粒子
            {
                if (!drug.Element.GetOxidizedPotential(drug.Index, properties, out var potential)) continue; //不可被氧化，跳过
                if (potential >= maxPotential || potential > minPotential) continue; //无法被氧化剂氧化，或比已找到的最强还原剂弱，跳过
                minPotential = potential; //（和下句）更新已找到的最强还原剂
                reducer = drug;
            }

            if (reducer == null) //若没有找到合适的还原剂
            {
                ignore.Add(oxidizer); //忽略此氧化剂
                React(properties, ignore); //重新开始寻找合适的氧化剂
                return;
            }

            //Debug.Log("Reducer: " + reducer.Element.symbol + " " + reducer.Element.valences[reducer.Index] +
            //          " " + reducer.Amount);

            /*
            Debug.LogFormat("Reaction happens: {0}({1} -> {2}) ↓   {3}({4} -> {5}) ↑",
                oxidizer.Element.symbol,
                oxidizer.Element.valences[oxidizer.Index], oxidizer.Element.valences[oxidizer.Index - 1],
                reducer.Element.symbol,
                reducer.Element.valences[reducer.Index], reducer.Element.valences[reducer.Index + 1]
            );
            */

            //若程序执行到此处，说明反应可以发生且正在发生
            var m = oxidizer.Element.GetReducedCoefficient(oxidizer.Index); //氧化剂降价数，用于配平
            var n = reducer.Element.GetOxidizedCoefficient(reducer.Index); //还原剂升价数，用于配平
            float reactionAmount; //反应量（反应前后电势变化），从氧化剂和还原剂中不足者的物质的量得出，用于计算热力伤害
            if (oxidizer.Amount * m >= reducer.Amount * n) //还原剂不足或刚好反应，以下是反应主体
            {
                Add(new IonStack
                {
                    Element = reducer.Element, Amount = reducer.Amount,
                    Index = reducer.Index + 1, Concentration = 1f
                }, properties); //沾染氧化产物
                Add(new IonStack
                {
                    Element = oxidizer.Element, Amount = reducer.Amount / m * n,
                    Index = oxidizer.Index - 1, Concentration = 1f
                }, properties); //沾染还原产物
                reactionAmount = (maxPotential - minPotential) * (reducer.Amount + reducer.Amount / m * n) / 2f; //计算反应量
                oxidizer.Amount -= reducer.Amount / m * n; //氧化剂消耗
                reducer.Amount = .0f; //还原剂消耗（殆尽）
            }
            else //氧化剂不足，以下是反应主体
            {
                Add(new IonStack
                {
                    Element = reducer.Element, Amount = oxidizer.Amount / n * m,
                    Index = reducer.Index + 1, Concentration = 1f
                }, properties); //沾染氧化产物
                Add(new IonStack
                {
                    Element = oxidizer.Element, Amount = oxidizer.Amount,
                    Index = oxidizer.Index - 1, Concentration = 1f
                }, properties); //沾染还原产物
                reactionAmount =
                    (maxPotential - minPotential) * (oxidizer.Amount + oxidizer.Amount / n * m) / 2f; //计算反应量
                reducer.Amount -= oxidizer.Amount / n * m; //还原剂消耗
                oxidizer.Amount = .0f; //氧化剂消耗（殆尽）
            }

            //以下是热力伤害的计算部分
            reactionAmount -= MinimumDamagePotential; //除去MinimumDamagePotential部分
            if (reactionAmount >= 0) //对剩余部分
            {
                var origin = GetComponent<Variables>().declarations.Get<float>("Health");
                GetComponent<Variables>().declarations
                    .Set("Health", origin - reactionAmount * ThermalDamageCoefficient); //直接扣血
            }
        }

        /**
         * <summary>
         * 复分解的处理部分，在反应结束后调用。
         * 游戏中的复分解只考虑与H+或OH-结合生成气体或沉淀的情况。
         * <param name="properties">反应体系环境</param>
         * </summary>
         */
        private void DoubleReplace(int properties)
        {
            for (var i = 0; i < Stain.Count; i++)
            {
                if (Stain[i].Element.state[properties][Stain[i].Index] == Element.Aqua) continue; //跳过在当前环境下依然为可溶的粒子
                var stack = Stain[i]; //（和下面三句）重新添加这种粒子，以便在Add中被过滤掉
                Remove(stack);
                i--;
                Add(stack, properties);
            }
        }

        /**
         * <summary>
         * 沾染的入口函数
         * 游戏中的沾染分“粒子沾染，氧化还原，复分解”三步骤，在此处可以体现出来。
         * <param name="drug">沾染的药品</param>
         * </summary>
         */
        public void Register(DrugStack drug)
        {
            foreach (var ion in drug.Ions) //解包为粒子
            {
                Add(ion, drug.Properties); //沾染粒子
                React(drug.Properties); //氧化还原
                DoubleReplace(drug.Properties); //复分解
                Cleanup(); //清理无效粒子
            }
        }
    }
}