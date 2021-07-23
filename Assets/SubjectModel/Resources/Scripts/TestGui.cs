using System;
using System.Collections.Generic;
using System.Linq;
using Bolt;
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
        private BossSpawner bossSpawner;
        private string bossHealth;
        private string bossSpeed;
        private string bossDefence;
        private GunShoot playerGun;
        private string bulletRemain;
        private bool bulletRemainUpdate;
        private string damage;
        private string depth;
        private string deviation;
        private string maxRange;
        private string loadingTime;
        private string loading;
        private bool loadingUpdate;
        private string distance;
        private string mdc;
        private string tdc;
        private string mdp;
        private IList<DrugStack> inventory;
        private IList<IList<IList<string>>> chemistryMenu;
        private IList<string> drugProperties;
        private Vector2 chemistryScrollPos;

        private void Start()
        {
            selected = 4;
            var player = GameObject.FindWithTag("Player");
            playerVariables = player.GetComponent<Variables>();
            maxHealth = playerVariables.declarations.Get("MaxHealth").ToString();
            health = (float) playerVariables.declarations.Get("Health");
            healthUpdate = false;
            maxStrength = playerVariables.declarations.Get("MaxStrength").ToString();
            energy = (float) playerVariables.declarations.Get("Energy");
            energyUpdate = false;
            strength = (float) playerVariables.declarations.Get("Strength");
            strengthUpdate = false;
            speed = playerVariables.declarations.Get("Speed").ToString();
            speedUpdate = false;
            defence = playerVariables.declarations.Get("Defence").ToString();
            defenceUpdate = false;
            bossSpawner = GameObject.FindWithTag("BossSpawner").GetComponent<BossSpawner>();
            bossHealth = bossSpawner.bossHealth.ToString();
            bossSpeed = bossSpawner.bossSpeed.ToString();
            bossDefence = bossSpawner.bossDefence.ToString();
            playerGun = player.GetComponent<GunShoot>();
            bulletRemain = playerGun.bulletRemain.ToString();
            bulletRemainUpdate = false;
            damage = playerGun.damage.ToString();
            depth = playerGun.depth.ToString();
            deviation = playerGun.deviation.ToString();
            maxRange = playerGun.maxRange.ToString();
            loadingTime = playerGun.loadingTime.ToString();
            loading = playerVariables.declarations.Get("Loading").ToString();
            loadingUpdate = false;
            distance = playerGun.distance.ToString();
            inventory = DrugDictionary.GetDefaultInventory();
            drugProperties = new List<string>();
            mdc = BuffRenderer.MotiveDamageCoefficient.ToString();
            tdc = BuffRenderer.ThermalDamageCoefficient.ToString();
            mdp = BuffRenderer.MinimumDamagePotential.ToString();
            chemistryMenu = new List<IList<IList<string>>>();
            foreach (var drug in inventory)
            {
                drugProperties.Add(drug.Properties.ToString());
                var drugList = drug.Ions.Select(ion => new List<string>
                {
                    ion.GetSymbol(drug.Properties), ion.Index.ToString(),
                    ion.Amount.ToString(), ion.Concentration.ToString()
                }).Cast<IList<string>>().ToList();
                chemistryMenu.Add(drugList);
            }
            chemistryScrollPos = Vector2.zero;
        }

        private void Update()
        {
            if (float.TryParse(bossHealth, out var value)) bossSpawner.bossHealth = value;
            if (float.TryParse(bossSpeed, out value)) bossSpawner.bossSpeed = value;
            if (float.TryParse(bossDefence, out value)) bossSpawner.bossDefence = value;
            if (float.TryParse(damage, out value)) playerGun.damage = value;
            if (float.TryParse(depth, out value)) playerGun.depth = value;
            if (float.TryParse(deviation, out value)) playerGun.deviation = value;
            if (float.TryParse(maxRange, out value)) playerGun.maxRange = value;
            if (float.TryParse(loadingTime, out value)) playerGun.loadingTime = value;
            if (float.TryParse(distance, out value)) playerGun.distance = value;
            if (float.TryParse(mdc, out value)) BuffRenderer.MotiveDamageCoefficient = value;
            if (float.TryParse(tdc, out value)) BuffRenderer.ThermalDamageCoefficient = value;
            if (float.TryParse(mdp, out value)) BuffRenderer.MinimumDamagePotential = value;
            for (var i = 0; i < inventory.Count; i++)
            {
                if (int.TryParse(drugProperties[i], out var intValue)) inventory[i].Properties = intValue;
                for (var j = 0; j < chemistryMenu[i].Count; j++)
                {
                    chemistryMenu[i][j][0] = inventory[i].Ions[j].GetSymbol(inventory[i].Properties);
                    if (int.TryParse(chemistryMenu[i][j][1], out intValue)) inventory[i].Ions[j].Index = intValue;
                    if (float.TryParse(chemistryMenu[i][j][2], out value)) inventory[i].Ions[j].Amount = value;
                    if (float.TryParse(chemistryMenu[i][j][3], out value)) inventory[i].Ions[j].Concentration = value;
                }
            }
        }

        private void OnGUI()
        {
            GUILayout.Window(0, new Rect(30, 40, 300, 20), id =>
            {
                selected = GUILayout.Toolbar(selected, new[] {"玩家", "敌人", "枪械", "炼金术", "收起"});
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
                        ManualAdjustIntUpdating("剩余弹药数", ref bulletRemain, ref bulletRemainUpdate,
                            ref playerGun.bulletRemain);
                        AutoAdjustString("伤害", ref damage);
                        AutoAdjustString("穿深", ref depth);
                        AutoAdjustString("精度", ref deviation);
                        AutoAdjustString("散布", ref maxRange);
                        AutoAdjustString("总装填时间(s)", ref loadingTime);
                        ManualAdjustFloatUpdating("剩余装填时间(s)", ref loading, ref loadingUpdate, "Loading",
                            playerVariables);
                        AutoAdjustString("射程(m)", ref distance);
                        GUILayout.BeginHorizontal("Box");
                        playerGun.telescope = GUILayout.Toggle(playerGun.telescope, "使用瞄具");
                        GUILayout.EndHorizontal();
                        break;
                    case 3:
                        GUILayout.BeginVertical("Box");
                        AutoAdjustString("动力伤害系数", ref mdc);
                        AutoAdjustString("热力伤害系数", ref tdc);
                        AutoAdjustString("最小热力伤害电势差", ref mdp);
                        GUILayout.EndVertical();
                        chemistryScrollPos = GUILayout.BeginScrollView(chemistryScrollPos,
                            true, false, GUILayout.Height(200));
                        for (var i = 0; i < chemistryMenu.Count; i++)
                        {
                            var drugProperty = drugProperties[i];
                            DrugStackAdjuster(inventory[i].Tag, ref drugProperty, chemistryMenu[i]);
                            drugProperties[i] = drugProperty;
                        }
                        GUILayout.EndScrollView();

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
            var v = (float) target.declarations.Get(targetName);
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
            var v = (int) target.declarations.Get(targetName);
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
            var v = (float) target.declarations.Get(targetName);
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
            var max = (float) target.declarations.Get(maxName);
            var v = (float) target.declarations.Get(targetName);
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

        private static void DrugStackAdjuster(string tag, ref string properties, IList<IList<string>> stack)
        {
            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            GUILayout.Label(tag);
            properties = GUILayout.TextField(properties, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();
            foreach (var t in stack) IonStackAdjuster(t);
            GUILayout.EndVertical();
        }

        private static void IonStackAdjuster(IList<string> stack)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(stack[0]);
            for (var i = 1; i < stack.Count; i++)
                stack[i] = GUILayout.TextField(stack[i], GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();
        }
    }
}