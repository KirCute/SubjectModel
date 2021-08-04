using System;
using System.Collections.Generic;
using System.Linq;
using Bolt;
using SubjectModel.Scripts.InventorySystem;
using SubjectModel.Scripts.System;
using UnityEngine;

namespace SubjectModel.Scripts.Firearms
{
    public static class FirearmDictionary
    {
        public static readonly Func<Firearm, string> DefaultName = firearm => firearm.Magazine == null
            ? $"{firearm.Temple.Name}{(firearm.Loading > .0f ? "(装填中)" : "")}"
            : firearm.Magazine.Containing == null
                ? $"{firearm.Temple.Name}({firearm.Magazine.Temple.Name})"
                : firearm.Magazine.Containing.Filler == null
                    ? $"{firearm.Temple.Name}({firearm.Magazine.Containing.Temple.Name} {firearm.Magazine.Containing.Count}/{firearm.Magazine.Temple.BulletContains}{(firearm.Loading > .0f ? " 装填中" : "")})"
                    : $"{firearm.Temple.Name}({firearm.Magazine.Containing.Temple.Name} {firearm.Magazine.Containing.Filler.GetFillerName()} {firearm.Magazine.Containing.Count}/{firearm.Magazine.Temple.BulletContains}{(firearm.Loading > .0f ? " 装填中" : "")})";

        private static readonly Action<Firearm, GameObject, Vector2> DefaultShoot = (firearm, user, aim) =>
        {
            if (firearm.Loading > .0f || firearm.Magazine?.Containing == null ||
                firearm.Magazine.Containing.Count <= 0 ||
                Camera.main == null) return;
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
                if (defence <= depth) firearm.Magazine.Containing.Filler?.OnBulletHit(collider.gameObject);
            }

            if (firearm.Magazine.Containing.Count == 0) firearm.Magazine.Containing = null;
        };

        public static readonly Action<Firearm, GameObject> DefaultReload = (firearm, user) =>
        {
            if (firearm.SwitchingMagazine || !user.GetComponent<Inventory>().TryGetSubItem(out var ready)) return;
            firearm.Ready = ready;
            firearm.SwitchingMagazine = true;
            firearm.Loading = firearm.Temple.Reload;
            var declarations = user.GetComponent<Variables>().declarations;
            declarations.Set("Speed", declarations.Get<float>("Speed") * firearm.Temple.ReloadSpeed);
            if (firearm.Magazine != null) firearm.AddWeight(user, -firearm.Magazine.Temple.Weight);

            user.GetComponent<Inventory>().Add(firearm.Magazine);
            firearm.Magazine = null;
        };

        public static readonly Func<Firearm, Func<IItemStack, bool>> DefaultSub = firearm => item =>
            item.GetType() == typeof(Magazine) && firearm.Temple.Magazine.Contains(((Magazine) item).Temple.Name);

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
            firearm => {
                firearm.ShootKeep = DefaultShoot;
                firearm.Reload = DefaultReload;
            },
            firearm =>
            {
                firearm.Magazine = new Magazine(Test.MagazineTemples
                    .Where(item => item.Name == firearm.Temple.Magazine[0]).FirstOrDefault());
                firearm.Name = gun => gun.Magazine.Containing == null
                    ? $"{gun.Temple.Name}{(firearm.Loading > .0f ? "(装填中)" : "")}"
                    : gun.Magazine.Containing.Filler == null
                        ? $"{gun.Temple.Name}({gun.Magazine.Containing.Temple.Name} {gun.Magazine.Containing.Count}/{gun.Magazine.Temple.BulletContains}{(firearm.Loading > .0f ? " 装填中" : "")})"
                        : $"{gun.Temple.Name}({gun.Magazine.Containing.Temple.Name} {gun.Magazine.Containing.Filler.GetFillerName()} {gun.Magazine.Containing.Count}/{gun.Magazine.Temple.BulletContains}{(firearm.Loading > .0f ? " 装填中" : "")})";
                firearm.ShootOnce = DefaultShoot;
                firearm.Sub = gun => gun.Magazine.SubInventory();
                firearm.Reload = (gun, user) =>
                {
                    if (gun.SwitchingMagazine || !user.GetComponent<Inventory>().TryGetSubItem(out var ready)) return;
                    var bullet = (Bullet) ready;
                    if (gun.Magazine.Containing == null || gun.Magazine.Containing.Is(bullet)) return;
                    user.GetComponent<Inventory>().Add(gun.Magazine.Containing);
                    gun.Magazine.Containing = null;
                };
                firearm.ReloadKeep = (gun, user) =>
                {
                    if (gun.SwitchingMagazine || !user.GetComponent<Inventory>().TryGetSubItem(out var ready)) return;
                    var bullet = (Bullet) ready;
                    if (gun.Magazine.Containing != null && (!gun.Magazine.Containing.Is(bullet) ||
                            gun.Magazine.Containing.Count >= gun.Magazine.Temple.BulletContains)) return;
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

    public interface IFiller
    {
        public void OnBulletHit(GameObject target);
        public string GetFillerName();
        public bool Equals(IFiller other);
        public int GetCount();
        public void CountAppend(int count);
    }

    [Serializable]
    public class GunData
    {
        public List<FirearmTemple> firearmTemples;
        public List<MagazineTemple> magazineTemples;
        public List<BulletTemple> bulletTemples;
    }
}