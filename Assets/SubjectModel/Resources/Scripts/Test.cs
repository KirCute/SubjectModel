using System.Collections.Generic;

namespace SubjectModel
{
    public static class Test
    {
        public static void GeneratePlayerInventory(Inventory inventory)
        {
            var debugMagazine = new MagazineTemple("测试用弹匣", 20);
            inventory.Add(new Firearm(new FirearmTemple("模板0", 100f, 0.75f, 0.2f,
                1f, 20f, 0.025f, 0.5f, 1f, 20f, 0.5f, debugMagazine)));
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
                10
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
                10
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
                10
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
                10
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
                10
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
                10
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
                10
            ));
            inventory.Add(new Magazine(debugMagazine) {BulletRemain = 20});
            inventory.Add(new Magazine(debugMagazine) {BulletRemain = 20});
            inventory.Add(new Magazine(debugMagazine) {BulletRemain = 20});
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
                        Element = Elements.Cl, Index = Elements.Cl.GetIndex(-1), Amount = 2f, Concentration = 3f}
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