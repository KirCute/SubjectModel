using System;
using SubjectModel.Scripts.InventorySystem;

namespace SubjectModel.Scripts.Firearms
{
    [Serializable]
    public class BulletTemple
    {
        public readonly string Name;
        public readonly float BreakDamage;
        public readonly float Explode;
        public readonly float Depth;
        public readonly float MinDefence;
        public readonly float Radius;
        public readonly float Length;

        public BulletTemple(string name, float breakDamage, float explode, float depth, float minDefence, float radius,
            float length)
        {
            Name = name;
            BreakDamage = breakDamage;
            Explode = explode;
            Depth = depth;
            MinDefence = minDefence;
            Radius = radius;
            Length = length;
        }
    }

    public class Bullet : IItemStack
    {
        public readonly BulletTemple Temple;
        public readonly IFiller Filler;
        public int Count;

        public Bullet(BulletTemple temple, int count, IFiller filler = null)
        {
            Temple = temple;
            Count = count;
            Filler = filler;
        }

        public string GetName()
        {
            return Filler == null
                ? $"{Temple.Name}({Count})"
                : $"{Temple.Name}({Filler.GetFillerName()})({Count})";
        }

        public int GetCount()
        {
            return Count;
        }

        public bool CanMerge(IItemStack item)
        {
            if (!(item is Bullet bullet) || bullet.Temple != Temple) return false;
            if (Filler == null) return bullet.Filler == null;
            return Filler.Equals(bullet.Filler);
        }

        public void Merge(IItemStack item)
        {
            Count += ((Bullet) item).Count;
            Filler?.CountAppend(((Bullet) item).Filler.GetCount());
        }

        public IItemStack Fetch(int count)
        {
            if (count > Count) count = Count;
            Count -= count;
            return new Bullet(Temple, count, Filler);
        }

        public bool Is(Bullet bullet)
        {
            return bullet.Temple == Temple && (Filler?.Equals(bullet.Filler) ?? bullet.Filler == null);
        }
    }
}