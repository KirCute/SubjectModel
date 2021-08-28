using System;
using System.Collections.Generic;
using System.Linq;
using Bolt;
using SubjectModel.Scripts.InventorySystem;
using UnityEngine;

namespace SubjectModel.Scripts.Subject.Firearms
{
    public static class FirearmDictionary
    {
        public static List<FirearmTemple> FirearmTemples; // = new List<FirearmTemple>();

        public static List<MagazineTemple> MagazineTemples; /* = new List<MagazineTemple>
        {
            new MagazineTemple("弹匣0", 20, 1f, 1f, 1f),
            new MagazineTemple("弹匣1", 100, 3f, 1f, 1f)
        };
        //*/

        public static List<BulletTemple> BulletTemples; /* = new List<BulletTemple>
        {
            new BulletTemple("子弹0", 37.5f, 0f, 50f, 0f, 1f, 1f),
            new BulletTemple("子弹1", 6.25f, 87.5f, 200f, 120f, 1f, 1f),
            new BulletTemple("子弹2", 10f, 0f, 100f, 0f, 1f, 1f)
        };
        //*/

        public static readonly Func<Firearm, string> DefaultName = firearm =>
        {
            var fn = firearm.Temple.Name;
            var l = firearm.Loading;
            var m = firearm.Magazine;
            var b = m?.Containing;
            var bn = b?.Temple.Name;
            var bf = b?.Filler;
            var bc = b?.Count;
            var mc = m?.Temple.BulletContains;
            return m == null
                ? $"{fn}{(l > .0f ? "(装填中)" : "")}"
                : b == null
                    ? $"{fn}({m.Temple.Name})"
                    : bf == null
                        ? $"{fn}({bn} {bc}/{mc}{(l > .0f ? " 装填中" : "")})"
                        : $"{fn}({bn} {bf.FillerName} {bc}/{mc}{(l > .0f ? " 装填中" : "")})";
        };

        private static readonly Action<Firearm, GameObject, Vector2> DefaultShoot = (firearm, user, aim) =>
        {
            if (firearm.Loading > .0f || firearm.Magazine?.Containing == null || Camera.main == null) return;
            var shooterPosition = user.GetComponent<Rigidbody2D>().position;
            if (shooterPosition == aim) return;
            aim = Utils.GetRotatedVector(aim - shooterPosition,
                Utils.GenerateGaussian(.0f, firearm.Deviation,
                    (float) (Math.PI / firearm.Temple.MaxRange))) + shooterPosition;

            firearm.Magazine.Containing.Count--;
            if (firearm.Magazine.Containing.Count != 0) firearm.Loading = firearm.Temple.Loading;
            firearm.InvokeKick(user);
            var collider = user.GetComponent<GunFlash>().Shoot(shooterPosition, aim);
            if (collider != null)
            {
                var declarations = collider.GetComponent<Variables>().declarations;
                var defence = declarations.IsDefined("Defence") ? declarations.Get<float>("Defence") : .0f;
                var health = declarations.Get<float>("Health");
                var depth = firearm.Magazine.Containing.Filler == null
                    ? firearm.Temple.Depth + firearm.Magazine.Containing.Temple.Depth
                    : 0f;
                var minDefence = firearm.Magazine.Containing.Temple.MinDefence;
                var damage = firearm.Temple.Damage * firearm.Magazine.Containing.Temple.BreakDamage;
                var explode = firearm.Magazine.Containing.Temple.Explode;
                declarations.Set("Health",
                    health - (defence > depth
                            ? Utils.Map(.0f, defence, .0f, damage, depth) // 未击穿
                            : depth - defence <= minDefence || minDefence < 0.000001f // 未过穿 或 不存在过穿可能
                                ? damage + (firearm.Magazine.Containing.Filler == null ? explode : .0f) // 刚好击穿
                                : damage // 过穿
                    )
                );
                /*if (defence <= depth) */
                firearm.Magazine.Containing.Filler?.OnBulletHit(collider.gameObject);
            }

            if (firearm.Magazine.Containing.Count == 0) firearm.Magazine.Containing = null;
        };

        public static readonly Action<Firearm, GameObject> DefaultReload = (firearm, user) =>
        {
            if (firearm.SwitchingMagazine) return;
            var ret = user.GetComponent<Inventory>().TryGetSubItem(out var ready);
            if (ret)
            {
                firearm.Ready = ready;
                firearm.SwitchingMagazine = true;
                firearm.Loading = firearm.Temple.Reload;
                var declarations = user.GetComponent<Variables>().declarations;
                declarations.Set("Speed", declarations.Get<float>("Speed") * firearm.Temple.ReloadSpeed);
            }

            if (firearm.Magazine != null) firearm.AddWeight(user, -firearm.Magazine.Temple.Weight);
            user.GetComponent<Inventory>().Add(firearm.Magazine);
            firearm.Magazine = null;
        };

        public static readonly Func<Firearm, Func<IItemStack, bool>> DefaultSub = firearm => item =>
            item is Magazine m && firearm.Temple.Magazine.Contains(m.Temple.Name);

        public static readonly Action<Firearm, GameObject> DefaultCompleteReload = (firearm, user) =>
        {
            var ready = (Magazine) firearm.Ready;
            firearm.Magazine = ready;
            firearm.AddWeight(user, firearm.Magazine.Temple.Weight);
            user.GetComponent<Inventory>().Remove(ready);
            firearm.Ready = null;
            var declarations = user.GetComponent<Variables>().declarations;
            declarations.Set("Speed", declarations.Get<float>("Speed") / firearm.Temple.ReloadSpeed);
        };

        public static readonly Action<Firearm, GameObject, Vector2> NoShoot = (firearm, user, aim) => { };

        public static readonly Action<Firearm, GameObject> NoReload = (firearm, user) => { };

        private static readonly string[] Type = {"Unknown", "AR", "RF"};

        public static readonly Action<Firearm>[] FirearmBuilder =
        {
            firearm => { },
            firearm =>
            {
                firearm.ShootKeep = DefaultShoot;
                firearm.Reload = DefaultReload;
            },
            firearm =>
            {
                firearm.Magazine = new Magazine(MagazineTemples
                    .Where(item => item.Name == firearm.Temple.Magazine[0]).FirstOrDefault());
                firearm.NameGetter = gun =>
                {
                    var fn = gun.Temple.Name;
                    var l = gun.Loading;
                    var b = gun.Magazine.Containing;
                    var bn = b?.Temple.Name;
                    var bf = b?.Filler;
                    var bc = b?.Count;
                    var mc = gun.Magazine.Temple.BulletContains;
                    return b == null
                        ? $"{fn}{(l > .0f ? "(装填中)" : "")}"
                        : bf == null
                            ? $"{fn}({bn} {bc}/{mc}{(l > .0f ? " 装填中" : "")})"
                            : $"{fn}({bn} {bf.FillerName} {bc}/{mc}{(l > .0f ? " 装填中" : "")})";
                };
                firearm.ShootOnce = DefaultShoot;
                firearm.Sub = gun => gun.Magazine.AppropriateBullet();
                firearm.Reload = (gun, user) =>
                {
                    if (gun.SwitchingMagazine) return;
                    var ret = user.GetComponent<Inventory>().TryGetSubItem(out var ready);
                    if (ret)
                    {
                        var bullet = (Bullet) ready;
                        if (gun.Magazine.Containing == null || gun.Magazine.Containing.Is(bullet)) return;
                    }

                    user.GetComponent<Inventory>().Add(gun.Magazine.Containing);
                    gun.Magazine.Containing = null;
                };
                firearm.ReloadKeep = (gun, user) =>
                {
                    if (gun.SwitchingMagazine || !user.GetComponent<Inventory>().TryGetSubItem(out var ready)) return;
                    var bullet = (Bullet) ready;
                    if (gun.Magazine.Containing != null && (!gun.Magazine.Containing.Is(bullet) ||
                                                            gun.Magazine.Containing.Count >=
                                                            gun.Magazine.Temple.BulletContains)) return;
                    gun.Ready = bullet;
                    gun.SwitchingMagazine = true;
                    gun.Loading = gun.Temple.Reload;
                    var declarations = user.GetComponent<Variables>().declarations;
                    declarations.Set("Speed", declarations.Get<float>("Speed") * gun.Temple.ReloadSpeed);
                };
                firearm.CompleteReload = (gun, user) =>
                {
                    var bullet = (Bullet) gun.Ready.Fetch(1);
                    if (gun.Magazine.Containing == null) gun.Magazine.Containing = bullet;
                    else gun.Magazine.Containing.Count++;
                    gun.Ready = null;
                    var declarations = user.GetComponent<Variables>().declarations;
                    declarations.Set("Speed", declarations.Get<float>("Speed") / gun.Temple.ReloadSpeed);
                };
            }
        };

        public static int TypeIndexOf(string type)
        {
            for (var i = 0; i < Type.Length; i++)
                if (Type[i].Equals(type))
                    return i;
            return 0;
        }

        public static string GetFirearmType(int type)
        {
            return Type[type];
        }
    }

    [Serializable]
    public class GunData
    {
        public List<FirearmTemple> firearmTemples;
        public List<MagazineTemple> magazineTemples;
        public List<BulletTemple> bulletTemples;
    }
}