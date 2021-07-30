using System.Collections.Generic;

namespace SubjectModel
{
    public static class Test
    {
        public static readonly IList<FirearmTemple> FirearmTemples = new List<FirearmTemple>();

        public static readonly IList<MagazineTemple> MagazineTemples = new List<MagazineTemple>
        {
            new MagazineTemple("弹匣0", 20, 1f, 1f, 1f),
            new MagazineTemple("弹匣1", 100, 3f, 1f, 1f)
        };

        public static readonly IList<BulletTemple> BulletTemples = new List<BulletTemple>
        {
            new BulletTemple("子弹0", 37.5f, 0f, 50f, 0f, 1f, 1f),
            new BulletTemple("子弹1", 6.25f, 87.5f, 200f, 120f, 1f, 1f),
            new BulletTemple("子弹2", 10f, 0f, 100f, 0f, 1f, 1f)
        };

        public static void GeneratePlayerInventory(Inventory inventory)
        {
            FirearmTemples.Add(new FirearmTemple("模板0", 2f, 0.75f, 0.2f, 2f,
                50f, 0.025f, 6f, 2f, 20f, 0.5f,
                new[] {MagazineTemples[0], MagazineTemples[1]}));
            inventory.Add(new Firearm(FirearmTemples[0]));
            inventory.Add(new Sling());
            inventory.Add(new DrugStack
            (
                "FeCl3",
                new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.Fe, Index = Elements.Fe.GetIndex(3), Amount = 1f, Concentration = 1f
                    },
                    new IonStack
                    {
                        Element = Elements.Cl, Index = Elements.Cl.GetIndex(-1), Amount = 3f, Concentration = 3f
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
                        Element = Elements.Cu, Index = Elements.Cu.GetIndex(2), Amount = 1f, Concentration = 1f
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
                        Element = Elements.Co, Index = Elements.Co.GetIndex(2), Amount = 1f, Concentration = 1f
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
                        Element = Elements.H, Index = Elements.H.GetIndex(1), Amount = 1f, Concentration = 1f
                    },
                    new IonStack
                    {
                        Element = Elements.Cl, Index = Elements.Cl.GetIndex(-1), Amount = 1f, Concentration = 1f
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
                        Element = Elements.Fe, Index = Elements.Fe.GetIndex(2), Amount = 1f, Concentration = 1f
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
                        Element = Elements.O, Index = Elements.O.GetIndex(-1), Amount = 1f, Concentration = 1f
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
                        Element = Elements.K, Index = Elements.K.GetIndex(1), Amount = 1f, Concentration = 1f
                    },
                    new IonStack
                    {
                        Element = Elements.Mn, Index = Elements.Mn.GetIndex(7), Amount = 1f, Concentration = 1f
                    }
                },
                Element.Acid,
                75
            ));
            inventory.Add(new Magazine(MagazineTemples[0]));
            inventory.Add(new Magazine(MagazineTemples[0]));
            inventory.Add(new Magazine(MagazineTemples[0]));
            inventory.Add(new Magazine(MagazineTemples[1]));
            inventory.Add(new Bullet(BulletTemples[0], 1000));
            inventory.Add(new Bullet(BulletTemples[1], 1000));
            inventory.Add(new Bullet(BulletTemples[2], 500, new DrugStack(
                "CuSO4",
                new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.Cu, Index = Elements.Cu.GetIndex(2), Amount = 0.5f, Concentration = 1f
                    }
                },
                Element.Acid,
                500
            )));
            inventory.Add(new Bullet(BulletTemples[2], 500, new DrugStack(
                "H2O2",
                new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.O, Index = Elements.O.GetIndex(-1), Amount = 0.5f, Concentration = 1f
                    }
                },
                Element.Acid,
                500
            )));
            inventory.Add(new Bullet(BulletTemples[2], 500, new DrugStack(
                "KMnO4",
                new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.K, Index = Elements.K.GetIndex(1), Amount = 0.5f, Concentration = 1f
                    },
                    new IonStack
                    {
                        Element = Elements.Mn, Index = Elements.Mn.GetIndex(7), Amount = 0.5f, Concentration = 1f
                    }
                },
                Element.Acid,
                500
            )));
            inventory.Add(new Bullet(BulletTemples[2], 500));
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
                        Element = Elements.Cu, Index = Elements.Cu.GetIndex(2), Amount = 1f, Concentration = 1f
                    },
                    new IonStack
                    {
                        Element = Elements.Cl, Index = Elements.Cl.GetIndex(-1), Amount = 2f, Concentration = 3f
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
                        Element = Elements.Co, Index = Elements.Co.GetIndex(2), Amount = 1f, Concentration = 1f
                    }
                },
                Element.Acid,
                10000
            ));
        }
    }
}