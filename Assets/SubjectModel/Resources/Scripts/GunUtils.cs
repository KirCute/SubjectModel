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
                    : $"{temple.Name}({magazine.Containing.Temple.Name} {magazine.Containing.Count}/{magazine.Temple.BulletContains})";
        }

        public void OnMouseClickLeft(GameObject user, Vector2 aim)
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
            if (magazine.Containing.Count == 0) magazine.Containing = null;
            if (collider == null) return;
            var declarations = collider.GetComponent<Variables>().declarations;
            var defence = declarations.IsDefined("Defence") ? declarations.Get<float>("Defence") : .0f;
            var health = declarations.Get<float>("Health");
            var depth = temple.Depth + magazine.Containing.Temple.Depth;
            var damage = temple.Damage * magazine.Containing.Temple.Damage;
            declarations.Set("Health",
                health - (defence > depth
                    ? Utils.Map(.0f, defence, .0f, damage, depth)
                    : damage));
        }

        public void OnMouseClickLeftDown(GameObject user, Vector2 pos)
        {
        }

        public void OnMouseClickRight(GameObject user, Vector2 pos)
        {
        }

        public void OnMouseClickRightDown(GameObject user, Vector2 aim)
        {
            SwitchMagazine(user);
        }

        public void OnMouseClickLeftUp(GameObject user, Vector2 pos)
        {
        }

        public void OnMouseClickRightUp(GameObject user, Vector2 pos)
        {
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
            return count == 0 ? new Firearm(temple) {fetched = true} : this;
        }

        public Func<IItemStack, bool> SubInventory()
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
                : $"{Temple.Name}({Containing.Temple.Name} {Containing.Count}/{Temple.BulletContains})";
        }

        public void OnMouseClickLeft(GameObject user, Vector2 pos)
        {
        }

        public void OnMouseClickRight(GameObject user, Vector2 pos)
        {
        }

        public void OnMouseClickLeftDown(GameObject user, Vector2 pos)
        {
            user.GetComponent<Inventory>().Add(Containing);
            Containing = null;
        }

        public void OnMouseClickRightDown(GameObject user, Vector2 pos)
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

        public void OnMouseClickLeftUp(GameObject user, Vector2 pos)
        {
        }

        public void OnMouseClickRightUp(GameObject user, Vector2 pos)
        {
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
        public readonly float Damage;
        public readonly float Depth;
        public readonly float Radius;
        public readonly float Length;

        public BulletTemple(string name, float damage, float depth, float radius, float length)
        {
            Name = name;
            Damage = damage;
            Depth = depth;
            Radius = radius;
            Length = length;
        }
    }

    public class Bullet : Material
    {
        public readonly BulletTemple Temple;
        public int Count;

        public Bullet(BulletTemple temple, int count)
        {
            Temple = temple;
            Count = count;
        }

        public override string GetName()
        {
            return $"{Temple.Name}({Count})";
        }

        public override int GetCount()
        {
            return Count;
        }

        public override bool CanMerge(IItemStack item)
        {
            return item.GetType() == typeof(Bullet) && ((Bullet) item).Temple == Temple;
        }

        public override void Merge(IItemStack item)
        {
            Count += ((Bullet) item).Count;
        }

        public override IItemStack Fetch(int count)
        {
            if (count > Count) count = Count;
            Count -= count;
            return new Bullet(Temple, count);
        }
    }

    [Serializable]
    public class GunData
    {
        //public List<> guns;
    }
}