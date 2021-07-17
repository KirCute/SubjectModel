using System;
using System.Collections.Generic;
using UnityEngine;

namespace SubjectModel
{
    public struct DrugStack
    {
        public Buff Type;
        public object[] Param;
    }
    public enum Buff
    {
        Slowness,
        Poison,
        Curing,
        Ghost,
        Corrosion
    }

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
            return BuffDictionary[buff];
        }

        public static string GetName(Buff buff)
        {
            if (BuffName.Count != 0) return BuffName[buff];
            BuffName.Add(Buff.Slowness, "缓慢");
            BuffName.Add(Buff.Poison, "中毒");
            BuffName.Add(Buff.Curing, "治疗");
            BuffName.Add(Buff.Ghost, "P(III)");
            BuffName.Add(Buff.Corrosion, "腐蚀");
            return BuffName[buff];
        }

        public static string GetName(DrugStack stack)
        {
            return GetName(stack.Type);
        }

        public static Vector3 GetColor(Buff buff)
        {
            if (BuffColor.Count != 0) return BuffColor[buff];
            BuffColor.Add(Buff.Slowness, new Vector3(0.5f, 0.5f, 0.5f));
            BuffColor.Add(Buff.Poison, new Vector3(0.0f, 0.5f, 0.0f));
            BuffColor.Add(Buff.Curing, new Vector3(1.0f, 0.125f, 0.375f));
            BuffColor.Add(Buff.Ghost, new Vector3(0.25f, 0.75f, 0.375f));
            BuffColor.Add(Buff.Corrosion, new Vector3(1.0f, 0.875f, 0.0f));
            return BuffColor[buff];
        }

        public static Vector3 GetColor(DrugStack stack)
        {
            return GetColor(stack.Type);
        }
        
        public static IList<DrugStack> GenerateInventory()
        {
            return new List<DrugStack>
            {
                DrugStackFactory(Buff.Slowness, new object[] {3.0f, 2.0f}),
                DrugStackFactory(Buff.Poison, new object[] {2.0f, 12.5f}),
                DrugStackFactory(Buff.Curing, new object[] {2.0f, 25f}),
                DrugStackFactory(Buff.Ghost, new object[] {6.0f}),
                DrugStackFactory(Buff.Corrosion, new object[] {2.0f, 50f})
            };
        }

        public static DrugStack DrugStackFactory(Buff type, object[] param)
        {
            return new DrugStack {Type = type, Param = param};
        }
    }
}
