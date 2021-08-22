using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SubjectModel.Scripts.Subject.Chemistry
{
    /**
     * <summary>效果的枚举</summary>
     */
    public enum Buff
    {
        Empty,
        Slowness,
        Poison,
        Curing,
        Ghost,
        Corrosion,
        Rapid,
        Immune
    }

    /**
     * <summary>用于储存，发放效果有关信息的工具类</summary>
     */
    public static class DrugDictionary
    {
        private static readonly Dictionary<Buff, Type> BuffDictionary = new Dictionary<Buff, Type>();
        private static readonly Dictionary<Buff, string> BuffName = new Dictionary<Buff, string>();
        private static readonly Dictionary<Buff, Vector3> BuffColor = new Dictionary<Buff, Vector3>();

        public static Type GetTypeOfBuff(Buff buff)
        {
            if (BuffDictionary.Count != 0) return BuffDictionary[buff];
            BuffDictionary.Add(Buff.Slowness, typeof(DrugEffect.Slowness));
            BuffDictionary.Add(Buff.Poison, typeof(DrugEffect.Poison));
            BuffDictionary.Add(Buff.Curing, typeof(DrugEffect.Curing));
            BuffDictionary.Add(Buff.Ghost, typeof(DrugEffect.Ghost));
            BuffDictionary.Add(Buff.Corrosion, typeof(DrugEffect.Corrosion));
            BuffDictionary.Add(Buff.Rapid, typeof(DrugEffect.Rapid));
            BuffDictionary.Add(Buff.Immune, typeof(DrugEffect.Immune));
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
            BuffName.Add(Buff.Immune, "免疫");
            return BuffName[buff];
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
            BuffColor.Add(Buff.Immune, new Vector3(1f, 1f, 1f));
            return BuffColor[buff];
        }

        public static Vector3 GetColor(IonStack stack)
        {
            return GetColor(stack.Element.buffType[stack.Index]);
        }
    }

    /**
     * <summary>储存Element实例对象的工具类</summary>
     */
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

        /**
         * <summary>
         * 用于通过元素符号得到对应的元素实例
         * <param name="symbol">元素符号，区分大小写</param>
         * <returns>对应元素的实例</returns>
         * </summary>
         */
        public static Element Get(string symbol)
        {
            return Dic.FirstOrDefault(e => e.symbol == symbol);
        }
    }
}