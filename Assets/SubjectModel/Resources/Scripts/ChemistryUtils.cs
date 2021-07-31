using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SubjectModel
{
    public enum Buff
    {
        Empty,
        Slowness,
        Poison,
        Curing,
        Ghost,
        Corrosion,
        Rapid
    }

    public static class Elements
    {
        //private static readonly float[] DefaultParam = {1f, 1f};

        public static IList<Element> Dic; /* = new List<Element>
        {
            new Element("H")
            {
                valences = new[] {0, 1},
                combination = new[]
                {
                    new[] {2f, .0f},
                    new[] {2f, 1f}
                },
                potential = new[]
                {
                    new[] {.0f},
                    new[] {.0f}
                },
                buffType = new[] {Buff.Empty, Buff.Corrosion},
                buffParam = new[] {DefaultParam, new[] {0.25f, 400f}},
                state = new[]
                {
                    new[] {Element.Gas, Element.Aqua},
                    new[] {Element.Gas, Element.Aqua},
                }
            },

            new Element("O")
            {
                valences = new[] {-2, -1, 0},
                combination = new[]
                {
                    new[] {-2f, -1f, 2f},
                    new[] {-1f, -1f, 2f}
                },
                potential = new[]
                {
                    new[] {1.77f, 0.68f},
                    new[] {0.87f, -0.08f}
                },
                buffType = new[] {Buff.Empty, Buff.Empty, Buff.Empty},
                buffParam = new[] {DefaultParam, DefaultParam, DefaultParam},
                state = new[]
                {
                    new[] {Element.Aqua, Element.Aqua, Element.Gas},
                    new[] {Element.Aqua, Element.Aqua, Element.Gas},
                }
            },

            new Element("P")
            {
                valences = new[] {-3, 3, 5},
                combination = new[]
                {
                    new[] {-4f, 3f, 4f},
                    new[] {-3f, 3f, 4f}
                },
                potential = new[]
                {
                    new[] {-0.565f, -0.276f},
                    new[] {-2.6f, -1.05f}
                },
                buffType = new[] {Buff.Ghost, Buff.Empty, Buff.Empty},
                buffParam = new[] {new[] {10f, 2f}, DefaultParam, DefaultParam},
                state = new[]
                {
                    new[] {Element.Aqua, Element.Aqua, Element.Aqua},
                    new[] {Element.Gas, Element.Aqua, Element.Aqua}
                }
            },

            new Element("Cl")
            {
                valences = new[] {-1, 0},
                combination = new[]
                {
                    new[] {.0f, 2f},
                    new[] {.0f, 2f}
                },
                potential = new[]
                {
                    new[] {1.358f},
                    new[] {1.358f}
                },
                buffType = new[] {Buff.Empty, Buff.Poison},
                buffParam = new[] {DefaultParam, new[] {10f, 1f}},
                state = new[]
                {
                    new[] {Element.Aqua, Element.Gas},
                    new[] {Element.Aqua, Element.Gas},
                }
            },

            new Element("K")
            {
                valences = new[] {1},
                combination = new[]
                {
                    new[] {.0f},
                    new[] {.0f}
                },
                potential = new[]
                {
                    new float[] { },
                    new float[] { }
                },
                buffType = new[] {Buff.Empty},
                buffParam = new[] {DefaultParam},
                state = new[]
                {
                    new[] {Element.Aqua},
                    new[] {Element.Aqua},
                }
            },

            new Element("Mn")
            {
                valences = new[] {2, 4, 7},
                combination = new[]
                {
                    new[] {.0f, 2f, 4f},
                    new[] {1f, 2f, 4f}
                },
                potential = new[]
                {
                    new[] {1.23f, 1.70f},
                    new[] {-0.05f, 0.59f}
                },
                buffType = new[] {Buff.Empty, Buff.Empty, Buff.Empty},
                buffParam = new[] {DefaultParam, DefaultParam, DefaultParam},
                state = new[]
                {
                    new[] {Element.Aqua, Element.Solid, Element.Aqua},
                    new[] {Element.Solid, Element.Solid, Element.Aqua}
                }
            },

            new Element("Fe")
            {
                valences = new[] {2, 3, 6},
                combination = new[]
                {
                    new[] {.0f, .0f, 4f},
                    new[] {1f, 1.5f, 4f}
                },
                potential = new[]
                {
                    new[] {0.771f, 2.2f},
                    new[] {-0.56f, 0.72f}
                },
                buffType = new[] {Buff.Rapid, Buff.Slowness, Buff.Empty},
                buffParam = new[] {new[] {3f, 3f}, new[] {3f, 3f}, DefaultParam},
                state = new[]
                {
                    new[] {Element.Aqua, Element.Aqua, Element.Aqua},
                    new[] {Element.Solid, Element.Solid, Element.Aqua}
                }
            },

            new Element("Co")
            {
                valences = new[] {2, 3, 4},
                combination = new[]
                {
                    new[] {.0f, .0f, 2f},
                    new[] {1f, 1.5f, 2f}
                },
                potential = new[]
                {
                    new[] {1.82f, 1.416f},
                    new[] {0.17f, 0.62f}
                },
                buffType = new[] {Buff.Curing, Buff.Empty, Buff.Empty},
                buffParam = new[] {new[] {1f, 12.5f}, DefaultParam, DefaultParam},
                state = new[]
                {
                    new[] {Element.Aqua, Element.Aqua, Element.Solid},
                    new[] {Element.Solid, Element.Solid, Element.Solid}
                }
            },

            new Element("Cu")
            {
                valences = new[] {2, 3},
                combination = new[]
                {
                    new[] {.0f, 1F},
                    new[] {1f, 2F}
                },
                potential = new[]
                {
                    new[] {1.8f},
                    new[] {0.65f} // Off-standard
                },
                buffType = new[] {Buff.Poison, Buff.Empty},
                buffParam = new[] {new[] {2f, 17.5f}, DefaultParam},
                state = new[]
                {
                    new[] {Element.Aqua, Element.Aqua},
                    new[] {Element.Solid, Element.Solid}
                }
            }
        };
        //*/

        public static Element Get(string symbol)
        {
            return Dic.FirstOrDefault(e => e.symbol == symbol);
        }
    }

    public static class DrugDictionary
    {
        private static readonly Dictionary<Buff, Type> BuffDictionary = new Dictionary<Buff, Type>();
        private static readonly Dictionary<Buff, string> BuffName = new Dictionary<Buff, string>();
        private static readonly Dictionary<Buff, Vector3> BuffColor = new Dictionary<Buff, Vector3>();

        public static Type GetTypeOfBuff(Buff buff)
        {
            if (BuffDictionary.Count != 0) return BuffDictionary[buff];
            BuffDictionary.Add(Buff.Slowness, typeof(DrugEffect.IIIFe));
            BuffDictionary.Add(Buff.Poison, typeof(DrugEffect.IICu));
            BuffDictionary.Add(Buff.Curing, typeof(DrugEffect.IICo));
            BuffDictionary.Add(Buff.Ghost, typeof(DrugEffect.PIII));
            BuffDictionary.Add(Buff.Corrosion, typeof(DrugEffect.H));
            BuffDictionary.Add(Buff.Rapid, typeof(DrugEffect.IIFe));
            return BuffDictionary[buff];
        }

        public static string GetName(Buff buff)
        {
            if (BuffName.Count != 0) return BuffName[buff];
            BuffName.Add(Buff.Empty, "空效果");
            BuffName.Add(Buff.Slowness, "缓慢");
            BuffName.Add(Buff.Poison, "中毒");
            BuffName.Add(Buff.Curing, "治疗");
            BuffName.Add(Buff.Ghost, "P(III)");
            BuffName.Add(Buff.Corrosion, "腐蚀");
            BuffName.Add(Buff.Rapid, "急速");
            return BuffName[buff];
        }

        public static string GetName(IonStack stack)
        {
            return GetName(stack.Element.buffType[stack.Index]);
        }

        public static Vector3 GetColor(Buff buff)
        {
            if (BuffColor.Count != 0) return BuffColor[buff];
            BuffColor.Add(Buff.Empty, new Vector3(1f, 1f, 1f));
            BuffColor.Add(Buff.Slowness, new Vector3(0.5f, 0.5f, 0.5f));
            BuffColor.Add(Buff.Poison, new Vector3(0.0f, 0.5f, 0.0f));
            BuffColor.Add(Buff.Curing, new Vector3(1.0f, 0.125f, 0.375f));
            BuffColor.Add(Buff.Ghost, new Vector3(0.25f, 0.75f, 0.375f));
            BuffColor.Add(Buff.Corrosion, new Vector3(1.0f, 0.875f, 0.0f));
            BuffColor.Add(Buff.Rapid, new Vector3(0.75f, 0.75f, 0.75f));
            return BuffColor[buff];
        }

        public static Vector3 GetColor(IonStack stack)
        {
            return GetColor(stack.Element.buffType[stack.Index]);
        }
    }

    public class IonStack
    {
        private const int MaxPartner = 10;

        public Element Element;
        public int Index;
        public float Amount;
        public float Concentration;
        public float DropTime;

        public string GetSymbol(int properties)
        {
            if (Element.valences[Index] == 0)
                return $"{Element.symbol}{Utils.ToInteger(Element.combination[properties][Index])}";
            var fCombination = Element.combination[properties][Index];
            var iPartner = FindPartner(fCombination);
            var iCombination = Utils.ToInteger(fCombination * iPartner);
            var partner = iPartner == 1 ? "" : iPartner.ToString();
            var combination = iCombination == 1 ? "" : iCombination.ToString();
            var charge = Element.GetCharge(Index, properties) * iPartner;
            var chargeStr = charge == 0
                ? ""
                : Math.Abs(charge) == 1
                    ? $"{(charge >= 0 ? "+" : "-")}"
                    : $"{Math.Abs(charge)}{(charge >= 0 ? "+" : "-")}";
            return iCombination == 0
                ? $"{Element.symbol} {chargeStr}"
                : fCombination > .0f
                    ? $"{Element.symbol}{partner}O{combination} {chargeStr}"
                    : $"{Element.symbol}{partner}H{combination} {chargeStr}";
        }

        private static int FindPartner(float num)
        {
            for (var i = 1; i < MaxPartner; i++)
                if (Utils.IsInteger(num * i))
                    return i;
            return 1;
        }
    }

    public class DrugStack : IItemStack, IFiller
    {
        public readonly string Tag;
        public readonly IList<IonStack> Ions;
        public readonly int Properties;
        private int count;

        public DrugStack(string tag, IList<IonStack> ions, int properties, int count)
        {
            Tag = tag;
            Ions = ions;
            Properties = properties;
            this.count = count;
        }

        public void Merge(IItemStack item)
        {
            count += ((DrugStack) item).count;
        }

        public IItemStack Fetch(int c)
        {
            if (c > count) c = count;
            var ions = Ions.Select(ion => new IonStack
                    {Element = ion.Element, Index = ion.Index, Amount = ion.Amount, Concentration = ion.Concentration})
                .ToList();
            count -= c;
            return new DrugStack(Tag, ions, Properties, c);
        }

        public int GetCount()
        {
            return count;
        }

        public void CountAppend(int c)
        {
            count += c;
        }

        public bool CanMerge(IItemStack item)
        {
            if (item.GetType() != typeof(DrugStack)) return false;
            var drug = (DrugStack) item;
            if (drug.Ions.Count != Ions.Count || drug.Tag != Tag || drug.Properties != Properties) return false;
            return Ions.All(ion => drug.Ions.Any(i =>
                ion.Element == i.Element && ion.Index == i.Index && Math.Abs(ion.Amount - i.Amount) < 0.000001f &&
                Math.Abs(ion.Concentration - i.Concentration) < 0.000001f));
        }

        public string GetName()
        {
            return $"{Tag}({count})";
        }

        public void OnMouseClickLeft(GameObject user, Vector2 pos)
        {
        }

        public void OnMouseClickRight(GameObject user, Vector2 pos)
        {
        }

        public void OnMouseClickLeftDown(GameObject user, Vector2 pos)
        {
            if (Camera.main == null) return;
            BuffInvoker.InvokeByThrower(this, pos, user.GetComponent<Rigidbody2D>().position);
        }

        public void OnMouseClickRightDown(GameObject user, Vector2 pos)
        {
            if (Camera.main == null) return;
            BuffInvoker.InvokeByThrower(this, user.GetComponent<Rigidbody2D>().position,
                user.GetComponent<Rigidbody2D>().position);
        }

        public void OnMouseClickLeftUp(GameObject user, Vector2 pos)
        {
        }

        public void OnMouseClickRightUp(GameObject user, Vector2 pos)
        {
        }

        public void Selecting(GameObject user)
        {
        }

        public void OnSelected(GameObject user)
        {
        }

        public void LoseSelected(GameObject user)
        {
        }

        public Func<IItemStack, bool> SubInventory()
        {
            return item => false;
        }

        public void OnBulletHit(GameObject target)
        {
            if (!target.TryGetComponent<BuffRenderer>(out var br)) return;
            br.Register((DrugStack) Fetch(1));
        }

        public string GetFillerName()
        {
            return Tag;
        }

        public bool Equals(IFiller other)
        {
            return other != null && other.GetType() == typeof(DrugStack) && CanMerge((DrugStack) other);
        }
    }

    public class Sling : IItemStack
    {
        private bool fetched;

        public Sling()
        {
            fetched = false;
        }

        public string GetName()
        {
            return "弹弓";
        }

        public void OnMouseClickLeft(GameObject user, Vector2 pos)
        {
            if (!user.GetComponent<Inventory>().TryGetSubItem(out var stone)) return;
            stone.OnMouseClickLeft(user, pos);
        }

        public void OnMouseClickRight(GameObject user, Vector2 pos)
        {
            if (!user.GetComponent<Inventory>().TryGetSubItem(out var stone)) return;
            stone.OnMouseClickRight(user, pos);
        }

        public void OnMouseClickLeftDown(GameObject user, Vector2 pos)
        {
            if (!user.GetComponent<Inventory>().TryGetSubItem(out var stone)) return;
            stone.OnMouseClickLeftDown(user, pos);
        }

        public void OnMouseClickRightDown(GameObject user, Vector2 pos)
        {
            if (!user.GetComponent<Inventory>().TryGetSubItem(out var stone)) return;
            stone.OnMouseClickRightDown(user, pos);
        }

        public void OnMouseClickLeftUp(GameObject user, Vector2 pos)
        {
            if (!user.GetComponent<Inventory>().TryGetSubItem(out var stone)) return;
            stone.OnMouseClickLeftUp(user, pos);
        }

        public void OnMouseClickRightUp(GameObject user, Vector2 pos)
        {
            if (!user.GetComponent<Inventory>().TryGetSubItem(out var stone)) return;
            stone.OnMouseClickRightUp(user, pos);
        }

        public void Selecting(GameObject user)
        {
        }

        public void OnSelected(GameObject user)
        {
        }

        public void LoseSelected(GameObject user)
        {
        }

        public int GetCount()
        {
            return fetched ? 0 : 1;
        }

        public bool CanMerge(IItemStack item)
        {
            return false;
        }

        public void Merge(IItemStack item)
        {
        }

        public IItemStack Fetch(int count)
        {
            return count == 0 ? new Sling {fetched = true} : this;
        }

        public Func<IItemStack, bool> SubInventory()
        {
            return item => item.GetType() == typeof(DrugStack);
        }
    }

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

        public Element(string symbol)
        {
            this.symbol = symbol;
        }

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