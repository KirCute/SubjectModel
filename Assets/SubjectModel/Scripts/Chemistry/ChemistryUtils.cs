using System;
using System.Collections.Generic;
using UnityEngine;

namespace SubjectModel.Scripts.Chemistry
{
    public static class DrugDictionary
    {
        private static readonly Dictionary<Buff, Type> BuffDictionary = new Dictionary<Buff, Type>();
        private static readonly Dictionary<Buff, string> BuffName = new Dictionary<Buff, string>();
        private static readonly Dictionary<Buff, Vector3> BuffColor = new Dictionary<Buff, Vector3>();

        public static Type GetTypeOfBuff(Buff buff)
        {
            if (BuffDictionary.Count != 0) return BuffDictionary[buff];
            BuffDictionary.Add(Buff.Slowness, typeof(DrugEffect.IIIFe));
            BuffDictionary.Add(Buff.Poison, typeof(DrugEffect.IICu));
            BuffDictionary.Add(Buff.Curing, typeof(DrugEffect.IICo));
            BuffDictionary.Add(Buff.Ghost, typeof(DrugEffect.PIII));
            BuffDictionary.Add(Buff.Corrosion, typeof(DrugEffect.H));
            BuffDictionary.Add(Buff.Rapid, typeof(DrugEffect.IIFe));
            return BuffDictionary[buff];
        }

        public static string GetName(Buff buff)
        {
            if (BuffName.Count != 0) return BuffName[buff];
            BuffName.Add(Buff.Empty, "空效果");
            BuffName.Add(Buff.Slowness, "缓慢");
            BuffName.Add(Buff.Poison, "中毒");
            BuffName.Add(Buff.Curing, "治疗");
            BuffName.Add(Buff.Ghost, "P(III)");
            BuffName.Add(Buff.Corrosion, "腐蚀");
            BuffName.Add(Buff.Rapid, "急速");
            return BuffName[buff];
        }

        public static string GetName(IonStack stack)
        {
            return GetName(stack.Element.buffType[stack.Index]);
        }

        public static Vector3 GetColor(Buff buff)
        {
            if (BuffColor.Count != 0) return BuffColor[buff];
            BuffColor.Add(Buff.Empty, new Vector3(1f, 1f, 1f));
            BuffColor.Add(Buff.Slowness, new Vector3(0.5f, 0.5f, 0.5f));
            BuffColor.Add(Buff.Poison, new Vector3(0.0f, 0.5f, 0.0f));
            BuffColor.Add(Buff.Curing, new Vector3(1.0f, 0.125f, 0.375f));
            BuffColor.Add(Buff.Ghost, new Vector3(0.25f, 0.75f, 0.375f));
            BuffColor.Add(Buff.Corrosion, new Vector3(1.0f, 0.875f, 0.0f));
            BuffColor.Add(Buff.Rapid, new Vector3(0.75f, 0.75f, 0.75f));
            return BuffColor[buff];
        }

        public static Vector3 GetColor(IonStack stack)
        {
            return GetColor(stack.Element.buffType[stack.Index]);
        }
    }
}