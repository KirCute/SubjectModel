using System;
using System.Collections.Generic;
using SubjectModel.Scripts.InventorySystem;
using SubjectModel.Scripts.Subject.Chemistry;
using UnityEngine;

namespace SubjectModel.Scripts.Subject.Electronics
{
    public interface IMachineComponent
    {
        public IEnumerable<float> PowerRequirement { get; }
        public float Watt { get; }
        public IBattery Battery { get; set; }
        public void OnInstall(Machine machine);
        public void OnUninstall(Machine machine);
        public void Update(GameObject gameObject);
    }

    public class DistanceMagicSensor : IMachineComponent
    {
        public IEnumerable<float> PowerRequirement => new[] {5f};
        public float Watt => 1f;
        public IBattery Battery { get; set; }
        public GameObject Soul;
        public Wire Vec;
        private Action<Wire> vecMerge;

        public void OnInstall(Machine machine)
        {
            vecMerge = w => Vec = w;
            machine.WireApply(vecMerge);
        }

        public void OnUninstall(Machine machine)
        {
            machine.RemoveWireMerge(Vec, vecMerge);
            vecMerge = null;
        }

        public void Update(GameObject gameObject)
        {
            Vec.Output(SignalType.OneWire,
                Battery.Cost(Watt * Time.deltaTime)
                    ? (Soul.TryGetComponent<Rigidbody2D>(out var rbody)
                          ? rbody.position
                          : Utils.Vector3To2(Soul.transform.position)) -
                      (gameObject.TryGetComponent(out rbody)
                          ? rbody.position
                          : Utils.Vector3To2(gameObject.transform.position))
                    : Vector2.zero);
        }
    }

    public class Catapult : IMachineComponent
    {
        public IEnumerable<float> PowerRequirement => new[] {5f};
        public float Watt => 20f;
        public IBattery Battery { get; set; }
        public Wire Button;
        private Action<Wire> buttonMerge;
        public Wire Select;
        private Action<Wire> selectMerge;
        public Wire Direction;
        private Action<Wire> dirMerge;
        private Inventory inv;
        private float reserve = 0f;

        public void OnInstall(Machine machine)
        {
            buttonMerge = w => Button = w;
            machine.WireApply(buttonMerge);
            selectMerge = w => Select = w;
            machine.WireApply(selectMerge);
            dirMerge = w => Direction = w;
            machine.WireApply(dirMerge);
            if (!machine.TryGetComponent(out inv)) return;
            inv.SwitchTo(inv.Contains.IndexOf(inv.Add(new Sling())));
        }

        public void OnUninstall(Machine machine)
        {
            machine.RemoveWireMerge(Button, buttonMerge);
            buttonMerge = null;
            machine.RemoveWireMerge(Select, selectMerge);
            selectMerge = null;
            machine.RemoveWireMerge(Direction, dirMerge);
            dirMerge = null;
            inv = null;
        }

        public void Update(GameObject gameObject)
        {
            reserve -= Time.deltaTime;
            if (reserve < 0f) reserve = 0f;
            if (inv == null || !Button.Read<bool>(SignalType.Digital) || reserve > 0f) return;
            reserve = 5f;
            inv.subSelecting = Select.Read<int>(SignalType.OneWire);
            var dir = Direction.Read<Vector2>(SignalType.OneWire);
            inv.MasterUse(dir + (gameObject.TryGetComponent<Rigidbody2D>(out var rbody)
                ? rbody.position
                : Utils.Vector3To2(gameObject.transform.position)));
        }
    }
}