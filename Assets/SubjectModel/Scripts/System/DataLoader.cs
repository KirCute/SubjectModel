using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using SubjectModel.Scripts.Development;
using SubjectModel.Scripts.Subject.Chemistry;
using SubjectModel.Scripts.Subject.Firearms;
using UnityEngine;

namespace SubjectModel.Scripts.System
{
    public class DataLoader : MonoBehaviour
    {
        private void Awake()
        {
            //SaveElementsModel();
            LoadElementsModel();
            LoadFirearmsModel();
        }

        private void Start()
        {
            Test.GenerateContainerData();
        }

        private static void LoadFirearmsModel()
        {
            var origin = new StringBuilder(File.ReadAllText(Path.Combine(Application.dataPath, "Firearms.json")));
            origin.Replace("\n", "");
            origin.Replace("\t", "");
            var data = JsonConvert.DeserializeObject<GunData>(origin.ToString());
            FirearmDictionary.FirearmTemples = data == null ? new List<FirearmTemple>() : data.firearmTemples;
            FirearmDictionary.MagazineTemples = data == null ? new List<MagazineTemple>() : data.magazineTemples;
            FirearmDictionary.BulletTemples = data == null ? new List<BulletTemple>() : data.bulletTemples;
        }

        private static void LoadElementsModel()
        {
            var origin = new StringBuilder(File.ReadAllText(Path.Combine(Application.dataPath, "Elements.json")));
            origin.Replace("\n", "");
            origin.Replace("\t", "");
            Elements.Dic = JsonConvert.DeserializeObject<List<Element>>(origin.ToString());
        }

        private static void SaveElementsModel()
        {
            var path = Path.Combine(Application.dataPath, "Elements.json");
            var backup = Path.Combine(Application.dataPath, "Elements.bak");
            if (File.Exists(path))
            {
                if (File.Exists(backup)) File.Delete(backup);
                File.Move(path, backup);
            }

            var json = new StringBuilder(JsonConvert.SerializeObject(Elements.Dic));
            var level = 0;
            var check = true;
            for (var i = 0; i < json.Length; i++)
            {
                if (json[i] == '"') check = !check;
                if (check)
                    switch (json[i])
                    {
                        case '[':
                        case '{':
                            level++;
                            for (var j = 0; j < level; j++) json.Insert(i + 1, "\t");
                            json.Insert(i + 1, "\n");
                            break;
                        case ',':
                            for (var j = 0; j < level; j++) json.Insert(i + 1, "\t");
                            json.Insert(i + 1, "\n");
                            break;
                        case ']':
                        case '}':
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