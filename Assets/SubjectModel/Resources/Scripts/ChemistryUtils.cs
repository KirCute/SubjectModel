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

        public static readonly Element O = new Element("O")
        {
            valences = new[] {-2, -1, 0},
            oxygenCount = new[]
            {
                new[] {.0f, .0f},
                new[] {.0f, .0f},
                new[] {1f, 1f}
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
                new[] {Element.Aqua, Element.Aqua},
                new[] {Element.Aqua, Element.Aqua},
                new[] {Element.Gas, Element.Gas}
            }
        };

        public static readonly Element Mn = new Element("Mn")
        {
            valences = new[] {0, 2, 4, 7},
            oxygenCount = new[]
            {
                new[] {.0f, .0f},
                new[] {.0f, 1f},
                new[] {2f, 2f},
                new[] {4f, 4f}
            },
            potential = new[]
            {
                new[] {-1.18f, 1.23f, 1.70f},
                new[] {-1.55f, -0.05f, 0.59f}
            },
            buffType = new[] {Buff.Empty, Buff.Empty, Buff.Empty, Buff.Empty},
            buffParam = new[] {DefaultParam, DefaultParam, DefaultParam, DefaultParam},
            state = new[]
            {
                new[] {Element.Solid, Element.Solid},
                new[] {Element.Aqua, Element.Solid},
                new[] {Element.Solid, Element.Solid},
                new[] {Element.Aqua, Element.Aqua}
            }
        };

        public static readonly Element Fe = new Element("Fe")
        {
            valences = new[] {0, 2, 3, 6},
            oxygenCount = new[]
            {
                new[] {.0f, .0f},
                new[] {.0f, 1f},
                new[] {.0f, 1.5f},
                new[] {4f, 4f}
            },
            potential = new[]
            {
                new[] {-0.473f, 0.771f, 2.2f},
                new[] {-0.887f, -0.56f, 0.72f}
            },
            buffType = new[] {Buff.Empty, Buff.Rapid, Buff.Slowness, Buff.Empty},
            buffParam = new[] {DefaultParam, new[] {3f, 3f}, new[] {3f, 3f}, DefaultParam},
            state = new[]
            {
                new[] {Element.Solid, Element.Solid},
                new[] {Element.Aqua, Element.Solid},
                new[] {Element.Aqua, Element.Solid},
                new[] {Element.Aqua, Element.Aqua}
            }
        };

        public static readonly Element Co = new Element("Co")
        {
            valences = new[] {0, 2, 3, 4},
            oxygenCount = new[]
            {
                new[] {.0f, .0f},
                new[] {.0f, 1f},
                new[] {.0f, 1.5f},
                new[] {2f, 2f}
            },
            potential = new[]
            {
                new[] {-0.277f, 1.82f, 1.416f},
                new[] {-0.72f, 0.17f, 0.62f}
            },
            buffType = new[] {Buff.Empty, Buff.Curing, Buff.Empty, Buff.Empty},
            buffParam = new[] {DefaultParam, new[] {1f, 12.5f}, DefaultParam, DefaultParam},
            state = new[]
            {
                new[] {Element.Solid, Element.Solid},
                new[] {Element.Aqua, Element.Solid},
                new[] {Element.Aqua, Element.Solid},
                new[] {Element.Solid, Element.Solid}
            }
        };

        public static readonly Element Cu = new Element("Cu")
        {
            valences = new[] {0, 2, 3},
            oxygenCount = new[]
            {
                new[] {.0f, .0f},
                new[] {.0f, 1f},
                new[] {1f, 2f}
            },
            potential = new[]
            {
                new[] {0.3402f, 1.8f},
                new[] {-0.37f, /* Offstandard */ 0.65f}
            },
            buffType = new[] {Buff.Empty, Buff.Poison, Buff.Empty},
            buffParam = new[] {DefaultParam, new[] {2f, 12.5f}, DefaultParam},
            state = new[]
            {
                new[] {Element.Solid, Element.Solid},
                new[] {Element.Aqua, Element.Solid},
                new[] {Element.Aqua, Element.Solid}
            }
        };
    }

    public static class DrugDictionary
    {
        private static readonly Dictionary<Buff, Type> BuffDictionary = new Dictionary<Buff, Type>();
        private static readonly Dictionary<Buff, string> BuffName = new Dictionary<Buff, string>();
        private static readonly Dictionary<Buff, Vector3> BuffColor = new Dictionary<Buff, Vector3>();
        private static readonly List<DrugStack> DefaultInventory = new List<DrugStack>();

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

        public static IList<DrugStack> GetDefaultInventory()
        {
            if (DefaultInventory.Count != 0) return DefaultInventory;
            DefaultInventory.Add(new DrugStack
            {
                Tag = "FeCl3(H+)",
                Ions = new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.Fe, Index = Elements.Fe.GetIndex(3),
                        Amount = 1f, Concentration = 1f
                    }
                },
                Properties = Element.Acid
            });
            DefaultInventory.Add(new DrugStack
            {
                Tag = "CuSO4(H+)",
                Ions = new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.Cu, Index = Elements.Cu.GetIndex(2),
                        Amount = 1f, Concentration = 1f
                    }
                },
                Properties = Element.Acid
            });
            DefaultInventory.Add(new DrugStack
            {
                Tag = "CoSO4(H+)",
                Ions = new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.Co, Index = Elements.Co.GetIndex(2),
                        Amount = 1f, Concentration = 1f
                    }
                },
                Properties = Element.Acid
            });
            DefaultInventory.Add(new DrugStack
            {
                Tag = "FeSO4(H+)",
                Ions = new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.Fe, Index = Elements.Fe.GetIndex(2),
                        Amount = 1f, Concentration = 1f
                    }
                },
                Properties = Element.Acid
            });
            DefaultInventory.Add(new DrugStack
            {
                Tag = "H2O2(H+)",
                Ions = new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.O, Index = Elements.O.GetIndex(-1),
                        Amount = 1f, Concentration = 1f
                    }
                },
                Properties = Element.Acid
            });
            DefaultInventory.Add(new DrugStack
            {
                Tag = "KMnO4(H+)",
                Ions = new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.Mn, Index = Elements.Mn.GetIndex(7),
                        Amount = 1f, Concentration = 1f
                    }
                },
                Properties = Element.Acid
            });
            DefaultInventory.Add(new DrugStack
            {
                Tag = "Cu",
                Ions = new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.Cu, Index = Elements.Cu.GetIndex(0),
                        Amount = 1f, Concentration = 1f
                    }
                },
                Properties = Element.Acid
            });
            /*
            DefaultInventory.Add(DrugStackFactory(Buff.Slowness, new object[] {3.0f, 3.0f}));
            DefaultInventory.Add(DrugStackFactory(Buff.Poison, new object[] {2.0f, 12.5f}));
            DefaultInventory.Add(DrugStackFactory(Buff.Curing, new object[] {1.0f, 12.5f}));
            DefaultInventory.Add(DrugStackFactory(Buff.Ghost, new object[] {6.0f, 10.0f, 0.15f}));
            DefaultInventory.Add(DrugStackFactory(Buff.Corrosion, new object[] {2.0f, 50f}));
            */
            return DefaultInventory;
        }
    }

    public class IonStack
    {
        public Element Element;
        public int Index;
        public float Amount;
        public float Concentration;
    }

    public class DrugStack
    {
        public string Tag;
        public IList<IonStack> Ions;
        public int Properties;

        public DrugStack Clone()
        {
            var ions = Ions.Select(ion => new IonStack
                    {Element = ion.Element, Index = ion.Index, Amount = ion.Amount, Concentration = ion.Concentration})
                .ToList();
            return new DrugStack
            {
                Tag = Tag,
                Ions = ions,
                Properties = Properties
            };
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

        public float GetOxygenCount(int index, bool properties)
        {
            return oxygenCount[index][properties ? 0 : 1];
        }

        public int GetCharge(int index, bool properties)
        {
            return valences[index] - (int) (GetOxygenCount(index, properties) * 2f);
        }
    }
}