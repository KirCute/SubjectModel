using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SubjectModel
{
    
    public static class GunDictionary
    {
        private static List<Gun> defaultInventory;
        
        public static List<Gun> GetDefaultInventory()
        {
            if (defaultInventory != null) return defaultInventory;
            defaultInventory = JsonUtility.FromJson<GunData>(Resources.Load<TextAsset>("Firearms").text).guns;
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
    }

    [Serializable]
    public class GunData
    {
        public List<Gun> guns;
    }
    
    [Serializable]
    public class Gun
    {
        public string name;
        public float damage;
        public float loadingTime;
        public int clipCapacity;
        public float clipSwitchTime;
    }
}