using System;
using SubjectModel.Scripts.InventorySystem;

namespace SubjectModel.Scripts.Firearms
{
    /**
     * <summary>
     * 子弹的模板
     * 可序列化，在子弹实例创建时作为参数。
     * </summary>
     */
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

    /**
     * <summary>子弹</summary>
     */
    public class Bullet : IItemStack
    {
        public readonly BulletTemple Temple; //模板
        public readonly IFiller Filler; //内容物，在击中时沾染在目标上
        public int Count; //数量

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