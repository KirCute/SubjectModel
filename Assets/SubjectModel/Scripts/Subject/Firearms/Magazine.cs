using System;
using SubjectModel.Scripts.InventorySystem;

namespace SubjectModel.Scripts.Subject.Firearms
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

    public class Magazine : Unstackable
    {
        public readonly MagazineTemple Temple;
        public Bullet Containing;

        public Magazine(MagazineTemple temple)
        {
            Temple = temple;
            Containing = null;
        }

        public override string GetName()
        {
            return Containing == null
                ? Temple.Name
                : Containing.Filler == null
                    ? $"{Temple.Name}({Containing.Temple.Name} {Containing.Count}/{Temple.BulletContains})"
                    : $"{Temple.Name}({Containing.Temple.Name} {Containing.Filler.GetFillerName()} {Containing.Count}/{Temple.BulletContains})";
        }

        public void Release(Inventory inv)
        {
            inv.Add(Containing);
            Containing = null;
        }

        public void Load(Inventory inv, Bullet bullet)
        {
            Release(inv);
            if (!AppropriateBullet()(bullet)) return;
            if (bullet.Count > Temple.BulletContains) Containing = (Bullet) bullet.Fetch(Temple.BulletContains);
            else
            {
                Containing = bullet;
                inv.Remove(bullet);
            }
        }

        public bool Load(Bullet bullet, out Bullet containing)
        {
            containing = Containing;
            Containing = null;
            var ret = AppropriateBullet()(bullet);
            if (ret) Containing = bullet;
            return ret;
        }

        public override IItemStack Fetch(int count)
        {
            return new Magazine(Temple) {Containing = Containing};
        }

        public Func<IItemStack, bool> AppropriateBullet()
        {
            return item =>
                item is Bullet bullet &&
                Math.Abs(bullet.Temple.Length - Temple.Length) < 0.001f &&
                Math.Abs(bullet.Temple.Radius - Temple.Radius) < 0.001f;
        }
    }
}