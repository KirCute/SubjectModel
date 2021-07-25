using System;
using System.Collections.Generic;
using System.Linq;
using Bolt;
using UnityEngine;

namespace SubjectModel
{
    [RequireComponent(typeof(Variables))]
    public class BuffRenderer : MonoBehaviour
    {
        public static float MotiveDamageCoefficient = 30.0f;
        public static float ThermalDamageCoefficient = 50.0f;
        public static float MinimumDamagePotential = .4f;

        private List<IBuff> buffs;
        private IList<IonStack> stain;

        private void Start()
        {
            buffs = new List<IBuff>();
            stain = new List<IonStack>();
        }

        private void Update()
        {
            for (var i = 0; i < buffs.Count; i++)
            {
                var buff = buffs[i];
                buff.Update(gameObject);
                if (buff.AfterDelay(gameObject)) buff.UpdateAfterDelay(gameObject);
                if (!buff.Ended(gameObject)) continue;
                Remove(buff);
                i--;
            }

            for (var i = 0; i < stain.Count; i++)
            {
                stain[i].DropTime -= Time.deltaTime;
                if (!(stain[i].DropTime <= .0f)) continue;
                Remove(stain[i]);
                i--;
            }
        }

        public void Clear()
        {
            foreach (var buff in buffs) buff.Destroy(gameObject);
            buffs.Clear();
            stain.Clear();
        }

        private void Add(IBuff buff)
        {
            buff.Appear(gameObject);
            buffs.Add(buff);
        }

        private void Remove(IBuff buff)
        {
            buff.Destroy(gameObject);
            buffs.Remove(buff);
        }

        private void Remove(IonStack ion)
        {
            Debug.LogFormat("Remove: {0}({1})", ion.Element.symbol, ion.Element.valences[ion.Index]);
            stain.Remove(ion);
        }

        private void Apply(IBuff buff)
        {
            foreach (var b in buffs.Where(b => b.GetType() == buff.GetType()))
            {
                if (b.GetLevel() > buff.GetLevel()) return;

                if (Math.Abs(b.GetLevel() - buff.GetLevel()) < .0001f &&
                    b.GetTotalTime() - b.GetRemainedTime() <= buff.GetTotalTime())
                {
                    b.Append(buff.GetTotalTime());
                    return;
                }

                b.LevelUp(gameObject, buff.GetLevel());
                b.Append(buff.GetTotalTime());
            }

            Add(buff);
        }

        private void Cleanup()
        {
            for (var i = 0; i < stain.Count; i++)
                if (stain[i].Amount < .1f ||
                    (stain[i].Element == Elements.O && stain[i].Index == 0))
                {
                    Remove(stain[i]);
                    i--;
                }
        }

        private void Insert(IonStack stack, int properties)
        {
            switch (stack.Element.state[properties][stack.Index])
            {
                case Element.Solid:
                    return;
                case Element.Gas:
                    var origin = GetComponent<Variables>().declarations.Get<float>("Health");
                    GetComponent<Variables>().declarations
                        .Set("Health", origin - stack.Amount * MotiveDamageCoefficient);
                    if (stack.Element.buffType[stack.Index] != Buff.Empty)
                        Apply((IBuff) Activator.CreateInstance(
                            DrugDictionary.GetTypeOfBuff(stack.Element.buffType[stack.Index]),
                            stack.Amount * stack.Element.buffParam[stack.Index][0],
                            stack.Concentration * stack.Element.buffParam[stack.Index][1]));
                    return;
                case Element.Aqua:
                    Debug.LogFormat("Insert: {0}({1}) {2}mol {3}mol/L", stack.Element.symbol,
                        stack.Element.valences[stack.Index], stack.Amount, stack.Concentration);
                    if (stack.Element.buffType[stack.Index] != Buff.Empty)
                        Apply((IBuff) Activator.CreateInstance(
                            DrugDictionary.GetTypeOfBuff(stack.Element.buffType[stack.Index]),
                            stack.Amount * stack.Element.buffParam[stack.Index][0],
                            stack.Concentration * stack.Element.buffParam[stack.Index][1]));
                    foreach (var ion in stain.Where(ion => ion.Element == stack.Element && ion.Index == stack.Index))
                    {
                        ion.Amount += stack.Amount;
                        ion.DropTime = 10.0f;
                        return;
                    }

                    stack.DropTime = 10.0f;
                    stain.Add(stack);
                    return;
            }
        }

        private void React(int properties)
        {
            React(properties, new List<IonStack>());
        }

        private void React(int properties, ICollection<IonStack> ignore)
        {
            var maxPotential = float.NegativeInfinity;
            IonStack oxidizer = null;
            foreach (var drug in stain.Where(ionStack => !ignore.Contains(ionStack)))
            {
                if (!drug.Element.GetReducedPotential(drug.Index, properties, out var potential)) continue;
                if (potential <= maxPotential) continue;
                maxPotential = potential;
                oxidizer = drug;
            }

            if (oxidizer == null) return;
            //Debug.Log("Oxidizer: " + oxidizer.Element.symbol + " " + oxidizer.Element.valences[oxidizer.Index] +
            //          " " + oxidizer.Amount);

            IonStack reducer = null;
            var minPotential = float.PositiveInfinity;
            foreach (var drug in stain.Where(drug => !Equals(drug, oxidizer)))
            {
                if (!drug.Element.GetOxidizedPotential(drug.Index, properties, out var potential)) continue;
                if (potential >= maxPotential || potential > minPotential) continue;
                minPotential = potential;
                reducer = drug;
            }

            if (reducer == null)
            {
                ignore.Add(oxidizer);
                React(properties, ignore);
                return;
            }

            //Debug.Log("Reducer: " + reducer.Element.symbol + " " + reducer.Element.valences[reducer.Index] +
            //          " " + reducer.Amount);

            Debug.LogFormat("Reaction happens: {0}({1} -> {2}) ↓   {3}({4} -> {5}) ↑",
                oxidizer.Element.symbol,
                oxidizer.Element.valences[oxidizer.Index], oxidizer.Element.valences[oxidizer.Index - 1],
                reducer.Element.symbol,
                reducer.Element.valences[reducer.Index], reducer.Element.valences[reducer.Index + 1]
            );

            var m = oxidizer.Element.GetReducedCoefficient(oxidizer.Index);
            var n = reducer.Element.GetOxidizedCoefficient(reducer.Index);
            float reactionAmount;
            if (oxidizer.Amount * m >= reducer.Amount * n)
            {
                Insert(new IonStack
                {
                    Element = reducer.Element, Amount = reducer.Amount,
                    Index = reducer.Index + 1, Concentration = 1f
                }, properties);
                Insert(new IonStack
                {
                    Element = oxidizer.Element, Amount = reducer.Amount / m * n,
                    Index = oxidizer.Index - 1, Concentration = 1f
                }, properties);
                reactionAmount = (maxPotential - minPotential) * (reducer.Amount + reducer.Amount / m * n) / 2f;
                oxidizer.Amount -= reducer.Amount / m * n;
                reducer.Amount = .0f;
            }
            else
            {
                Insert(new IonStack
                {
                    Element = reducer.Element, Amount = oxidizer.Amount / n * m,
                    Index = reducer.Index + 1, Concentration = 1f
                }, properties);
                Insert(new IonStack
                {
                    Element = oxidizer.Element, Amount = oxidizer.Amount,
                    Index = oxidizer.Index - 1, Concentration = 1f
                }, properties);
                reactionAmount = (maxPotential - minPotential) * (oxidizer.Amount + oxidizer.Amount / n * m) / 2f;
                reducer.Amount -= oxidizer.Amount / n * m;
                oxidizer.Amount = .0f;
            }

            reactionAmount -= MinimumDamagePotential;
            if (reactionAmount >= 0)
            {
                var origin = GetComponent<Variables>().declarations.Get<float>("Health");
                GetComponent<Variables>().declarations
                    .Set("Health", origin - reactionAmount * ThermalDamageCoefficient);
            }

            Cleanup();
        }

        public void Register(DrugStack drug)
        {
            foreach (var ion in drug.Ions)
            {
                Insert(ion, drug.Properties);
                React(drug.Properties);
            }
        }
    }
}