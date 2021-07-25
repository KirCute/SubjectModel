using System;
using System.Collections.Generic;
using UnityEngine;

namespace SubjectModel
{
    public static class GunDictionary
    {
        private static List<Firearm> defaultInventory;
        
        public static List<Firearm> GetDefaultInventory()
        {
            if (defaultInventory != null) return defaultInventory;
            //defaultInventory = JsonUtility.FromJson<GunData>(Resources.Load<TextAsset>("Firearms").text).guns;
            defaultInventory = new List<Firearm>();
            var defaultFirearm = new Firearm("Debug Firearm");
            defaultFirearm.AddComponent(new DebugComponent());
            defaultInventory.Add(defaultFirearm);
            return defaultInventory;
        }

        /*
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
        */
    }

    public class Firearm
    {
        public readonly float[] Data;
        private readonly IList<FirearmComponent> components;

        public Firearm(string name)
        {
            Data = new[] {.0f, .0f, .0f, 1f, .0f, 1f, .0f, 1f};
            components = new List<FirearmComponent> { new DefaultComponent(name) };
        }

        public bool AddComponent(FirearmComponent component)
        {
            components.Add(component);
            Statistics();
            return true;
        }

        public FirearmComponent GetComponent(int index)
        {
            return components[index];
        }

        public bool RemoveComponent(FirearmComponent component)
        {
            var ret = components.Remove(component);
            if (ret) Statistics();
            return ret;
        }

        public void RemoveComponentAt(int index)
        {
            components.RemoveAt(index);
            Statistics();
        }

        public void Statistics()
        {
            for (var i = 0; i < Data.Length; i++)
            {
                float add = .0f, multiply = .0f;
                foreach (var component in components) component.Statistics(i, ref add, ref multiply);
                Data[i] = add * multiply;
            }
        }
    }

    [Serializable]
    public class GunData
    {
        //public List<> guns;
    }

    [Serializable]
    public struct ComponentFunction
    {
        public const int Add = 0;
        public const int Multiply = 1;

        public static readonly ComponentFunction NoEffect = new ComponentFunction
        {
            algorithm = Add,
            value = .0f
        };

        public int algorithm;
        public float value;
    }

    [Serializable]
    public abstract class FirearmComponent
    {
        public const int Sight = 0;
        public const int Barrel = 1;
        public const int Action = 2;
        public const int Body = 3;
        public const int Grip = 4;
        public const int Buttstock = 5;
        public const int Magazine = 6;
        public const int Other = 7;

        public const int Damage = 0;
        public const int Reload = 1; // 换弹匣时间
        public const int Loading = 2; // 自动换弹时间
        public const int Weight = 3;
        public const int Depth = 4;
        public const int Deviation = 5;
        public const int MaxRange = 6;
        public const int Kick = 7;

        public readonly ComponentFunction[] Function;
        public string name;

        protected FirearmComponent(string name)
        {
            this.name = name;
            Function = new[]
            {
                ComponentFunction.NoEffect, ComponentFunction.NoEffect, ComponentFunction.NoEffect,
                ComponentFunction.NoEffect, ComponentFunction.NoEffect, ComponentFunction.NoEffect,
                ComponentFunction.NoEffect, ComponentFunction.NoEffect
            };
        }

        public void Statistics(int type, ref float addValue, ref float multiplyValue)
        {
            switch (Function[type].algorithm)
            {
                case ComponentFunction.Add:
                    addValue += Function[type].value;
                    break;
                case ComponentFunction.Multiply:
                    multiplyValue += Function[type].value;
                    break;
            }
        }

        public abstract int GetComponentType();
    }

    public class DefaultComponent : FirearmComponent
    {
        public DefaultComponent(string name) : base(name)
        {
            Function[Damage] = new ComponentFunction {algorithm = ComponentFunction.Multiply, value = 1f};
            Function[Reload] = new ComponentFunction {algorithm = ComponentFunction.Multiply, value = 1f};
            Function[Loading] = new ComponentFunction {algorithm = ComponentFunction.Multiply, value = 1f};
            Function[Weight] = new ComponentFunction {algorithm = ComponentFunction.Add, value = 1f};
            Function[Depth] = new ComponentFunction {algorithm = ComponentFunction.Multiply, value = 1f};
            Function[Deviation] = new ComponentFunction {algorithm = ComponentFunction.Add, value = 1f};
            Function[MaxRange] = new ComponentFunction {algorithm = ComponentFunction.Multiply, value = 1f};
            Function[Kick] = new ComponentFunction {algorithm = ComponentFunction.Add, value = 1f};
        }

        public override int GetComponentType()
        {
            return Other;
        }
    }
    
    public class DebugComponent : FirearmComponent
    {
        public DebugComponent() : base("Testing Data")
        {
            Function[Damage] = new ComponentFunction {algorithm = ComponentFunction.Add, value = 100.0f};
            Function[Reload] = new ComponentFunction {algorithm = ComponentFunction.Add, value = 0.75f};
            Function[Loading] = new ComponentFunction {algorithm = ComponentFunction.Add, value = 0.2f};
            Function[Weight] = new ComponentFunction {algorithm = ComponentFunction.Multiply, value = 1f};
            Function[Depth] = new ComponentFunction {algorithm = ComponentFunction.Add, value = 20.0f};
            Function[Deviation] = new ComponentFunction {algorithm = ComponentFunction.Multiply, value = 0.025f};
            Function[MaxRange] = new ComponentFunction {algorithm = ComponentFunction.Add, value = 0.5f};
            Function[Kick] = new ComponentFunction {algorithm = ComponentFunction.Multiply, value = 1f};
        }

        public override int GetComponentType()
        {
            return Other;
        }
    }
}