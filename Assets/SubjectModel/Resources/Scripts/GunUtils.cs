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

    public class Firearm : ITemStack
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
            return magazine == null ? temple.Name : $"{temple.Name} {magazine.BulletRemain}/{magazine.Temple.BulletContains}";
        }

        public void OnMouseClickLeft(GameObject user, Vector2 aim)
        {
            if (loading > .0f || magazine == null || magazine.BulletRemain <= 0 || Camera.main == null) return;
            aim.x = Utils.GenerateGaussian(aim.x, deviation * temple.Distance, temple.MaxRange);
            aim.y = Utils.GenerateGaussian(aim.y, deviation * temple.Distance, temple.MaxRange);
            var shooterPosition = user.GetComponent<Rigidbody2D>().position;
            if (shooterPosition == aim) return;
            Debug.Log(deviation);

            magazine.BulletRemain--;
            if (magazine.BulletRemain != 0) loading = temple.Loading;
            InvokeKick(user);
            var collider = user.GetComponent<GunFlash>().Shoot(shooterPosition, aim);
            if (collider == null) return;
            var declarations = collider.GetComponent<Variables>().declarations;
            var defence = declarations.IsDefined("Defence") ? declarations.Get<float>("Defence") : .0f;
            var health = declarations.Get<float>("Health");
            declarations.Set("Health",
                health - (defence > temple.Depth
                    ? Utils.Map(.0f, defence, .0f, temple.Damage, temple.Depth)
                    : temple.Damage));
        }

        public void OnMouseClickLeftDown(GameObject user, Vector2 pos) { }

        public void OnMouseClickRight(GameObject user, Vector2 pos) { }

        public void OnMouseClickRightDown(GameObject user, Vector2 aim)
        {
            SwitchMagazine(user);
        }

        public void OnMouseClickLeftUp(GameObject user, Vector2 pos) { }

        public void OnMouseClickRightUp(GameObject user, Vector2 pos) { }

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

        public Func<ITemStack, bool> SubInventory()
        {
            return item => item.GetType() == typeof(Magazine) && temple.Magazine.Contains(((Magazine) item).Temple);
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

        public ITemStack Fetch()
        {
            fetched = true;
            return this;
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

        public override ITemStack Fetch()
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