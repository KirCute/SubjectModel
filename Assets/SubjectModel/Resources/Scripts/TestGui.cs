using System;
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
        }

        private void Update()
        {
            playerGun.damage = float.Parse(damage);
            playerGun.depth = float.Parse(depth);
            playerGun.deviation = float.Parse(deviation);
            playerGun.maxRange = float.Parse(maxRange);
            playerGun.loadingTime = float.Parse(loadingTime);
            playerGun.distance = float.Parse(distance);
        }

        private void OnGUI()
        {
            GUILayout.Window(0, new Rect(20, 20, 400, 50), id =>
            {
                selected = GUILayout.Toolbar(selected, new[] {"玩家", "敌人", "枪械", "炼金术", "收起"});
                switch (selected)
                {
                    case 0:
                        ManualAdjustString("血量上限", ref maxHealth,"MaxHealth" , playerVariables);
                        ManualAdjustSlider("血量", ref health, ref healthUpdate, "MaxHealth", "Health", playerVariables);
                        ManualAdjustString("精神上限", ref maxStrength, "MaxStrength", playerVariables);
                        ManualAdjustSlider("能量", ref energy, ref energyUpdate, "MaxStrength", "Energy", playerVariables);
                        ManualAdjustSlider("精神", ref strength, ref strengthUpdate, "Energy", "Strength", playerVariables);
                        ManualAdjustFloatUpdating("速度", ref speed, ref speedUpdate, "Speed", playerVariables);
                        ManualAdjustFloatUpdating("防御", ref defence, ref defenceUpdate, "Defence", playerVariables);
                        break;
                    case 1:
                        break;
                    case 2:
                        ManualAdjustIntUpdating("剩余弹药数", ref bulletRemain, ref bulletRemainUpdate, ref playerGun.bulletRemain);
                        AutoAdjustString("伤害", ref damage);
                        AutoAdjustString("穿深", ref depth);
                        AutoAdjustString("精度", ref deviation);
                        AutoAdjustString("散布", ref maxRange);
                        AutoAdjustString("总装填时间(s)", ref loadingTime);
                        ManualAdjustFloatUpdating("剩余装填时间(s)", ref loading, ref loadingUpdate, "Loading", playerVariables);
                        AutoAdjustString("射程(m)", ref distance);
                        break;
                    case 3:
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
        private static void ManualAdjustIntUpdating(string text, ref string value, ref bool update, string targetName, Variables target)
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
        private static void ManualAdjustFloatUpdating(string text, ref string value, ref bool update, string targetName, Variables target)
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
        private static void ManualAdjustSlider(string text, ref float value, ref bool update, string maxName, string targetName, Variables target)
        {
            var max = (float) target.declarations.GetDeclaration(maxName).value;
            var v = (float) target.declarations.GetDeclaration(targetName).value;
            ManualAdjustSlider(text, ref value, ref update, max, ref v);
            target.declarations.GetDeclaration(targetName).value = v;
        }
        private static void ManualAdjustSlider(string text, ref float value, ref bool update, float max, ref float target)
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
    }
}