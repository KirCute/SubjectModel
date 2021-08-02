using System;
using System.Linq;
using Bolt;
using SubjectModel.Scripts.InventorySystem;
using UnityEngine;

namespace SubjectModel.Scripts.Firearms
{
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
            return count == 0 ? new Firearm(temple) {fetched = true} : new Firearm(temple);
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
}