using System;
using Bolt;
using SubjectModel.Scripts.InventorySystem;
using UnityEngine;

namespace SubjectModel.Scripts.Subject.Firearms
{
    /**
     * <summary>
     * 枪支的模板
     * 可序列化，在枪支实例创建时作为参数。
     * </summary>
     */
    [Serializable]
    public class FirearmTemple
    {
        public readonly string Name;
        public readonly int Type;
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

        public FirearmTemple(string name, int type, float damage, float reload, float loading, float weight,
            float depth, float deviation, float maxRange, float kick, float distance, float reloadSpeed,
            string[] magazine)
        {
            Name = name;
            Type = type;
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

    /**
     * <summary>
     * 枪支
     * 使用函数式编程思想，添加用法不同于AR的枪支，需在GunUtils中修改有关内容
     * </summary>
     */
    public class Firearm : Weapon
    {
        private const float KickTime = .01f;
        private const float DeceleratePercentPerWeight = .08f;
        public static double KickPowerForDeviation = 3.0;

        public readonly FirearmTemple Temple;
        public Magazine Magazine;
        public IItemStack Ready;
        public float Loading;
        public float Deviation;
        public bool SwitchingMagazine;
        private float weight;
        private float kickingTime;
        private bool kicking;
        public Func<Firearm, string> NameGetter = FirearmDictionary.DefaultName;
        public Action<Firearm, GameObject, Vector2> ShootKeep = FirearmDictionary.NoShoot;
        public Action<Firearm, GameObject, Vector2> ShootOnce = FirearmDictionary.NoShoot;
        public Action<Firearm, GameObject> Reload = FirearmDictionary.NoReload;
        public Action<Firearm, GameObject> ReloadKeep = FirearmDictionary.NoReload;
        public Func<Firearm, Func<IItemStack, bool>> Sub = FirearmDictionary.DefaultSub;
        public Action<Firearm, GameObject> CompleteReload = FirearmDictionary.DefaultCompleteReload;
        public override string Name => NameGetter(this);

        public Firearm(FirearmTemple temple)
        {
            Temple = temple;
            Magazine = null;
            Deviation = temple.Deviation;
            weight = temple.Weight;
            FirearmDictionary.FirearmBuilder[temple.Type](this);
        }
        
        public override void OnMasterUseKeep(GameObject user, Vector2 aim)
        {
            ShootKeep(this, user, aim);
        }

        public override void OnMasterUseOnce(GameObject user, Vector2 aim)
        {
            ShootOnce(this, user, aim);
        }

        public override void OnSlaveUseKeep(GameObject user)
        {
            ReloadKeep(this, user);
        }

        public override void OnSlaveUseOnce(GameObject user)
        {
            Reload(this, user);
        }

        public override void Selecting(GameObject user)
        {
            Loading -= Time.deltaTime;
            kickingTime -= Time.deltaTime;
            user.GetComponent<GunFlash>().shootAble = Loading <= .0f && Magazine?.Containing != null;
            if (kickingTime <= .0f) CancelKick(user);
            if (Loading > .0f) return;
            Loading = .0f;
            if (!SwitchingMagazine) return;
            SwitchingMagazine = false;
            CompleteReload(this, user);
        }

        public override void OnSelected(GameObject user)
        {
            ResetVelocity(user);
            user.GetComponent<GunFlash>().distance = Temple.Distance;
            user.GetComponent<GunFlash>().enabled = true;
        }

        public override void LoseSelected(GameObject user)
        {
            CancelVelocity(user);
            CancelKick(user);
            user.GetComponent<LineRenderer>().enabled = false;
            user.GetComponent<GunFlash>().enabled = false;
            if (!SwitchingMagazine) return;
            SwitchingMagazine = false;
            Loading = .0f;
            Ready = null;
            var declarations = user.GetComponent<Variables>().declarations;
            declarations.Set("Speed", declarations.Get<float>("Speed") / Temple.ReloadSpeed);
        }

        public override IItemStack Fetch(int count)
        {
            base.Fetch(count);
            return new Firearm(Temple);
        }

        public override Func<IItemStack, bool> SubInventory()
        {
            return Sub(this);
        }

        public void AddWeight(GameObject user, float value)
        {
            CancelVelocity(user);
            weight += value;
            ResetVelocity(user);
        }

        public void InvokeKick(GameObject user)
        {
            kickingTime = Temple.Loading + KickTime;
            if (kicking) return;
            kicking = true;
            var speed = user.GetComponent<Variables>().declarations.Get<float>("Speed");
            user.GetComponent<Variables>().declarations.Set("Speed", speed * Temple.Kick);
            Deviation /= (float) Math.Pow(Temple.Kick, KickPowerForDeviation);
        }

        private void CancelKick(GameObject user)
        {
            kickingTime = .0f;
            if (!kicking) return;
            kicking = false;
            var speed = user.GetComponent<Variables>().declarations.Get<float>("Speed");
            user.GetComponent<Variables>().declarations.Set("Speed", speed / Temple.Kick);
            Deviation *= (float) Math.Pow(Temple.Kick, KickPowerForDeviation);
        }

        private void ResetVelocity(GameObject user)
        {
            var speed = user.GetComponent<Variables>().declarations.Get<float>("Speed");
            user.GetComponent<Variables>().declarations
                .Set("Speed", speed * (1f - weight * DeceleratePercentPerWeight));
        }

        private void CancelVelocity(GameObject user)
        {
            var speed = user.GetComponent<Variables>().declarations.Get<float>("Speed");
            user.GetComponent<Variables>().declarations
                .Set("Speed", speed / (1f - weight * DeceleratePercentPerWeight));
        }
    }
}