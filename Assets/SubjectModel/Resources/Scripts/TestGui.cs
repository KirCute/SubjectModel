using System;
using System.Collections.Generic;
using System.Linq;
using Bolt;
using UnityEngine;

namespace SubjectModel
{
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
        private IList<DrugStack> inventory;
        private IList<string[]> chemistryMenu;

        private void Start()
        {
            selected = 4;
            var player = GameObject.FindWithTag("Player");
            playerVariables = player.GetComponent<Variables>();
            maxHealth = playerVariables.declarations.GetDeclaration("MaxHealth").value.ToString();
            health = (float) playerVariables.declarations.GetDeclaration("Health").value;
            healthUpdate = false;
            maxStrength = playerVariables.declarations.GetDeclaration("MaxStrength").value.ToString();
            energy = (float) playerVariables.declarations.GetDeclaration("Energy").value;
            energyUpdate = false;
            strength = (float) playerVariables.declarations.GetDeclaration("Strength").value;
            strengthUpdate = false;
            speed = playerVariables.declarations.GetDeclaration("Speed").value.ToString();
            speedUpdate = false;
            defence = playerVariables.declarations.GetDeclaration("Defence").value.ToString();
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
            loading = playerVariables.declarations.GetDeclaration("Loading").value.ToString();
            loadingUpdate = false;
            distance = playerGun.distance.ToString();
            inventory = DrugDictionary.GetDefaultInventory();
            chemistryMenu = new List<string[]>();
            foreach (var stack in inventory)
                chemistryMenu.Add((from param in stack.Param select param.ToString()).ToArray());
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
            for (var i = 0; i < inventory.Count; i++)
            for (var j = 0; j < inventory[i].Param.Length; j++)
                if (float.TryParse(chemistryMenu[i][j], out value))
                    inventory[i].Param[j] = value;
        }

        private void OnGUI()
        {
            GUILayout.Window(0, new Rect(20, 20, 400, 50), id =>
            {
                selected = GUILayout.Toolbar(selected, new[] {"玩家", "敌人", "枪械", "炼金术", "收起"});
                switch (selected)
                {
                    case 0:
                        if (GUILayout.Button("回到重生点"))
                            playerVariables.declarations.GetDeclaration("Health").value = .0f;
                        ManualAdjustString("血量上限", ref maxHealth, "MaxHealth", playerVariables);
                        ManualAdjustSlider("血量", ref health, ref healthUpdate, "MaxHealth", "Health", playerVariables);
                        ManualAdjustString("精神上限", ref maxStrength, "MaxStrength", playerVariables);
                        ManualAdjustSlider("能量", ref energy, ref energyUpdate, "MaxStrength", "Energy",
                            playerVariables);
                        ManualAdjustSlider("精神", ref strength, ref strengthUpdate, "Energy", "Strength",
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
                                boss.GetComponent<Variables>().declarations.GetDeclaration("Health").value = .0f;
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
                        break;
                    case 3:
                        for (var i = 0; i < inventory.Count; i++)
                        {
                            var strings = chemistryMenu[i];
                            DrugStackAdjuster(inventory[i].Type, ref strings);
                            chemistryMenu[i] = strings;
                        }

                        break;
                }
            }, "Test");
        }

        private static void AutoAdjustString(string text, ref string value)
        {
            GUILayout.BeginHorizontal("Box");
            GUILayout.Label(text);
            value = GUILayout.TextField(value);
            GUILayout.EndHorizontal();
        }

        private static void ManualAdjustString(string text, ref string value, string targetName, Variables target)
        {
            var v = (float) target.declarations.GetDeclaration(targetName).value;
            ManualAdjustString(text, ref value, ref v);
            target.declarations.GetDeclaration(targetName).value = v;
        }

        private static void ManualAdjustString(string text, ref string value, ref float target)
        {
            GUILayout.BeginHorizontal("Box");
            GUILayout.Label(text);
            value = GUILayout.TextField(value);
            if (GUILayout.Button("生效")) target = float.Parse(value);
            GUILayout.EndHorizontal();
        }

        private static void ManualAdjustIntUpdating(string text, ref string value, ref bool update, string targetName,
            Variables target)
        {
            var v = (int) target.declarations.GetDeclaration(targetName).value;
            ManualAdjustIntUpdating(text, ref value, ref update, ref v);
            target.declarations.GetDeclaration(targetName).value = v;
        }

        private static void ManualAdjustIntUpdating(string text, ref string value, ref bool update, ref int target)
        {
            GUILayout.BeginHorizontal("Box");
            GUILayout.Label(text);
            value = GUILayout.TextField(value);
            if (GUILayout.Button("生效")) target = int.Parse(value);
            update = GUILayout.Toggle(update, "实时更新");
            if (update) value = target.ToString();
            GUILayout.EndHorizontal();
        }

        private static void ManualAdjustFloatUpdating(string text, ref string value, ref bool update, string targetName,
            Variables target)
        {
            var v = (float) target.declarations.GetDeclaration(targetName).value;
            ManualAdjustFloatUpdating(text, ref value, ref update, ref v);
            target.declarations.GetDeclaration(targetName).value = v;
        }

        private static void ManualAdjustFloatUpdating(string text, ref string value, ref bool update, ref float target)
        {
            GUILayout.BeginHorizontal("Box");
            GUILayout.Label(text);
            value = GUILayout.TextField(value);
            if (GUILayout.Button("生效")) target = float.Parse(value);
            update = GUILayout.Toggle(update, "实时更新");
            if (update) value = target.ToString();
            GUILayout.EndHorizontal();
        }

        private static void ManualAdjustSlider(string text, ref float value, ref bool update, string maxName,
            string targetName, Variables target)
        {
            var max = (float) target.declarations.GetDeclaration(maxName).value;
            var v = (float) target.declarations.GetDeclaration(targetName).value;
            ManualAdjustSlider(text, ref value, ref update, max, ref v);
            target.declarations.GetDeclaration(targetName).value = v;
        }

        private static void ManualAdjustSlider(string text, ref float value, ref bool update, float max,
            ref float target)
        {
            GUILayout.BeginHorizontal("Box");
            GUILayout.Label(text);
            value = GUILayout.HorizontalSlider(value, .0f, max);
            GUILayout.Label(Math.Round(value).ToString());
            if (GUILayout.Button("生效")) target = value;
            update = GUILayout.Toggle(update, "实时更新");
            if (update) value = target;
            GUILayout.EndHorizontal();
        }

        private static void DrugStackAdjuster(Buff type, ref string[] stack)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(DrugDictionary.GetName(type));
            for (var i = 0; i < stack.Length; i++) stack[i] = GUILayout.TextField(stack[i]);
            GUILayout.EndHorizontal();
        }
    }
}