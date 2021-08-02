using System;
using SubjectModel.Scripts.InventorySystem;
using UnityEngine;

namespace SubjectModel.Scripts.Firearms
{
    [Serializable]
    public class MagazineTemple
    {
        public readonly string Name;
        public readonly int BulletContains;
        public readonly float Weight;
        public readonly float Radius;
        public readonly float Length;

        public MagazineTemple(string name, int bulletContains, float weight, float radius, float length)
        {
            Name = name;
            BulletContains = bulletContains;
            Weight = weight;
            Radius = radius;
            Length = length;
        }
    }

    public class Magazine : IItemStack
    {
        public readonly MagazineTemple Temple;
        public Bullet Containing;
        private bool fetched;

        public Magazine(MagazineTemple temple)
        {
            Temple = temple;
            Containing = null;
            fetched = false;
        }

        public string GetName()
        {
            return Containing == null
                ? Temple.Name
                : Containing.Filler == null
                    ? $"{Temple.Name}({Containing.Temple.Name} {Containing.Count}/{Temple.BulletContains})"
                    : $"{Temple.Name}({Containing.Temple.Name} {Containing.Filler.GetFillerName()} {Containing.Count}/{Temple.BulletContains})";
        }

        public void OnMasterUseKeep(GameObject user, Vector2 pos)
        {
        }

        public void OnMasterUseOnce(GameObject user, Vector2 pos)
        {
            user.GetComponent<Inventory>().Add(Containing);
            Containing = null;
        }

        public void OnSlaveUseKeep(GameObject user)
        {
        }

        public void OnSlaveUseOnce(GameObject user)
        {
            if (!user.GetComponent<Inventory>().TryGetSubItem(out var item)) return;
            var bullet = (Bullet) item;
            user.GetComponent<Inventory>().Add(Containing);
            if (bullet.Count > Temple.BulletContains) Containing = (Bullet) bullet.Fetch(Temple.BulletContains);
            else
            {
                Containing = bullet;
                user.GetComponent<Inventory>().Remove(bullet);
            }
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

        public int GetCount()
        {
            return fetched ? 0 : 1;
        }

        public bool CanMerge(IItemStack item)
        {
            return false;
        }

        public void Merge(IItemStack item)
        {
        }

        public IItemStack Fetch(int count)
        {
            if (count > 0) fetched = true;
            return count == 0
                ? new Magazine(Temple) {Containing = Containing, fetched = true}
                : new Magazine(Temple) {Containing = Containing};
        }

        public Func<IItemStack, bool> SubInventory()
        {
            return item =>
                item.GetType() == typeof(Bullet) &&
                Math.Abs(((Bullet) item).Temple.Length - Temple.Length) < 0.000001f &&
                Math.Abs(((Bullet) item).Temple.Radius - Temple.Radius) < 0.000001f;
        }
    }
}