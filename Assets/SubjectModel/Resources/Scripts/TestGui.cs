using System;
using System.Linq;
using Bolt;
using UnityEditor;
using UnityEngine;

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

        private string magazineName;
        private string bulletContains;
        private string magazineWeight;

        private Inventory inventory;
        private GunFlash playerFlash;
        private string firearm;
        private string magazine;

        private bool standOnly;
        private bool standOnlyInvoke;
        private bool pause;

        private void Start()
        {
            selected = 4;
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
            mdc = BuffRenderer.MotiveDamageCoefficient.ToString();
            tdc = BuffRenderer.ThermalDamageCoefficient.ToString();
            mdp = BuffRenderer.MinimumDamagePotential.ToString();
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
            GUILayout.Window(0, new Rect(60, 80, 400, 20), id =>
            {
                selected = GUILayout.Toolbar(selected, new[] {"玩家", "敌人", "炼金术", "枪械", "收起", "系统"});
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
                        ManualAdjustFloatUpdating("生效速度百分比", ref firearmSpeed, ref firearmSpeedUpdate, "FirearmSpeed", playerVariables);
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
                        for (var i = 0; i < Test.FirearmTemples.Count; i++)
                        {
                            GUILayout.BeginHorizontal("Box");
                            GUILayout.Label($"{i} - {Test.FirearmTemples[i].Name}");
                            if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
                            {
                                Test.FirearmTemples.RemoveAt(i);
                                i--;
                            }

                            GUILayout.EndHorizontal();
                        }

                        GUILayout.BeginVertical("Box");
                        GUILayout.BeginHorizontal();
                        firearmName = GUILayout.TextField(firearmName);
                        magazineTemple = GUILayout.TextField(magazineTemple);
                        if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                            Test.FirearmTemples.Add(new FirearmTemple(firearmName, float.Parse(damage), float.Parse(reload), 
                                float.Parse(loading), float.Parse(weight), float.Parse(depth), float.Parse(deviation),
                                float.Parse(maxRange), float.Parse(kick), float.Parse(distance), float.Parse(reloadSpeed), 
                                SplitTempleString(magazineTemple).Select(i => Test.MagazineTemples[i]).ToArray()));
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        damage = GUILayout.TextField(damage);
                        reload = GUILayout.TextField(reload);
                        loading = GUILayout.TextField(loading);
                        weight = GUILayout.TextField(weight);
                        depth = GUILayout.TextField(depth);
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        deviation = GUILayout.TextField(deviation);
                        maxRange = GUILayout.TextField(maxRange);
                        kick = GUILayout.TextField(kick);
                        distance = GUILayout.TextField(distance);
                        reloadSpeed = GUILayout.TextField(reloadSpeed);
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical("Box");
                        GUILayout.Label("弹匣模板");
                        for (var i = 0; i < Test.MagazineTemples.Count; i++)
                        {
                            GUILayout.BeginHorizontal("Box");
                            GUILayout.Label($"{i} - {Test.MagazineTemples[i].Name}");
                            GUILayout.Label(Test.MagazineTemples[i].BulletContains.ToString());
                            if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
                            {
                                Test.MagazineTemples.RemoveAt(i);
                                i--;
                            }

                            GUILayout.EndHorizontal();
                        }

                        GUILayout.BeginHorizontal("Box");
                        magazineName = GUILayout.TextField(magazineName);
                        bulletContains = GUILayout.TextField(bulletContains);
                        magazineWeight = GUILayout.TextField(magazineWeight);
                        if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                            Test.MagazineTemples.Add(new MagazineTemple(magazineName, int.Parse(bulletContains), float.Parse(magazineWeight)));
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();

                        GUILayout.BeginHorizontal("Box");
                        GUILayout.Label("枪械物品栏");
                        firearm = GUILayout.TextField(firearm, GUILayout.ExpandWidth(true));
                        if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                            inventory.Add(new Firearm(Test.FirearmTemples[int.Parse(firearm)]));
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal("Box");
                        GUILayout.Label("弹匣物品栏");
                        magazine = GUILayout.TextField(magazine, GUILayout.ExpandWidth(true));
                        if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                        {
                            var index = int.Parse(magazine);
                            inventory.Add(new Magazine(Test.MagazineTemples[index]) {
                                    BulletRemain = Test.MagazineTemples[index].BulletContains
                            });
                        }
                        GUILayout.EndHorizontal();
                        
                        GUILayout.BeginVertical("Box");
                        playerFlash.sight = GUILayout.Toggle(playerFlash.sight, "使用瞄具");
                        GUILayout.EndVertical();
                        
                        GUILayout.EndVertical();
                        break;
                    case 5:
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

        private static int[] SplitTempleString(string temple)
        {
            return temple.Split(',').Select(int.Parse).ToArray();
        }
    }
}