using System;
using System.Linq;
using Bolt;
using UnityEngine;

namespace SubjectModel
{
    public static class GunDictionary
    {
        /*
        private static List<Firearm> defaultInventory;
        
        public static List<Firearm> GetDefaultInventory()
        {
            if (defaultInventory != null) return defaultInventory;
            //defaultInventory = JsonUtility.FromJson<GunData>(Resources.Load<TextAsset>("Firearms").text).guns;
            defaultInventory = new List<Firearm>();
            var defaultFirearm = new Firearm("Debug Firearm");
            defaultFirearm.AddComponent(new DebugComponent());
            defaultInventory.Add(defaultFirearm);
            return defaultInventory;
        }
        
        [MenuItem("Data/Generate Firearms Data")]
        private static void GenerateFirearmsData()
        {
            var data = new GunData
            {
                guns = new List<Gun>
                {
                    new Gun {name = "模板1", damage = 90f, loadingTime = .8f, clipCapacity = 4, clipSwitchTime = 5f}
                }
            };

            var path = Path.Combine(Application.dataPath, "SubjectModel/Resources/Data/Firearms.json");
            File.WriteAllText(path, JsonUtility.ToJson(data));
        }
        */
    }

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
        public readonly MagazineTemple[] Magazine;

        public FirearmTemple(string name, float damage, float reload, float loading, float weight, float depth,
            float deviation, float maxRange, float kick, float distance, float reloadSpeed, MagazineTemple[] magazine)
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

    public class Firearm : ItemStack
    {
        public readonly FirearmTemple Temple;
        public Magazine Magazine;
        public Magazine ReadyMagazine;
        public float Loading;
        public bool SwitchingMagazine;
        public float Weight;
        private bool fetched;

        public Firearm(FirearmTemple temple)
        {
            Temple = temple;
            Magazine = null;
            fetched = false;
            Weight = temple.Weight;
        }

        public string GetName()
        {
            return Magazine == null ? Temple.Name : $"{Temple.Name} {Magazine.BulletRemain}/{Magazine.Temple.BulletContains}";
        }

        public void OnMouseClickLeft(GameObject user, Vector2 aim)
        {
            if (Loading > .0f || Magazine == null || Magazine.BulletRemain <= 0 || Camera.main == null) return;
            aim.x = Utils.GenerateGaussian(aim.x, Temple.Deviation * Temple.Distance, Temple.MaxRange);
            aim.y = Utils.GenerateGaussian(aim.y, Temple.Deviation * Temple.Distance, Temple.MaxRange);
            var shooterPosition = user.GetComponent<Rigidbody2D>().position;
            if (shooterPosition == aim) return;

            Magazine.BulletRemain--;
            if (Magazine.BulletRemain != 0) Loading = Temple.Loading;
            var collider = user.GetComponent<GunFlash>().Shoot(shooterPosition, aim);
            if (collider == null) return;
            var declarations = collider.GetComponent<Variables>().declarations;
            var defence = declarations.IsDefined("Defence") ? declarations.Get<float>("Defence") : .0f;
            var health = declarations.Get<float>("Health");
            declarations.Set("Health",
                health - (defence > Temple.Depth
                    ? Utils.Map(.0f, defence, .0f, Temple.Damage, Temple.Depth)
                    : Temple.Damage));
        }

        public void OnMouseClickRight(GameObject user, Vector2 aim)
        {
            SwitchMagazine(user);
        }

        public void Selecting(GameObject user)
        {
            Loading -= Time.deltaTime;
            if (!(Loading <= .0f)) return;
            Loading = .0f;
            if (!SwitchingMagazine) return;
            SwitchingMagazine = false;
            Magazine = ReadyMagazine;
            Weight += Magazine.Temple.Weight;
            ResetVelocity(user);
            user.GetComponent<Inventory>().Remove(ReadyMagazine);
            ReadyMagazine = null;
            var declarations = user.GetComponent<Variables>().declarations;
            declarations.Set("Speed", declarations.Get<float>("Speed") / Temple.ReloadSpeed);
        }

        public void OnSelected(GameObject user)
        {
            ResetVelocity(user);
            user.GetComponent<GunFlash>().distance = Temple.Distance;
            user.GetComponent<GunFlash>().enabled = true;
        }

        public void LoseSelected(GameObject user)
        {
            CancelVelocity(user);
            user.GetComponent<LineRenderer>().enabled = false;
            user.GetComponent<GunFlash>().enabled = false;
            if (!SwitchingMagazine) return;
            SwitchingMagazine = false;
            Loading = .0f;
            ReadyMagazine = null;
            var declarations = user.GetComponent<Variables>().declarations;
            declarations.Set("Speed", declarations.Get<float>("Speed") / Temple.ReloadSpeed);
        }

        public Func<ItemStack, bool> SubInventory()
        {
            return item => item.GetType() == typeof(Magazine) && Temple.Magazine.Contains(((Magazine) item).Temple);
        }

        private void SwitchMagazine(GameObject user)
        {
            if (SwitchingMagazine || !user.GetComponent<Inventory>().TryGetSubItem(out var ready) ||
                ready.GetType() != typeof(Magazine)) return;
            ReadyMagazine = (Magazine) ready;
            SwitchingMagazine = true;
            Loading = Temple.Reload;
            var declarations = user.GetComponent<Variables>().declarations;
            declarations.Set("Speed", declarations.Get<float>("Speed") * Temple.ReloadSpeed);
            if (Magazine != null)
            {
                Weight -= Magazine.Temple.Weight;
                ResetVelocity(user);
            }
            user.GetComponent<Inventory>().Add(Magazine);
            Magazine = null;
        }

        public int GetCount()
        {
            return fetched ? 0 : 1;
        }

        public ItemStack Fetch()
        {
            fetched = true;
            return this;
        }

        private void ResetVelocity(GameObject user)
        {
            user.GetComponent<Variables>().declarations.Set("FirearmSpeed", 1f - Weight * 0.08f);
        }

        private void CancelVelocity(GameObject user)
        {
            user.GetComponent<Variables>().declarations.Set("FirearmSpeed", 1f);
        }
    }

    [Serializable]
    public class MagazineTemple
    {
        public readonly string Name;
        public readonly int BulletContains;
        public readonly float Weight;

        public MagazineTemple(string name, int bulletContains, float weight)
        {
            Name = name;
            BulletContains = bulletContains;
            Weight = weight;
        }
    }

    public class Magazine : Material
    {
        public readonly MagazineTemple Temple;
        public int BulletRemain;
        private bool fetched;

        public Magazine(MagazineTemple temple)
        {
            Temple = temple;
            BulletRemain = 0;
            fetched = false;
        }

        public override string GetName()
        {
            return $"{Temple.Name} {BulletRemain}/{Temple.BulletContains}";
        }

        public override int GetCount()
        {
            return fetched ? 0 : 1;
        }

        public override ItemStack Fetch()
        {
            fetched = true;
            return this;
        }
    }

    [Serializable]
    public class GunData
    {
        //public List<> guns;
    }
}