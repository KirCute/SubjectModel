using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Bolt;
using Newtonsoft.Json;
using SubjectModel.Scripts.Chemistry;
using SubjectModel.Scripts.Firearms;
using SubjectModel.Scripts.InventorySystem;
using SubjectModel.Scripts.System;
using SubjectModel.Scripts.Task;
using UnityEditor;
using UnityEngine;

namespace SubjectModel.Scripts.Development
{
    internal struct Pair<T>
    {
        public T First;
        public T Second;
    }

    public class TestGui : MonoBehaviour
    {
        private int selected;
        private VariableDeclarations playerVariables;

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

        private GameObject[] bossSpawners;
        private string bossHealth;
        private string bossSpeed;
        private string bossDefence;
        private string bossLocate;

        private string st;
        private string mdc;
        private string tdc;
        private string mdp;
        private string drugTag;
        private string properties;
        private string drugCount;
        private IList<IonStack> ions;
        private Vector2 ionsScroll;
        private string element;
        private string valence;
        private string amount;
        private string concentration;
        private string chemicalBullet;

        private string kpfd;
        private string firearmName;
        private string firearmType;
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

        private void Start()
        {
            selected = 5;
            var player = GameObject.FindWithTag("Player");
            playerVariables = player.GetComponent<Variables>().declarations;
            maxHealth = playerVariables.Get("MaxHealth").ToString();
            health = playerVariables.Get<float>("Health");
            healthUpdate = false;
            maxStrength = playerVariables.Get("MaxStrength").ToString();
            energy = playerVariables.Get<float>("Energy");
            energyUpdate = false;
            strength = playerVariables.Get<float>("Strength");
            strengthUpdate = false;
            speed = playerVariables.Get("Speed").ToString();
            speedUpdate = false;
            defence = playerVariables.Get("Defence").ToString();
            defenceUpdate = false;
            bossSpawners = GameObject.FindGameObjectsWithTag("BossSpawner");
            bossHealth = "200";
            bossSpeed = "5";
            bossDefence = "400";
            bossLocate = "3";
            firearmScroll = Vector2.zero;
            magazineScroll = Vector2.zero;
            bulletScroll = Vector2.zero;
            st = $"{BuffRenderer.StainTime}";
            mdc = $"{BuffRenderer.MotiveDamageCoefficient}";
            tdc = $"{BuffRenderer.ThermalDamageCoefficient}";
            mdp = $"{BuffRenderer.MinimumDamagePotential}";
            ions = new List<IonStack>();
            ionsScroll = Vector2.zero;
            inventoryScroll = Vector2.zero;
            kpfd = $"{Firearm.KickPowerForDeviation}";
            standOnly = false;
            standOnlyInvoke = false;
            pause = false;
            inventory = player.GetComponent<Inventory>();
            playerFlash = player.GetComponent<GunFlash>();
        }

        private void Update()
        {
            if (float.TryParse(st, out var floatValue)) BuffRenderer.StainTime = floatValue;
            if (float.TryParse(mdc, out floatValue)) BuffRenderer.MotiveDamageCoefficient = floatValue;
            if (float.TryParse(tdc, out floatValue)) BuffRenderer.ThermalDamageCoefficient = floatValue;
            if (float.TryParse(mdp, out floatValue)) BuffRenderer.MinimumDamagePotential = floatValue;
            if (float.TryParse(kpfd, out var doubleValue)) Firearm.KickPowerForDeviation = doubleValue;
            if (standOnly != standOnlyInvoke)
            {
                standOnlyInvoke = standOnly;
                playerVariables.Set("Standonly",
                    playerVariables.Get<int>("Standonly") + (standOnly ? 1 : -1));
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
                            playerVariables.Set("Health", .0f);
                        ManualAdjustString("血量上限", ref maxHealth, "MaxHealth", playerVariables);
                        ManualAdjustSlider("血量", ref health, ref healthUpdate, "MaxHealth", "Health", playerVariables);
                        ManualAdjustString("精神上限", ref maxStrength, "MaxStrength", playerVariables);
                        ManualAdjustSlider("精神", ref energy, ref energyUpdate, "MaxStrength", "Energy",
                            playerVariables);
                        ManualAdjustSlider("体力", ref strength, ref strengthUpdate, "Energy", "Strength",
                            playerVariables);
                        ManualAdjustFloatUpdating("移动速度", ref speed, ref speedUpdate, "Speed", playerVariables);
                        ManualAdjustFloatUpdating("防御", ref defence, ref defenceUpdate, "Defence", playerVariables);
                        break;
                    case 1:
                        AutoAdjustString("血量", ref bossHealth);
                        AutoAdjustString("移动速度", ref bossSpeed);
                        AutoAdjustString("防御", ref bossDefence);
                        AutoAdjustString("位置", ref bossLocate);
                        if (GUILayout.Button("重新生成Boss"))
                        {
                            GameObject boss;
                            if ((boss = GameObject.FindWithTag("Boss")) != null)
                                boss.GetComponent<Variables>().declarations.Set("Health", .0f);
                            GameObject.FindWithTag("BossAssistance").GetComponent<BossAssistance>().BossDead();
                            var spawner = bossSpawners.Where(s => s.name == $"Boss Spawner {bossLocate}")
                                .FirstOrDefault();
                            if (spawner != null)
                            {
                                var spawnerComponent = spawner.GetComponent<BossSpawner>();
                                spawnerComponent.bossHealth = float.Parse(bossHealth);
                                spawnerComponent.bossSpeed = float.Parse(bossSpeed);
                                spawnerComponent.bossDefence = float.Parse(bossDefence);
                                spawnerComponent.triggered = false;
                                spawner.GetComponent<BoxCollider2D>().enabled = true;
                            }
                        }

                        break;
                    case 2:
                        AutoAdjustString("沾染时间", ref st);
                        AutoAdjustString("动力伤害系数", ref mdc);
                        AutoAdjustString("热力伤害系数", ref tdc);
                        AutoAdjustString("最小热力伤害电势差", ref mdp);
                        GUILayout.BeginVertical("Box");
                        GUILayout.Label("炼金术物品栏");
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("玻封    标签", GUILayout.ExpandWidth(false));
                        drugTag = GUILayout.TextField(drugTag, GUILayout.ExpandWidth(true));
                        GUILayout.Label("酸碱性", GUILayout.ExpandWidth(false));
                        properties = GUILayout.TextField(properties, GUILayout.ExpandWidth(true));
                        GUILayout.Label("数量", GUILayout.ExpandWidth(false));
                        drugCount = GUILayout.TextField(drugCount, GUILayout.ExpandWidth(true));
                        if (GUILayout.Button("+", GUILayout.ExpandWidth(false)) && ions.Count != 0)
                        {
                            inventory.Add(new DrugStack(drugTag, ions, int.Parse(properties), int.Parse(drugCount)));
                            ions = new List<IonStack>();
                        }

                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("子弹    子弹外壳ID", GUILayout.ExpandWidth(false));
                        chemicalBullet = GUILayout.TextField(chemicalBullet, GUILayout.ExpandWidth(true));
                        if (GUILayout.Button("+", GUILayout.ExpandWidth(false)) && ions.Count != 0)
                        {
                            inventory.Add(new Bullet(FirearmDictionary.BulletTemples[int.Parse(chemicalBullet)],
                                int.Parse(drugCount),
                                new DrugStack(drugTag, ions, int.Parse(properties), int.Parse(drugCount))));
                            ions = new List<IonStack>();
                        }

                        GUILayout.EndHorizontal();
                        GUILayout.Label("成分");
                        ionsScroll = GUILayout.BeginScrollView(ionsScroll, false, false, GUILayout.Height(100));
                        for (var i = 0; i < ions.Count; i++)
                        {
                            GUILayout.BeginHorizontal("Box");
                            GUILayout.Label(ions[i].Element.symbol);
                            var v = ions[i].Element.valences[ions[i].Index];
                            GUILayout.Label($"{(v > 0 ? "+" : "")}{v}");
                            GUILayout.Label($"{ions[i].Amount} mol");
                            GUILayout.Label($"{ions[i].Concentration} mol/L");
                            if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
                            {
                                ions.RemoveAt(i);
                                i--;
                            }

                            GUILayout.EndHorizontal();
                        }

                        GUILayout.EndScrollView();
                        GUILayout.BeginHorizontal("Box");
                        GUILayout.Label("添加成分    元素", GUILayout.ExpandWidth(false));
                        element = GUILayout.TextField(element, GUILayout.ExpandWidth(true));
                        GUILayout.Label("价态", GUILayout.ExpandWidth(false));
                        valence = GUILayout.TextField(valence, GUILayout.ExpandWidth(true));
                        GUILayout.Label("物质的量", GUILayout.ExpandWidth(false));
                        amount = GUILayout.TextField(amount, GUILayout.ExpandWidth(true));
                        GUILayout.Label("浓度", GUILayout.ExpandWidth(false));
                        concentration = GUILayout.TextField(concentration, GUILayout.ExpandWidth(true));
                        if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                        {
                            var e = Elements.Dic.FirstOrDefault(item =>
                                string.Equals(element, item.symbol, StringComparison.CurrentCultureIgnoreCase));
                            var v = int.Parse(valence);
                            if (e != null && e.HasValence(v))
                                ions.Add(new IonStack
                                {
                                    Element = e, Index = e.GetIndex(v),
                                    Amount = float.Parse(amount), Concentration = float.Parse(concentration)
                                });
                        }

                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        break;
                    case 3:
                        GUILayout.BeginVertical();
                        GUILayout.BeginVertical("Box");
                        GUILayout.Label("枪械模板");
                        firearmScroll = GUILayout.BeginScrollView(firearmScroll, false, false, GUILayout.Height(100));
                        for (var i = 0; i < FirearmDictionary.FirearmTemples.Count; i++)
                        {
                            GUILayout.BeginHorizontal("Box");
                            GUILayout.Label(
                                $"{i} - {FirearmDictionary.FirearmTemples[i].Name} ({FirearmDictionary.GetFirearmType(FirearmDictionary.FirearmTemples[i].Type)})");
                            GUILayout.Label($"{FirearmDictionary.FirearmTemples[i].Damage}");
                            GUILayout.Label($"{FirearmDictionary.FirearmTemples[i].Depth}");
                            GUILayout.Label(
                                $"{FirearmDictionary.FirearmTemples[i].Loading}({FirearmDictionary.FirearmTemples[i].Reload})");
                            GUILayout.Label(
                                $"{FirearmDictionary.FirearmTemples[i].Deviation}({FirearmDictionary.FirearmTemples[i].MaxRange})");
                            GUILayout.Label($"{FirearmDictionary.FirearmTemples[i].Weight}");
                            if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
                            {
                                FirearmDictionary.FirearmTemples.RemoveAt(i);
                                i--;
                            }

                            GUILayout.EndHorizontal();
                        }

                        GUILayout.EndScrollView();
                        GUILayout.BeginVertical("Box");
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("名称", GUILayout.ExpandWidth(false));
                        firearmName = GUILayout.TextField(firearmName);
                        GUILayout.Label("类别", GUILayout.ExpandWidth(false));
                        firearmType = GUILayout.TextField(firearmType);
                        GUILayout.Label("使用弹匣ID", GUILayout.ExpandWidth(false));
                        magazineTemple = GUILayout.TextField(magazineTemple);
                        if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                            FirearmDictionary.FirearmTemples.Add(new FirearmTemple(firearmName,
                                FirearmDictionary.TypeIndexOf(firearmType), float.Parse(damage), float.Parse(reload),
                                float.Parse(loading), float.Parse(weight), float.Parse(depth), float.Parse(deviation),
                                float.Parse(maxRange), float.Parse(kick), float.Parse(distance),
                                float.Parse(reloadSpeed),
                                SplitTempleString(magazineTemple).Select(i => FirearmDictionary.MagazineTemples[i].Name)
                                    .ToArray()));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("伤害倍率", GUILayout.ExpandWidth(false));
                        damage = GUILayout.TextField(damage, GUILayout.ExpandWidth(true));
                        GUILayout.Label("穿深", GUILayout.ExpandWidth(false));
                        depth = GUILayout.TextField(depth, GUILayout.ExpandWidth(true));
                        GUILayout.Label("换弹匣时间", GUILayout.ExpandWidth(false));
                        reload = GUILayout.TextField(reload, GUILayout.ExpandWidth(true));
                        GUILayout.Label("自动换弹时间", GUILayout.ExpandWidth(false));
                        loading = GUILayout.TextField(loading, GUILayout.ExpandWidth(true));
                        GUILayout.Label("重量", GUILayout.ExpandWidth(false));
                        weight = GUILayout.TextField(weight, GUILayout.ExpandWidth(true));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("精度", GUILayout.ExpandWidth(false));
                        deviation = GUILayout.TextField(deviation, GUILayout.ExpandWidth(true));
                        GUILayout.Label("散步", GUILayout.ExpandWidth(false));
                        maxRange = GUILayout.TextField(maxRange, GUILayout.ExpandWidth(true));
                        GUILayout.Label("换弹匣降速", GUILayout.ExpandWidth(false));
                        reloadSpeed = GUILayout.TextField(reloadSpeed, GUILayout.ExpandWidth(true));
                        GUILayout.Label("后坐力", GUILayout.ExpandWidth(false));
                        kick = GUILayout.TextField(kick, GUILayout.ExpandWidth(true));
                        GUILayout.Label("射程", GUILayout.ExpandWidth(false));
                        distance = GUILayout.TextField(distance, GUILayout.ExpandWidth(true));
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUILayout.EndVertical();
                        GUILayout.BeginVertical("Box");
                        GUILayout.Label("弹匣模板");
                        magazineScroll = GUILayout.BeginScrollView(magazineScroll, false, false, GUILayout.Height(100));
                        for (var i = 0; i < FirearmDictionary.MagazineTemples.Count; i++)
                        {
                            GUILayout.BeginHorizontal("Box");
                            GUILayout.Label($"{i} - {FirearmDictionary.MagazineTemples[i].Name}");
                            GUILayout.Label(FirearmDictionary.MagazineTemples[i].BulletContains.ToString());
                            GUILayout.Label(
                                $"{FirearmDictionary.MagazineTemples[i].Radius}*{FirearmDictionary.MagazineTemples[i].Length}");
                            GUILayout.Label($"{FirearmDictionary.MagazineTemples[i].Weight}");
                            if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
                            {
                                FirearmDictionary.MagazineTemples.RemoveAt(i);
                                i--;
                            }

                            GUILayout.EndHorizontal();
                        }

                        GUILayout.EndScrollView();
                        GUILayout.BeginHorizontal("Box");
                        GUILayout.Label("名称", GUILayout.ExpandWidth(false));
                        magazineName = GUILayout.TextField(magazineName, GUILayout.ExpandWidth(true));
                        GUILayout.Label("载弹量", GUILayout.ExpandWidth(false));
                        bulletContains = GUILayout.TextField(bulletContains, GUILayout.ExpandWidth(true));
                        GUILayout.Label("重量", GUILayout.ExpandWidth(false));
                        magazineWeight = GUILayout.TextField(magazineWeight, GUILayout.ExpandWidth(true));
                        GUILayout.Label("口径", GUILayout.ExpandWidth(false));
                        magazineRadius = GUILayout.TextField(magazineRadius, GUILayout.ExpandWidth(true));
                        GUILayout.Label("长度", GUILayout.ExpandWidth(false));
                        magazineLength = GUILayout.TextField(magazineLength, GUILayout.ExpandWidth(true));
                        if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                            FirearmDictionary.MagazineTemples.Add(new MagazineTemple(magazineName,
                                int.Parse(bulletContains),
                                float.Parse(magazineWeight), float.Parse(magazineRadius), float.Parse(magazineLength)));
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUILayout.BeginVertical("Box");
                        GUILayout.Label("子弹模板");
                        bulletScroll = GUILayout.BeginScrollView(bulletScroll, false, false, GUILayout.Height(100));
                        for (var i = 0; i < FirearmDictionary.BulletTemples.Count; i++)
                        {
                            GUILayout.BeginHorizontal("Box");
                            GUILayout.Label($"{i} - {FirearmDictionary.BulletTemples[i].Name}");
                            GUILayout.Label(
                                $"{FirearmDictionary.BulletTemples[i].BreakDamage}(+{FirearmDictionary.BulletTemples[i].Explode})");
                            GUILayout.Label(
                                $"{FirearmDictionary.BulletTemples[i].Depth}({FirearmDictionary.BulletTemples[i].MinDefence})");
                            GUILayout.Label(
                                $"{FirearmDictionary.BulletTemples[i].Radius}*{FirearmDictionary.BulletTemples[i].Length}");
                            if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
                            {
                                FirearmDictionary.BulletTemples.RemoveAt(i);
                                i--;
                            }

                            GUILayout.EndHorizontal();
                        }

                        GUILayout.EndScrollView();
                        GUILayout.BeginVertical("Box");
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("名称", GUILayout.ExpandWidth(false));
                        bulletName = GUILayout.TextField(bulletName, GUILayout.ExpandWidth(true));
                        GUILayout.Label("基础伤害", GUILayout.ExpandWidth(false));
                        bulletBreak = GUILayout.TextField(bulletBreak, GUILayout.ExpandWidth(true));
                        GUILayout.Label("击穿伤害", GUILayout.ExpandWidth(false));
                        bulletExplode = GUILayout.TextField(bulletExplode, GUILayout.ExpandWidth(true));
                        GUILayout.Label("穿深", GUILayout.ExpandWidth(false));
                        bulletDepth = GUILayout.TextField(bulletDepth, GUILayout.ExpandWidth(true));
                        if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                            FirearmDictionary.BulletTemples.Add(new BulletTemple(bulletName, float.Parse(bulletBreak),
                                float.Parse(bulletExplode), float.Parse(bulletDepth), float.Parse(bulletMinDefence),
                                float.Parse(bulletRadius), float.Parse(bulletLength)));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("过穿防御", GUILayout.ExpandWidth(false));
                        bulletMinDefence = GUILayout.TextField(bulletMinDefence, GUILayout.ExpandWidth(true));
                        GUILayout.Label("口径", GUILayout.ExpandWidth(false));
                        bulletRadius = GUILayout.TextField(bulletRadius, GUILayout.ExpandWidth(true));
                        GUILayout.Label("长度", GUILayout.ExpandWidth(false));
                        bulletLength = GUILayout.TextField(bulletLength, GUILayout.ExpandWidth(true));
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUILayout.EndVertical();
                        GUILayout.BeginVertical("Box");
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("枪械物品栏    ID", GUILayout.ExpandWidth(false));
                        firearm = GUILayout.TextField(firearm, GUILayout.ExpandWidth(true));
                        if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                            inventory.Add(new Firearm(FirearmDictionary.FirearmTemples[int.Parse(firearm)]));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("弹匣物品栏    ID", GUILayout.ExpandWidth(false));
                        magazine = GUILayout.TextField(magazine, GUILayout.ExpandWidth(true));
                        if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                        {
                            var index = int.Parse(magazine);
                            inventory.Add(new Magazine(FirearmDictionary.MagazineTemples[index]));
                        }

                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("子弹物品栏    ID", GUILayout.ExpandWidth(false));
                        bullet = GUILayout.TextField(bullet);
                        GUILayout.Label("数量", GUILayout.ExpandWidth(false));
                        bulletCount = GUILayout.TextField(bulletCount);
                        if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                        {
                            var index = int.Parse(bullet);
                            var count = int.Parse(bulletCount);
                            inventory.Add(new Bullet(FirearmDictionary.BulletTemples[index], count));
                        }

                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUILayout.BeginVertical("Box");
                        if (GUILayout.Button("保存模板数据")) SaveFirearmsModel();
                        AutoAdjustString("后坐力精度影响指数", ref kpfd);
                        playerFlash.sight = GUILayout.Toggle(playerFlash.sight, "使用瞄具");
                        GUILayout.EndVertical();
                        GUILayout.EndVertical();
                        break;
                    case 4:
                        inventoryScroll =
                            GUILayout.BeginScrollView(inventoryScroll, false, false, GUILayout.Height(600));
                        for (var i = 0; i < inventory.Contains.Count; i++)
                        {
                            GUILayout.BeginHorizontal();

                            GUILayout.Label($"{i} - {inventory.Contains[i].GetName()}",
                                GUILayout.ExpandWidth(true));
                            if (inventory.Contains[i] is Weapon &&
                                GUILayout.Button("·", GUILayout.ExpandWidth(false))) inventory.SwitchTo(i);

                            if (i != 0 && GUILayout.Button("↑", GUILayout.ExpandWidth(false)))
                            {
                                var front = inventory.Contains[i - 1];
                                var behind = inventory.Contains[i];
                                if (inventory.selecting == i) inventory.selecting--;
                                else if (inventory.selecting == i - 1) inventory.selecting++;
                                /*    behind.LoseSelected(inventory.gameObject);
                                else if (inventory.selecting == i - 1) front.LoseSelected(inventory.gameObject);
                                inventory.RebuildSubInventory();
                                if (inventory.selecting == i) front.OnSelected(inventory.gameObject);
                                else if (inventory.selecting == i - 1) behind.OnSelected(inventory.gameObject);
                                */
                                inventory.Contains[i - 1] = behind;
                                inventory.Contains[i] = front;
                            }

                            if (i != inventory.Contains.Count - 1 &&
                                GUILayout.Button("↓", GUILayout.ExpandWidth(false)))
                            {
                                var front = inventory.Contains[i];
                                var behind = inventory.Contains[i + 1];
                                if (inventory.selecting == i + 1) inventory.selecting--;
                                else if (inventory.selecting == i) inventory.selecting++;
                                /*    behind.LoseSelected(inventory.gameObject);
                                else if (inventory.selecting == i) front.LoseSelected(inventory.gameObject);
                                inventory.RebuildSubInventory();
                                if (inventory.selecting == i + 1) front.OnSelected(inventory.gameObject);
                                else if (inventory.selecting == i) behind.OnSelected(inventory.gameObject);
                                */
                                inventory.Contains[i] = behind;
                                inventory.Contains[i + 1] = front;
                            }

                            if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
                            {
                                inventory.Remove(inventory.Contains[i]);
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

        private static void ManualAdjustString(string text, ref string value, string targetName,
            VariableDeclarations target)
        {
            var v = target.Get<float>(targetName);
            ManualAdjustString(text, ref value, ref v);
            target.Set(targetName, v);
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
            VariableDeclarations target)
        {
            var v = target.Get<int>(targetName);
            ManualAdjustIntUpdating(text, ref value, ref update, ref v);
            target.Set(targetName, v);
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
            VariableDeclarations target)
        {
            var v = target.Get<float>(targetName);
            ManualAdjustFloatUpdating(text, ref value, ref update, ref v);
            target.Set(targetName, v);
        }

        private static void ManualAdjustFloatUpdating(string text, ref string value, ref bool update, ref float target)
        {
            GUILayout.BeginHorizontal("Box");
            GUILayout.Label(text, GUILayout.ExpandWidth(false));
            value = GUILayout.TextField(value, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("生效", GUILayout.ExpandWidth(false))) target = float.Parse(value);
            update = GUILayout.Toggle(update, "实时更新", GUILayout.ExpandWidth(false));
            if (update) value = $"{target}";
            GUILayout.EndHorizontal();
        }

        private static void ManualAdjustSlider(string text, ref float value, ref bool update, string maxName,
            string targetName, VariableDeclarations target)
        {
            var max = target.Get<float>(maxName);
            var v = target.Get<float>(targetName);
            ManualAdjustSlider(text, ref value, ref update, max, ref v);
            target.Set(targetName, v);
        }

        private static void ManualAdjustSlider(string text, ref float value, ref bool update, float max,
            ref float target)
        {
            GUILayout.BeginHorizontal("Box");
            GUILayout.Label(text, GUILayout.ExpandWidth(false));
            value = GUILayout.HorizontalSlider(value, .0f, max, GUILayout.MinWidth(260));
            GUILayout.Label($"{value:F2}");
            if (GUILayout.Button("生效", GUILayout.ExpandWidth(false))) target = value;
            update = GUILayout.Toggle(update, "实时更新", GUILayout.ExpandWidth(false));
            if (update) value = target;
            GUILayout.EndHorizontal();
        }

        private static IEnumerable<int> SplitTempleString(string temple)
        {
            return temple.Split(',').Select(int.Parse).ToArray();
        }

        private static void SaveFirearmsModel()
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
                firearmTemples = FirearmDictionary.FirearmTemples,
                magazineTemples = FirearmDictionary.MagazineTemples,
                bulletTemples = FirearmDictionary.BulletTemples
            }));
            var level = 0;
            var check = true;
            for (var i = 0; i < json.Length; i++)
            {
                if (json[i] == '"') check = !check;
                if (check)
                    switch (json[i])
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