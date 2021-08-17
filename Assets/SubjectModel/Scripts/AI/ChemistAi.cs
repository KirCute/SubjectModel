using System.Collections.Generic;
using SubjectModel.Scripts.Chemistry;
using SubjectModel.Scripts.InventorySystem;
using UnityEngine;

namespace SubjectModel.Scripts.AI
{
    public static class ChemistAi
    {
        public static void GenerateChemist(GameObject self)
        {
            if (!self.TryGetComponent<Inventory>(out var inventory)) return;
            inventory.Add(new Sling());
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
            inventory.SwitchTo(0);
        }
    }
}