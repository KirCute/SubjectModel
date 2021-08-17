using SubjectModel.Scripts.Firearms;
using SubjectModel.Scripts.InventorySystem;
using UnityEngine;

namespace SubjectModel.Scripts.AI
{
    public static class ShooterAi
    {
        public static void GenerateShooter(GameObject self)
        {
            if (!self.TryGetComponent<Inventory>(out var inventory)) return;
            var firearm = inventory.Add(new Firearm(FirearmDictionary.FirearmTemples[0]));
            inventory.Add(new Bullet(FirearmDictionary.BulletTemples[0], 1000000));
            inventory.Add(new Magazine(FirearmDictionary.MagazineTemples[0]));
            inventory.SwitchTo(0);
            firearm.OnSlaveUse(self);
        }

        public static void Reload(Inventory inventory, int bulletIndex)
        {
            if (!(inventory.Contains[bulletIndex] is Bullet bullet)) return;
            ((Firearm) inventory.Contains[0]).Magazine.Load(inventory, bullet);
        }
    }
}