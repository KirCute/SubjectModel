using System;
using System.Collections.Generic;
using System.Linq;
using Bolt;
using SubjectModel.Scripts.InventorySystem;
using UnityEngine;
using Material = SubjectModel.Scripts.InventorySystem.Material;

namespace SubjectModel.Scripts.Firearms
{
    /*
    new Gun {name = "模板1", damage = 90f, loadingTime = .8f, clipCapacity = 4, clipSwitchTime = 5f}
    */
    [Serializable]
    public class FirearmTemple
    {
        public readonly string Name;
        public readonly float Damage;
        public readonly float Reload; // 换弹匣时间
        public readonly float Loading; // 自动换弹时间
        public readonly float Weight;
        public readonly float Depth;
        public readonly float Deviation;
        public readonly float MaxRange;
        public readonly float Kick;
        public readonly float Distance;
        public readonly float ReloadSpeed;
        public readonly string[] Magazine;

        public FirearmTemple(string name, float damage, float reload, float loading, float weight, float depth,
            float deviation, float maxRange, float kick, float distance, float reloadSpeed, string[] magazine)
        {
            Name = name;
            Damage = damage;
            Reload = reload;
            Loading = loading;
            Weight = weight;
            Depth = depth;
            Deviation = deviation;
            MaxRange = maxRange;
            Kick = kick;
            Distance = distance;
            ReloadSpeed = reloadSpeed;
            Magazine = magazine;
        }
    }

    public class Firearm : IItemStack
    {
        private const float KickTime = .01f;
        private const float DeceleratePercentPerWeight = .08f;

        private readonly FirearmTemple temple;
        private Magazine magazine;
        private Magazine readyMagazine;
        private float loading;
        private float kickingTime;
        private float deviation;
        private float weight;
        private bool switchingMagazine;
        private bool fetched;
        private bool kicking;

        public Firearm(FirearmTemple temple)
        {
            this.temple = temple;
            magazine = null;
            deviation = temple.Deviation;
            weight = temple.Weight;
            fetched = false;
        }

        public string GetName()
        {
            return magazine == null
                ? temple.Name
                : magazine.Containing == null
                    ? $"{temple.Name}({magazine.Temple.Name})"
                    : magazine.Containing.Filler == null
                        ? $"{temple.Name}({magazine.Containing.Temple.Name} {magazine.Containing.Count}/{magazine.Temple.BulletContains})"
                        : $"{temple.Name}({magazine.Containing.Temple.Name} {magazine.Containing.Filler.GetFillerName()} {magazine.Containing.Count}/{magazine.Temple.BulletContains})";
        }

        public void OnMasterUseKeep(GameObject user, Vector2 aim)
        {
            if (loading > .0f || magazine?.Containing == null || magazine.Containing.Count <= 0 ||
                Camera.main == null) return;
            var shooterPosition = user.GetComponent<Rigidbody2D>().position;
            if (shooterPosition == aim) return;
            aim = Utils.GetRotatedVector(aim - shooterPosition,
                Utils.GenerateGaussian(.0f, deviation, (float) (Math.PI / temple.MaxRange))) + shooterPosition;

            magazine.Containing.Count--;
            if (magazine.Containing.Count != 0) loading = temple.Loading;
            InvokeKick(user);
            var collider = user.GetComponent<GunFlash>().Shoot(shooterPosition, aim);
            if (collider != null)
            {
                var declarations = collider.GetComponent<Variables>().declarations;
                var defence = declarations.IsDefined("Defence") ? declarations.Get<float>("Defence") : .0f;
                var health = declarations.Get<float>("Health");
                var depth = magazine.Containing.Filler == null ? temple.Depth + magazine.Containing.Temple.Depth : 0f;
                var minDefence = magazine.Containing.Temple.MinDefence;
                var damage = temple.Damage * magazine.Containing.Temple.BreakDamage;
                var explode = magazine.Containing.Temple.Explode;
                declarations.Set("Health",
                    health - (defence > depth
                            ? Utils.Map(.0f, defence, .0f, damage, depth) // 未击穿
                            : depth - defence <= minDefence || minDefence < 0.000001f // 未过穿 或 不存在过穿可能
                                ? damage + (magazine.Containing.Filler == null ? explode : .0f) // 刚好击穿
                                : damage // 过穿
                    )
                );
                if (defence <= depth)
                    magazine.Containing.Filler?.OnBulletHit(collider.gameObject);
            }

            if (magazine.Containing.Count == 0) magazine.Containing = null;
        }

        public void OnMasterUseOnce(GameObject user, Vector2 pos)
        {
        }

        public void OnSlaveUseKeep(GameObject user)
        {
        }

        public void OnSlaveUseOnce(GameObject user)
        {
            SwitchMagazine(user);
        }

        public void Selecting(GameObject user)
        {
            loading -= Time.deltaTime;
            kickingTime -= Time.deltaTime;
            if (kickingTime <= .0f) CancelKick(user);
            if (loading > .0f) return;
            loading = .0f;
            if (!switchingMagazine) return;
            switchingMagazine = false;
            magazine = readyMagazine;
            weight += magazine.Temple.Weight;
            ResetVelocity(user);
            user.GetComponent<Inventory>().Remove(readyMagazine);
            readyMagazine = null;
            var declarations = user.GetComponent<Variables>().declarations;
            declarations.Set("Speed", declarations.Get<float>("Speed") / temple.ReloadSpeed);
        }

        public void OnSelected(GameObject user)
        {
            ResetVelocity(user);
            user.GetComponent<GunFlash>().distance = temple.Distance;
            user.GetComponent<GunFlash>().enabled = true;
        }

        public void LoseSelected(GameObject user)
        {
            CancelVelocity(user);
            CancelKick(user);
            user.GetComponent<LineRenderer>().enabled = false;
            user.GetComponent<GunFlash>().enabled = false;
            if (!switchingMagazine) return;
            switchingMagazine = false;
            loading = .0f;
            readyMagazine = null;
            var declarations = user.GetComponent<Variables>().declarations;
            declarations.Set("Speed", declarations.Get<float>("Speed") / temple.ReloadSpeed);
        }

        public IItemStack Fetch(int count)
        {
            if (count > 0) fetched = true;
            return count == 0 ? new Firearm(temple) {fetched = true} : this;
        }

        public Func<IItemStack, bool> SubInventory()
        {
            return item =>
                item.GetType() == typeof(Magazine) && temple.Magazine.Contains(((Magazine) item).Temple.Name);
        }

        private void SwitchMagazine(GameObject user)
        {
            if (switchingMagazine || !user.GetComponent<Inventory>().TryGetSubItem(out var ready) ||
                ready.GetType() != typeof(Magazine)) return;
            readyMagazine = (Magazine) ready;
            switchingMagazine = true;
            loading = temple.Reload;
            var declarations = user.GetComponent<Variables>().declarations;
            declarations.Set("Speed", declarations.Get<float>("Speed") * temple.ReloadSpeed);
            if (magazine != null)
            {
                weight -= magazine.Temple.Weight;
                ResetVelocity(user);
            }

            user.GetComponent<Inventory>().Add(magazine);
            magazine = null;
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

        private void ResetVelocity(GameObject user)
        {
            user.GetComponent<Variables>().declarations.Set("FirearmSpeed", 1f - weight * DeceleratePercentPerWeight);
        }

        private void CancelVelocity(GameObject user)
        {
            user.GetComponent<Variables>().declarations.Set("FirearmSpeed", 1f);
        }

        private void InvokeKick(GameObject user)
        {
            kickingTime = temple.Loading + KickTime;
            if (kicking) return;
            kicking = true;
            var speed = user.GetComponent<Variables>().declarations.Get<float>("Speed");
            user.GetComponent<Variables>().declarations.Set("Speed", speed / temple.Kick);
            deviation *= temple.Kick;
        }

        private void CancelKick(GameObject user)
        {
            kickingTime = .0f;
            if (!kicking) return;
            kicking = false;
            var speed = user.GetComponent<Variables>().declarations.Get<float>("Speed");
            user.GetComponent<Variables>().declarations.Set("Speed", speed * temple.Kick);
            deviation /= temple.Kick;
        }
    }

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
            return count == 0 ? new Magazine(Temple) {Containing = Containing, fetched = true} : this;
        }

        public Func<IItemStack, bool> SubInventory()
        {
            return item =>
                item.GetType() == typeof(Bullet) &&
                Math.Abs(((Bullet) item).Temple.Length - Temple.Length) < 0.000001f &&
                Math.Abs(((Bullet) item).Temple.Radius - Temple.Radius) < 0.000001f;
        }
    }

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

    public class Bullet : Material
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

        public override string GetName()
        {
            return Filler == null
                ? $"{Temple.Name}({Count})"
                : $"{Temple.Name}({Filler.GetFillerName()})({Count})";
        }

        public override int GetCount()
        {
            return Count;
        }

        public override bool CanMerge(IItemStack item)
        {
            if (item.GetType() != typeof(Bullet) || ((Bullet) item).Temple != Temple) return false;
            var bullet = (Bullet) item;
            if (Filler == null) return bullet.Filler == null;
            return Filler.Equals(bullet.Filler);
        }

        public override void Merge(IItemStack item)
        {
            Count += ((Bullet) item).Count;
            Filler?.CountAppend(((Bullet) item).Filler.GetCount());
        }

        public override IItemStack Fetch(int count)
        {
            if (count > Count) count = Count;
            Count -= count;
            return new Bullet(Temple, count, Filler);
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