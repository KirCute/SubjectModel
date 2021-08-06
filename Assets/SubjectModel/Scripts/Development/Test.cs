using System.Collections.Generic;
using SubjectModel.Scripts.Chemistry;
using SubjectModel.Scripts.Firearms;
using SubjectModel.Scripts.InventorySystem;
using SubjectModel.Scripts.SceneObjects;
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
            inventory.Add(new Magazine(FirearmDictionary.MagazineTemples[0]));
            inventory.Add(new Magazine(FirearmDictionary.MagazineTemples[0]));
            inventory.Add(new Magazine(FirearmDictionary.MagazineTemples[0]));
            inventory.Add(new Magazine(FirearmDictionary.MagazineTemples[1]));
            inventory.Add(new Bullet(FirearmDictionary.BulletTemples[0], 1000));
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
        }

        public static void GenerateChemicalEnemyInventory(Inventory inventory)
        {
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
                10000
            ));
        }

        public static void GenerateShooterEnemyInventory(Inventory inventory)
        {
            inventory.Add(new Firearm(FirearmDictionary.FirearmTemples[0]));
            inventory.Add(new Bullet(FirearmDictionary.BulletTemples[0], 1000000));
            inventory.Add(new Magazine(FirearmDictionary.MagazineTemples[0]));
        }

        public static void GenerateContainerData()
        {
            foreach (var container in GameObject.FindGameObjectsWithTag("Container"))
            {
                var chest = container.GetComponent<Chest>();
                switch (container.name)
                {
                    case "PhysicsAntiArmor":
                        break;
                    case "PhysicsAntiBody":
                        break;
                    case "Poison":
                        break;
                    case "Reaction":
                        break;
                    case "Suicide":
                        break;
                }
            }
        }
    }
}