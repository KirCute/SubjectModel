using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bolt;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

namespace SubjectModel
{
    internal struct Pair<T>
    {
        public T First;
        public T Second;
    }

    public class TestGui : MonoBehaviour
    {
        private int selected;
        private Variables playerVariables;

        private string maxHealth;
        private float health;
        private bool healthUpdate;
        private string maxStrength;
        private float energy;
        private bool energyUpdate;
        private float strength;
        private bool strengthUpdate;
        private string speed;
        private bool speedUpdate;
        private string defence;
        private bool defenceUpdate;
        private string firearmSpeed;
        private bool firearmSpeedUpdate;

        private BossSpawner bossSpawner;
        private string bossHealth;
        private string bossSpeed;
        private string bossDefence;

        private string mdc;
        private string tdc;
        private string mdp;

        private string firearmName;
        private string damage;
        private string reload;
        private string loading;
        private string weight;
        private string depth;
        private string deviation;
        private string maxRange;
        private string kick;
        private string distance;
        private string reloadSpeed;
        private string magazineTemple;
        private Vector2 firearmScroll;

        private string magazineName;
        private string bulletContains;
        private string magazineWeight;
        private string magazineRadius;
        private string magazineLength;
        private Vector2 magazineScroll;

        private string bulletName;
        private string bulletBreak;
        private string bulletExplode;
        private string bulletDepth;
        private string bulletMinDefence;
        private string bulletRadius;
        private string bulletLength;
        private Vector2 bulletScroll;

        private Inventory inventory;
        private GunFlash playerFlash;
        private string firearm;
        private string magazine;
        private string bullet;
        private string bulletCount;

        private Vector2 inventoryScroll;

        private bool standOnly;
        private bool standOnlyInvoke;
        private bool pause;

        private void Awake()
        { 
            LoadModel();
        }

        private void Start()
        {
            selected = 5;
            var player = GameObject.FindWithTag("Player");
            playerVariables = player.GetComponent<Variables>();
            maxHealth = playerVariables.declarations.Get("MaxHealth").ToString();
            health = playerVariables.declarations.Get<float>("Health");
            healthUpdate = false;
            maxStrength = playerVariables.declarations.Get("MaxStrength").ToString();
            energy = playerVariables.declarations.Get<float>("Energy");
            energyUpdate = false;
            strength = playerVariables.declarations.Get<float>("Strength");
            strengthUpdate = false;
            speed = playerVariables.declarations.Get("Speed").ToString();
            speedUpdate = false;
            defence = playerVariables.declarations.Get("Defence").ToString();
            defenceUpdate = false;
            speed = playerVariables.declarations.Get("FirearmSpeed").ToString();
            speedUpdate = false;
            bossSpawner = GameObject.FindWithTag("BossSpawner").GetComponent<BossSpawner>();
            bossHealth = bossSpawner.bossHealth.ToString();
            bossSpeed = bossSpawner.bossSpeed.ToString();
            bossDefence = bossSpawner.bossDefence.ToString();
            firearmScroll = new Vector2(0f, 0f);
            magazineScroll = new Vector2(0f, 0f);
            bulletScroll = new Vector2(0f, 0f);
            mdc = BuffRenderer.MotiveDamageCoefficient.ToString();
            tdc = BuffRenderer.ThermalDamageCoefficient.ToString();
            mdp = BuffRenderer.MinimumDamagePotential.ToString();
            inventoryScroll = new Vector2(0f, 0f);
            standOnly = false;
            standOnlyInvoke = false;
            pause = false;
            inventory = player.GetComponent<Inventory>();
            playerFlash = player.GetComponent<GunFlash>();
        }

        private void Update()
        {
            if (float.TryParse(bossHealth, out var value)) bossSpawner.bossHealth = value;
            if (float.TryParse(bossSpeed, out value)) bossSpawner.bossSpeed = value;
            if (float.TryParse(bossDefence, out value)) bossSpawner.bossDefence = value;
            if (float.TryParse(mdc, out value)) BuffRenderer.MotiveDamageCoefficient = value;
            if (float.TryParse(tdc, out value)) BuffRenderer.ThermalDamageCoefficient = value;
            if (float.TryParse(mdp, out value)) BuffRenderer.MinimumDamagePotential = value;
            if (standOnly != standOnlyInvoke)
            {
                standOnlyInvoke = standOnly;
                playerVariables.declarations.Set("Standonly",
                    playerVariables.declarations.Get<int>("Standonly") + (standOnly ? 1 : -1));
            }

            Time.timeScale = pause ? 0f : 1f;
        }

        private void OnGUI()
        {
            GUILayout.Window(0, new Rect(60, 80, 500, 20), id =>
            {
                selected = GUILayout.Toolbar(selected, new[] {"玩家", "敌人", "炼金术", "枪械", "物品栏", "收起", "系统"});
                switch (selected)
                {
                    case 0:
                        if (GUILayout.Button("回到重生点"))
                            playerVariables.declarations.Set("Health", .0f);
                        ManualAdjustString("血量上限", ref maxHealth, "MaxHealth", playerVariables);
                        ManualAdjustSlider("血量", ref health, ref healthUpdate, "MaxHealth", "Health", playerVariables);
                        ManualAdjustString("精神上限", ref maxStrength, "MaxStrength", playerVariables);
                        ManualAdjustSlider("精神", ref energy, ref energyUpdate, "MaxStrength", "Energy",
                            playerVariables);
                        ManualAdjustSlider("体力", ref strength, ref strengthUpdate, "Energy", "Strength",
                            playerVariables);
                        ManualAdjustFloatUpdating("移动速度", ref speed, ref speedUpdate, "Speed", playerVariables);
                        ManualAdjustFloatUpdating("防御", ref defence, ref defenceUpdate, "Defence", playerVariables);
                        ManualAdjustFloatUpdating("生效速度百分比", ref firearmSpeed, ref firearmSpeedUpdate, "FirearmSpeed",
                            playerVariables);
                        break;
                    case 1:
                        AutoAdjustString("血量", ref bossHealth);
                        AutoAdjustString("移动速度", ref bossSpeed);
                        AutoAdjustString("防御", ref bossDefence);
                        if (GUILayout.Button("重新生成Boss"))
                        {
                            GameObject boss;
                            if ((boss = GameObject.FindWithTag("Boss")) != null)
                                boss.GetComponent<Variables>().declarations.Set("Health", .0f);
                            GameObject.FindWithTag("BossAssistance").GetComponent<BossAssistance>().BossDead();
                            bossSpawner.GetComponent<BoxCollider2D>().enabled = true;
                        }

                        break;
                    case 2:
                        AutoAdjustString("动力伤害系数", ref mdc);
                        AutoAdjustString("热力伤害系数", ref tdc);
                        AutoAdjustString("最小热力伤害电势差", ref mdp);
                        break;
                    case 3:
                        GUILayout.BeginVertical();
                        GUILayout.BeginVertical("Box");
                        GUILayout.Label("枪械模板");
                        firearmScroll = GUILayout.BeginScrollView(firearmScroll, false, false, GUILayout.Height(100));
                        for (var i = 0; i < Test.FirearmTemples.Count; i++)
                        {
                            GUILayout.BeginHorizontal("Box");
                            GUILayout.Label($"{i} - {Test.FirearmTemples[i].Name}");
                            GUILayout.Label(Test.FirearmTemples[i].Damage.ToString());
                            GUILayout.Label(Test.FirearmTemples[i].Depth.ToString());
                            GUILayout.Label($"{Test.FirearmTemples[i].Loading}({Test.FirearmTemples[i].Reload})");
                            GUILayout.Label($"{Test.FirearmTemples[i].Deviation}({Test.FirearmTemples[i].MaxRange})");
                            GUILayout.Label(Test.FirearmTemples[i].Weight.ToString());
                            if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
                            {
                                Test.FirearmTemples.RemoveAt(i);
                                i--;
                            }

                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndScrollView();
                        GUILayout.BeginVertical("Box");
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("名称：", GUILayout.ExpandWidth(false));
                        firearmName = GUILayout.TextField(firearmName);
                        GUILayout.Label("使用弹匣id：", GUILayout.ExpandWidth(false));
                        magazineTemple = GUILayout.TextField(magazineTemple);
                        if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                            Test.FirearmTemples.Add(new FirearmTemple(firearmName, float.Parse(damage),
                                float.Parse(reload),
                                float.Parse(loading), float.Parse(weight), float.Parse(depth), float.Parse(deviation),
                                float.Parse(maxRange), float.Parse(kick), float.Parse(distance),
                                float.Parse(reloadSpeed),
                                SplitTempleString(magazineTemple).Select(i => Test.MagazineTemples[i].Name).ToArray()));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("伤害倍率：", GUILayout.ExpandWidth(false));
                        damage = GUILayout.TextField(damage, GUILayout.ExpandWidth(true));
                        GUILayout.Label("穿深：", GUILayout.ExpandWidth(false));
                        depth = GUILayout.TextField(depth, GUILayout.ExpandWidth(true));
                        GUILayout.Label("换弹匣时间：", GUILayout.ExpandWidth(false));
                        reload = GUILayout.TextField(reload, GUILayout.ExpandWidth(true));
                        GUILayout.Label("自动换弹时间：", GUILayout.ExpandWidth(false));
                        loading = GUILayout.TextField(loading, GUILayout.ExpandWidth(true));
                        GUILayout.Label("重量：", GUILayout.ExpandWidth(false));
                        weight = GUILayout.TextField(weight, GUILayout.ExpandWidth(true));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("精度：", GUILayout.ExpandWidth(false));
                        deviation = GUILayout.TextField(deviation, GUILayout.ExpandWidth(true));
                        GUILayout.Label("散步：", GUILayout.ExpandWidth(false));
                        maxRange = GUILayout.TextField(maxRange, GUILayout.ExpandWidth(true));
                        GUILayout.Label("换弹匣降速：", GUILayout.ExpandWidth(false));
                        reloadSpeed = GUILayout.TextField(reloadSpeed, GUILayout.ExpandWidth(true));
                        GUILayout.Label("后坐力：", GUILayout.ExpandWidth(false));
                        kick = GUILayout.TextField(kick, GUILayout.ExpandWidth(true));
                        GUILayout.Label("射程：", GUILayout.ExpandWidth(false));
                        distance = GUILayout.TextField(distance, GUILayout.ExpandWidth(true));
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUILayout.EndVertical();
                        GUILayout.BeginVertical("Box");
                        GUILayout.Label("弹匣模板");
                        magazineScroll = GUILayout.BeginScrollView(magazineScroll, false, false, GUILayout.Height(100));
                        for (var i = 0; i < Test.MagazineTemples.Count; i++)
                        {
                            GUILayout.BeginHorizontal("Box");
                            GUILayout.Label($"{i} - {Test.MagazineTemples[i].Name}");
                            GUILayout.Label(Test.MagazineTemples[i].BulletContains.ToString());
                            GUILayout.Label($"{Test.MagazineTemples[i].Radius}*{Test.MagazineTemples[i].Length}");
                            GUILayout.Label(Test.MagazineTemples[i].Weight.ToString());
                            if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
                            {
                                Test.MagazineTemples.RemoveAt(i);
                                i--;
                            }

                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndScrollView();
                        GUILayout.BeginHorizontal("Box");
                        GUILayout.Label("名称：", GUILayout.ExpandWidth(false));
                        magazineName = GUILayout.TextField(magazineName, GUILayout.ExpandWidth(true));
                        GUILayout.Label("载弹量：", GUILayout.ExpandWidth(false));
                        bulletContains = GUILayout.TextField(bulletContains, GUILayout.ExpandWidth(true));
                        GUILayout.Label("重量：", GUILayout.ExpandWidth(false));
                        magazineWeight = GUILayout.TextField(magazineWeight, GUILayout.ExpandWidth(true));
                        GUILayout.Label("口径：", GUILayout.ExpandWidth(false));
                        magazineRadius = GUILayout.TextField(magazineRadius, GUILayout.ExpandWidth(true));
                        GUILayout.Label("长度：", GUILayout.ExpandWidth(false));
                        magazineLength = GUILayout.TextField(magazineLength, GUILayout.ExpandWidth(true));
                        if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                            Test.MagazineTemples.Add(new MagazineTemple(magazineName, int.Parse(bulletContains),
                                float.Parse(magazineWeight), float.Parse(magazineRadius), float.Parse(magazineLength)));
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUILayout.BeginVertical("Box");
                        GUILayout.Label("子弹模板");
                        bulletScroll = GUILayout.BeginScrollView(bulletScroll, false, false, GUILayout.Height(100));
                        for (var i = 0; i < Test.BulletTemples.Count; i++)
                        {
                            GUILayout.BeginHorizontal("Box");
                            GUILayout.Label($"{i} - {Test.BulletTemples[i].Name}");
                            GUILayout.Label(
                                $"{Test.BulletTemples[i].BreakDamage}(+{Test.BulletTemples[i].Explode})");
                            GUILayout.Label($"{Test.BulletTemples[i].Depth}({Test.BulletTemples[i].MinDefence})");
                            GUILayout.Label($"{Test.BulletTemples[i].Radius}*{Test.BulletTemples[i].Length}");
                            if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
                            {
                                Test.BulletTemples.RemoveAt(i);
                                i--;
                            }

                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndScrollView();
                        GUILayout.BeginVertical("Box");
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("名称：", GUILayout.ExpandWidth(false));
                        bulletName = GUILayout.TextField(bulletName, GUILayout.ExpandWidth(true));
                        GUILayout.Label("基础伤害：", GUILayout.ExpandWidth(false));
                        bulletBreak = GUILayout.TextField(bulletBreak, GUILayout.ExpandWidth(true));
                        GUILayout.Label("击穿伤害：", GUILayout.ExpandWidth(false));
                        bulletExplode = GUILayout.TextField(bulletExplode, GUILayout.ExpandWidth(true));
                        GUILayout.Label("穿深：", GUILayout.ExpandWidth(false));
                        bulletDepth = GUILayout.TextField(bulletDepth, GUILayout.ExpandWidth(true));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("过穿防御：", GUILayout.ExpandWidth(false));
                        bulletMinDefence = GUILayout.TextField(bulletMinDefence, GUILayout.ExpandWidth(true));
                        GUILayout.Label("口径：", GUILayout.ExpandWidth(false));
                        bulletRadius = GUILayout.TextField(bulletRadius, GUILayout.ExpandWidth(true));
                        GUILayout.Label("长度：", GUILayout.ExpandWidth(false));
                        bulletLength = GUILayout.TextField(bulletLength, GUILayout.ExpandWidth(true));
                        if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                            Test.BulletTemples.Add(new BulletTemple(bulletName, float.Parse(bulletBreak),
                                float.Parse(bulletExplode), float.Parse(bulletDepth), float.Parse(bulletMinDefence),
                                float.Parse(bulletRadius), float.Parse(bulletLength)));
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUILayout.EndVertical();
                        GUILayout.BeginVertical("Box");
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("枪械物品栏    ID：", GUILayout.ExpandWidth(false));
                        firearm = GUILayout.TextField(firearm, GUILayout.ExpandWidth(true));
                        if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                            inventory.Add(new Firearm(Test.FirearmTemples[int.Parse(firearm)]));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("弹匣物品栏    ID：", GUILayout.ExpandWidth(false));
                        magazine = GUILayout.TextField(magazine, GUILayout.ExpandWidth(true));
                        if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                        {
                            var index = int.Parse(magazine);
                            inventory.Add(new Magazine(Test.MagazineTemples[index]));
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("子弹物品栏    ID：", GUILayout.ExpandWidth(false));
                        bullet = GUILayout.TextField(bullet);
                        GUILayout.Label("数量：", GUILayout.ExpandWidth(false));
                        bulletCount = GUILayout.TextField(bulletCount);
                        if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                        {
                            var index = int.Parse(bullet);
                            var count = int.Parse(bulletCount);
                            inventory.Add(new Bullet(Test.BulletTemples[index], count));
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUILayout.BeginVertical("Box");
                        GUILayout.BeginHorizontal();
                        //if (GUILayout.Button("重新加载模板数据")) LoadModel();
                        if (GUILayout.Button("保存模板数据")) SaveModel();
                        GUILayout.EndHorizontal();
                        playerFlash.sight = GUILayout.Toggle(playerFlash.sight, "使用瞄具");
                        GUILayout.EndVertical();
                        GUILayout.EndVertical();
                        break;
                    case 4:
                        inventoryScroll =
                            GUILayout.BeginScrollView(inventoryScroll, false, false, GUILayout.Height(600));
                        for (var i = 0; i < inventory.bag.Count; i++)
                        {
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("·", GUILayout.ExpandWidth(false)))
                            {
                                inventory.selecting = i;
                                inventory.subSelecting = 0;
                                inventory.RebuildSubInventory();
                            }
                            GUILayout.Label($"{i} - {inventory.bag[i].GetName()}", GUILayout.ExpandWidth(true));
                            if (i != 0 && GUILayout.Button("↑", GUILayout.ExpandWidth(false)))
                            {
                                var front = inventory.bag[i - 1];
                                var behind = inventory.bag[i];
                                inventory.bag[i - 1] = behind;
                                inventory.bag[i] = front;
                                inventory.RebuildSubInventory();
                            }
                            if (i != inventory.bag.Count - 1 && GUILayout.Button("↓", GUILayout.ExpandWidth(false)))
                            {
                                var front = inventory.bag[i];
                                var behind = inventory.bag[i + 1];
                                inventory.bag[i] = behind;
                                inventory.bag[i + 1] = front;
                                inventory.RebuildSubInventory();
                            }
                            if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
                            {
                                inventory.Remove(inventory.bag[i]);
                                i--;
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndScrollView();
                        break;
                    case 6:
                        GUILayout.BeginVertical("Box");
                        standOnly = GUILayout.Toggle(standOnly, "锁定玩家操作");
                        pause = GUILayout.Toggle(pause, "暂停");
                        if (GUILayout.Button("退出游戏"))
                        {
#if UNITY_EDITOR
                            EditorApplication.isPlaying = false;
#else
                            Application.Quit();
#endif
                        }
                        GUILayout.EndVertical();
                        break;
                }
            }, "测试窗口");
        }

        private static void AutoAdjustString(string text, ref string value)
        {
            GUILayout.BeginHorizontal("Box");
            GUILayout.Label(text, GUILayout.ExpandWidth(false));
            value = GUILayout.TextField(value, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();
        }

        private static void ManualAdjustString(string text, ref string value, string targetName, Variables target)
        {
            var v = target.declarations.Get<float>(targetName);
            ManualAdjustString(text, ref value, ref v);
            target.declarations.Set(targetName, v);
        }

        private static void ManualAdjustString(string text, ref string value, ref float target)
        {
            GUILayout.BeginHorizontal("Box");
            GUILayout.Label(text, GUILayout.ExpandWidth(false));
            value = GUILayout.TextField(value, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("生效", GUILayout.ExpandWidth(false))) target = float.Parse(value);
            GUILayout.EndHorizontal();
        }

        private static void ManualAdjustIntUpdating(string text, ref string value, ref bool update, string targetName,
            Variables target)
        {
            var v = target.declarations.Get<int>(targetName);
            ManualAdjustIntUpdating(text, ref value, ref update, ref v);
            target.declarations.Set(targetName, v);
        }

        private static void ManualAdjustIntUpdating(string text, ref string value, ref bool update, ref int target)
        {
            GUILayout.BeginHorizontal("Box");
            GUILayout.Label(text, GUILayout.ExpandWidth(false));
            value = GUILayout.TextField(value, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("生效", GUILayout.ExpandWidth(false))) target = int.Parse(value);
            update = GUILayout.Toggle(update, "实时更新", GUILayout.ExpandWidth(false));
            if (update) value = target.ToString();
            GUILayout.EndHorizontal();
        }

        private static void ManualAdjustFloatUpdating(string text, ref string value, ref bool update, string targetName,
            Variables target)
        {
            var v = target.declarations.Get<float>(targetName);
            ManualAdjustFloatUpdating(text, ref value, ref update, ref v);
            target.declarations.Set(targetName, v);
        }

        private static void ManualAdjustFloatUpdating(string text, ref string value, ref bool update, ref float target)
        {
            GUILayout.BeginHorizontal("Box");
            GUILayout.Label(text, GUILayout.ExpandWidth(false));
            value = GUILayout.TextField(value, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("生效", GUILayout.ExpandWidth(false))) target = float.Parse(value);
            update = GUILayout.Toggle(update, "实时更新", GUILayout.ExpandWidth(false));
            if (update) value = target.ToString();
            GUILayout.EndHorizontal();
        }

        private static void ManualAdjustSlider(string text, ref float value, ref bool update, string maxName,
            string targetName, Variables target)
        {
            var max = target.declarations.Get<float>(maxName);
            var v = target.declarations.Get<float>(targetName);
            ManualAdjustSlider(text, ref value, ref update, max, ref v);
            target.declarations.Set(targetName, v);
        }

        private static void ManualAdjustSlider(string text, ref float value, ref bool update, float max,
            ref float target)
        {
            GUILayout.BeginHorizontal("Box");
            GUILayout.Label(text, GUILayout.ExpandWidth(false));
            value = GUILayout.HorizontalSlider(value, .0f, max, GUILayout.MinWidth(100));
            GUILayout.Label(Math.Round(value).ToString());
            if (GUILayout.Button("生效", GUILayout.ExpandWidth(false))) target = value;
            update = GUILayout.Toggle(update, "实时更新", GUILayout.ExpandWidth(false));
            if (update) value = target;
            GUILayout.EndHorizontal();
        }

        private static IEnumerable<int> SplitTempleString(string temple)
        {
            return temple.Split(',').Select(int.Parse).ToArray();
        }

        private static void LoadModel()
        {
            var origin = new StringBuilder(File.ReadAllText(Path.Combine(Application.dataPath, "Firearms.json")));
            origin.Replace("\n", "");
            origin.Replace("\t", "");
            var data = JsonConvert.DeserializeObject<GunData>(origin.ToString());
            Test.FirearmTemples = data == null ? new List<FirearmTemple>() : data.firearmTemples;
            Test.MagazineTemples = data == null ? new List<MagazineTemple>() : data.magazineTemples;
            Test.BulletTemples = data == null ? new List<BulletTemple>() : data.bulletTemples;
        }

        private static void SaveModel()
        {
            var path = Path.Combine(Application.dataPath, "Firearms.json");
            var backup = Path.Combine(Application.dataPath, "Firearms.bak");
            if (File.Exists(path))
            {
                if (File.Exists(backup)) File.Delete(backup);
                File.Move(path, backup);
            }
            var json = new StringBuilder(JsonConvert.SerializeObject(new GunData
            {
                firearmTemples = Test.FirearmTemples,
                magazineTemples = Test.MagazineTemples,
                bulletTemples = Test.BulletTemples
            }));
            var level = 0;
            var check = true;
            for (var i = 0; i < json.Length; i++)
            {
                if (json[i] == '"') check = !check;
                if (check) switch (json[i])
                {
                    case '{':
                    case '[':
                        level++;
                        for (var j = 0; j < level; j++) json.Insert(i + 1, "\t");
                        json.Insert(i + 1, "\n");
                        break;
                    case ',':
                        for (var j = 0; j < level; j++) json.Insert(i + 1, "\t");
                        json.Insert(i + 1, "\n");
                        break;
                    case '}':
                    case ']':
                        level--;
                        json.Insert(i, "\n");
                        i++;
                        for (var j = 0; j < level; j++)
                        {
                            json.Insert(i, "\t");
                            i++;
                        }
                        break;
                }
            }
            File.WriteAllText(path, json.ToString());
        }
    }
}