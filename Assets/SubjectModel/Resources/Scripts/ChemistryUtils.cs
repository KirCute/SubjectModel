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
        private static readonly float[] DefaultParam = {1f, 1f};

        public static readonly Element H = new Element("H")
        {
            valences = new[] {0, 1},
            oxygenCount = new[]
            {
                new[] {.0f, .0f},
                new[] {.0f, .5f}
            },
            potential = new[]
            {
                new[] {.0f},
                new[] {.0f}
            },
            buffType = new[] {Buff.Empty, Buff.Corrosion},
            buffParam = new[] {DefaultParam, new[] {2.0f, 50f}},
            state = new[]
            {
                new[] {Element.Gas, Element.Aqua},
                new[] {Element.Gas, Element.Aqua},
            }
        };

        public static readonly Element O = new Element("O")
        {
            valences = new[] {-2, -1, 0},
            oxygenCount = new[]
            {
                new[] {.0f, .0f, 1f},
                new[] {.0f, .0f, 1f}
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
        };

        public static readonly Element Cl = new Element("Cl")
        {
            valences = new[] {-1, 0},
            oxygenCount = new[]
            {
                new[] {.0f, .0f},
                new[] {.0f, .0f}
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
        };

        public static readonly Element K = new Element("K")
        {
            valences = new[] {1},
            oxygenCount = new[]
            {
                new[] {.0f, .0f}
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
        };

        public static readonly Element Mn = new Element("Mn")
        {
            valences = new[] {2, 4, 7},
            oxygenCount = new[]
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
        };

        public static readonly Element Fe = new Element("Fe")
        {
            valences = new[] {2, 3, 6},
            oxygenCount = new[]
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
        };

        public static readonly Element Co = new Element("Co")
        {
            valences = new[] {2, 3, 4},
            oxygenCount = new[]
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
        };

        public static readonly Element Cu = new Element("Cu")
        {
            valences = new[] {2, 3},
            oxygenCount = new[]
            {
                new[] {.0f, 1F},
                new[] {1f, 2F}
            },
            potential = new[]
            {
                new[] {1.8f},
                new[]
                {
                    /* Offstandard */ 0.65f
                }
            },
            buffType = new[] {Buff.Poison, Buff.Empty},
            buffParam = new[] {new[] {2f, 12.5f}, DefaultParam},
            state = new[]
            {
                new[] {Element.Aqua, Element.Aqua},
                new[] {Element.Solid, Element.Solid}
            }
        };
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
        public Element Element;
        public int Index;
        public float Amount;
        public float Concentration;
        public float DropTime;

        public string GetSymbol(int properties)
        {
            var charge = Element.GetCharge(Index, properties);
            var chargeStr = charge == 0
                ? ""
                : Math.Abs(charge) == 1
                    ? $"{(charge >= 0 ? "+" : "-")}"
                    : $"{Math.Abs(charge).ToString()}{(charge >= 0 ? "+" : "-")}";
            return Element.oxygenCount[properties][Index] < .5f
                ? $"{Element.symbol}{chargeStr}"
                : $"{Element.symbol}O{Element.oxygenCount[properties][Index]} {chargeStr}";
        }
    }

    public class DrugStack : ItemStack
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
        
        public ItemStack Fetch()
        {
            var ions = Ions.Select(ion => new IonStack
                    {Element = ion.Element, Index = ion.Index, Amount = ion.Amount, Concentration = ion.Concentration})
                .ToList();
            count--;
            return new DrugStack(Tag, ions, Properties, 1);
        }

        public int GetCount()
        {
            return count;
        }

        public string GetName()
        {
            return $"{Tag}({count})";
        }

        public void OnMouseClickLeft(GameObject user, Vector2 pos)
        {
            if (Camera.main == null) return;
            BuffInvoker.InvokeByThrower(this, pos, user.GetComponent<Rigidbody2D>().position);
        }

        public void OnMouseClickRight(GameObject user, Vector2 pos)
        {
            if (Camera.main == null) return;
            BuffInvoker.InvokeByThrower(this, user.GetComponent<Rigidbody2D>().position,
                user.GetComponent<Rigidbody2D>().position);
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

        public Func<ItemStack, bool> SubInventory()
        {
            return item => false;
        }
    }
    
    public class Sling : ItemStack
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

        public void Selecting(GameObject user) { }

        public void OnSelected(GameObject user) { }

        public void LoseSelected(GameObject user) { }

        public int GetCount()
        {
            return fetched ? 0 : 1;
        }

        public ItemStack Fetch()
        {
            fetched = true;
            return this;
        }

        public Func<ItemStack, bool> SubInventory()
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
        public float[][] oxygenCount;
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

        /*
        public bool GetOxidizedPotentialWithNernst(int index, float pH, out float p)
        {
            var result = GetOxidizedPotential(index, out p);
            if (result)
                p += 0.5916f / (valences[index + 1] - valences[index]) * (-pH) *
                     (GetOxygenCount(index + 1, pH) - GetOxygenCount(index, pH)) * 2;
            return result;
        }
        */

        public bool GetReducedPotential(int index, int properties, out float p)
        {
            return GetOxidizedPotential(index - 1, properties, out p);
        }

        /*
        public bool GetReducedPotentialWithNernst(int index, float pH, out float p)
        {
            return GetOxidizedPotentialWithNernst(index - 1, pH, out p);
        }
        */

        public float GetOxidizedCoefficient(int index)
        {
            return valences[index + 1] - valences[index];
        }

        public float GetReducedCoefficient(int index)
        {
            return valences[index] - valences[index - 1];
        }

        public float GetOxygenCount(int index, int properties)
        {
            return oxygenCount[properties][index];
        }

        public int GetCharge(int index, int properties)
        {
            return valences[index] - (int) (GetOxygenCount(index, properties) * 2f);
        }
    }
}