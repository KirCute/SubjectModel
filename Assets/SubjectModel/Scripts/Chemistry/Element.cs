using System;
using System.Collections.Generic;
using System.Linq;

namespace SubjectModel.Scripts.Chemistry
{
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
                buffParam = new[] {DefaultParam, new[] {0.25f, 20f}},
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
}