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
        public WireInterface Vec;

        public void OnInstall(Machine machine)
        {
            Vec = machine.AddWireInterface(this);
        }

        public void OnUninstall(Machine machine)
        {
            machine.RemoveWireInterface(Vec);
            Vec = null;
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
        public WireInterface Button;
        public WireInterface Select;
        public WireInterface Direction;
        private Inventory inv;
        private float reserve;

        public void OnInstall(Machine machine)
        {
            Button = machine.AddWireInterface(this);
            Select = machine.AddWireInterface(this);
            Direction = machine.AddWireInterface(this);
            machine.TryGetComponent(out inv);
            if (inv != null) inv.Selecting = inv.Contains.IndexOf(inv.Add(new Sling()));
        }

        public void OnUninstall(Machine machine)
        {
            machine.RemoveWireInterface(Button);
            Button = null;
            machine.RemoveWireInterface(Select);
            Select = null;
            machine.RemoveWireInterface(Direction);
            Direction = null;
            inv = null;
        }

        public void Update(GameObject gameObject)
        {
            reserve -= Time.deltaTime;
            if (reserve < 0f) reserve = 0f;
            if (inv == null || !Button.Read<bool>(SignalType.Digital) || reserve > 0f) return;
            reserve = 5f;
            inv.SubSelecting = Select.Read<int>(SignalType.OneWire);
            var dir = Direction.Read<Vector2>(SignalType.OneWire);
            inv.MasterUse(dir + (gameObject.TryGetComponent<Rigidbody2D>(out var rbody)
                ? rbody.position
                : Utils.Vector3To2(gameObject.transform.position)));
        }
    }
}