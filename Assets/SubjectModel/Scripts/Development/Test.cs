using System.Collections.Generic;
using SubjectModel.Scripts.InventorySystem;
using SubjectModel.Scripts.SceneObjects;
using SubjectModel.Scripts.Subject.Chemistry;
using SubjectModel.Scripts.Subject.Firearms;
using UnityEngine;

namespace SubjectModel.Scripts.Development
{
    public static class Test
    {
        public static void GeneratePlayerInventory(Inventory inventory)
        {
            /*
            FirearmTemples.Add(new FirearmTemple("模板,0", 2f, 0.75f, 0.2f, 2f,
                50f, 0.025f, 6f, 2f, 20f, 0.5f,
                new[] {MagazineTemples[0].Name, MagazineTemples[1].Name}));
            //*/
            /*
            inventory.Add(new Firearm(FirearmDictionary.FirearmTemples[0]));
            inventory.Add(new Firearm(FirearmDictionary.FirearmTemples[2]));
            inventory.Add(new Sling());
            inventory.Add(new DrugStack
            (
                "FeCl3",
                new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.Get("Fe"), Index = Elements.Get("Fe").GetIndex(3),
                        Amount = 5f, Concentration = 1f
                    },
                    new IonStack
                    {
                        Element = Elements.Get("Cl"), Index = Elements.Get("Cl").GetIndex(-1),
                        Amount = 15f, Concentration = 3f
                    }
                },
                Element.Acid,
                50
            ));
            inventory.Add(new DrugStack
            (
                "CuSO4",
                new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.Get("Cu"), Index = Elements.Get("Cu").GetIndex(2),
                        Amount = 1f, Concentration = 1f
                    }
                },
                Element.Acid,
                100
            ));
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
                50
            ));
            inventory.Add(new DrugStack
            (
                "HCl",
                new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.Get("H"), Index = Elements.Get("H").GetIndex(1),
                        Amount = 1f, Concentration = 1f
                    },
                    new IonStack
                    {
                        Element = Elements.Get("Cl"), Index = Elements.Get("Cl").GetIndex(-1),
                        Amount = 1f, Concentration = 1f
                    }
                },
                Element.Acid,
                100
            ));
            inventory.Add(new DrugStack
            (
                "FeSO4",
                new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.Get("Fe"), Index = Elements.Get("Fe").GetIndex(2),
                        Amount = 5f, Concentration = 1f
                    }
                },
                Element.Acid,
                50
            ));
            inventory.Add(new DrugStack
            (
                "H2O2",
                new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.Get("O"), Index = Elements.Get("O").GetIndex(-1),
                        Amount = 1f, Concentration = 1f
                    }
                },
                Element.Acid,
                75
            ));
            inventory.Add(new DrugStack
            (
                "KMnO4",
                new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.Get("K"), Index = Elements.Get("K").GetIndex(1),
                        Amount = 1f, Concentration = 1f
                    },
                    new IonStack
                    {
                        Element = Elements.Get("Mn"), Index = Elements.Get("Mn").GetIndex(7),
                        Amount = 1f, Concentration = 1f
                    }
                },
                Element.Acid,
                75
            ));
            inventory.Add(new DrugStack
            (
                "PH4Cl",
                new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.Get("P"), Index = Elements.Get("P").GetIndex(-3),
                        Amount = 1f, Concentration = 1f
                    },
                    new IonStack
                    {
                        Element = Elements.Get("Cl"), Index = Elements.Get("Cl").GetIndex(-1),
                        Amount = 1f, Concentration = 1f
                    }
                },
                Element.Acid,
                25
            ));
            inventory.Add(new DrugStack
            (
                "KOH",
                new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.Get("K"), Index = Elements.Get("K").GetIndex(1),
                        Amount = 1f, Concentration = 1f
                    },
                    new IonStack
                    {
                        Element = Elements.Get("O"), Index = Elements.Get("O").GetIndex(-2),
                        Amount = 1f, Concentration = 1f
                    }
                },
                Element.Bases,
                100
            ));
            inventory.Add(new Magazine(FirearmDictionary.MagazineTemples[1]));
            var bullet0 = inventory.Add(new Bullet(FirearmDictionary.BulletTemples[0], 1000));
            inventory.Add(new Bullet(FirearmDictionary.BulletTemples[1], 1000));
            inventory.Add(new Bullet(FirearmDictionary.BulletTemples[2], 500, new DrugStack(
                "CuSO4",
                new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.Get("Cu"), Index = Elements.Get("Cu").GetIndex(2),
                        Amount = 0.5f, Concentration = 1f
                    }
                },
                Element.Acid,
                500
            )));
            inventory.Add(new Bullet(FirearmDictionary.BulletTemples[2], 500, new DrugStack(
                "H2O2",
                new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.Get("O"), Index = Elements.Get("O").GetIndex(-1),
                        Amount = 0.5f, Concentration = 1f
                    }
                },
                Element.Acid,
                500
            )));
            inventory.Add(new Bullet(FirearmDictionary.BulletTemples[2], 500, new DrugStack(
                "KMnO4",
                new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.Get("K"), Index = Elements.Get("K").GetIndex(1),
                        Amount = 0.5f, Concentration = 1f
                    },
                    new IonStack
                    {
                        Element = Elements.Get("Mn"), Index = Elements.Get("Mn").GetIndex(7),
                        Amount = 0.5f, Concentration = 1f
                    }
                },
                Element.Acid,
                500
            )));
            inventory.Add(new Bullet(FirearmDictionary.BulletTemples[2], 500));
            inventory.Add(new Bullet(FirearmDictionary.BulletTemples[4], 1000));
            for (var i = 0; i < 10; i++)
                inventory.Add(new Magazine(FirearmDictionary.MagazineTemples[0])).Load(inventory, bullet0);
            */
        }

        /**
         * <summary>生成箱子内物品的方法</summary>
         */
        public static void GenerateContainerData()
        {
            foreach (var container in GameObject.FindGameObjectsWithTag("Container"))
            {
                var chest = container.GetComponent<Chest>();
                switch (container.name) //根据箱子的名称生成其内容物
                {
                    case "PhysicsAntiArmor":
                        chest.Contains.Add(new Firearm(FirearmDictionary.FirearmTemples[0]));
                        for (var i = 0; i < 5; i++)
                            chest.Contains.Add(new Magazine(FirearmDictionary.MagazineTemples[0]))
                                .Load(new Bullet(FirearmDictionary.BulletTemples[0], 20), out _);
                        break;
                    case "PhysicsAntiBody":
                        chest.Contains.Add(new Firearm(FirearmDictionary.FirearmTemples[2]));
                        chest.Contains.Add(new Bullet(FirearmDictionary.BulletTemples[4], 100));
                        chest.Contains.Add(new Bullet(FirearmDictionary.BulletTemples[4], 20, new DrugStack(
                            "H2SO4",
                            new List<IonStack>
                            {
                                new IonStack
                                {
                                    Element = Elements.Get("H"), Index = Elements.Get("H").GetIndex(1),
                                    Amount = 1f, Concentration = 1.2f
                                }
                            },
                            Element.Acid
                        )));
                        break;
                    case "Poison":
                        chest.Contains.Add(new Sling());
                        chest.Contains.Add(new SealStack(new DrugStack
                        (
                            "CuSO4",
                            new List<IonStack>
                            {
                                new IonStack
                                {
                                    Element = Elements.Get("Cu"), Index = Elements.Get("Cu").GetIndex(2),
                                    Amount = 1f, Concentration = 1f
                                }
                            },
                            Element.Acid
                        ), 100));
                        chest.Contains.Add(new SealStack(new DrugStack
                        (
                            "FeCl3",
                            new List<IonStack>
                            {
                                new IonStack
                                {
                                    Element = Elements.Get("Fe"), Index = Elements.Get("Fe").GetIndex(3),
                                    Amount = 5f, Concentration = 1f
                                },
                                new IonStack
                                {
                                    Element = Elements.Get("Cl"), Index = Elements.Get("Cl").GetIndex(-1),
                                    Amount = 15f, Concentration = 3f
                                }
                            },
                            Element.Acid
                        ), 50));
                        chest.Contains.Add(new SealStack(new DrugStack
                        (
                            "FeSO4",
                            new List<IonStack>
                            {
                                new IonStack
                                {
                                    Element = Elements.Get("Fe"), Index = Elements.Get("Fe").GetIndex(2),
                                    Amount = 25f, Concentration = 1f
                                }
                            },
                            Element.Acid
                        ), 50));
                        chest.Contains.Add(new SealStack(new DrugStack
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
                            Element.Acid
                        ), 50));
                        break;
                    case "Reaction":
                        chest.Contains.Add(new Sling());
                        chest.Contains.Add(new SealStack(new DrugStack
                        (
                            "H2O2",
                            new List<IonStack>
                            {
                                new IonStack
                                {
                                    Element = Elements.Get("O"), Index = Elements.Get("O").GetIndex(-1),
                                    Amount = 1f, Concentration = 1f
                                }
                            },
                            Element.Acid
                        ), 75));
                        chest.Contains.Add(new SealStack(new DrugStack
                        (
                            "KMnO4",
                            new List<IonStack>
                            {
                                new IonStack
                                {
                                    Element = Elements.Get("K"), Index = Elements.Get("K").GetIndex(1),
                                    Amount = 1f, Concentration = 1f
                                },
                                new IonStack
                                {
                                    Element = Elements.Get("Mn"), Index = Elements.Get("Mn").GetIndex(7),
                                    Amount = 1f, Concentration = 1f
                                }
                            },
                            Element.Acid
                        ), 75));
                        break;
                    case "Suicide":
                        break;
                }
            }
        }
    }
}