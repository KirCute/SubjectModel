using System;
using UnityEngine;

namespace SubjectModel.Scripts.Subject.Electronics
{
    [RequireComponent(typeof(Machine))]
    public class TestCar : MonoBehaviour
    {
        private void Awake()
        {
            var machine = GetComponent<Machine>();
            machine.AddBattery(new TestBattery());
            var motor = new TestMotor();
            machine.AddComponent(motor);
            machine.AddComponent(new TestController(motor));
        }
    }

    public class TestMotor : IMachineComponent
    {
        public float[] PowerRequirement => new[] {5f, 3f};
        public float Watt => 10f;
        public float PowerUsing { get; set; }
        public Wire Button;
        private Action<Wire> buttonMerge;

        public void OnInstall(Machine machine)
        {
            buttonMerge = w => Button = w;
            Button = machine.WireApply(buttonMerge);
        }

        public void OnUninstall(Machine machine)
        {
            machine.RemoveWireMerge(Button, buttonMerge);
            buttonMerge = null;
        }

        public void Update(GameObject gameObject)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Button.Read<float>(SignalType.Stable), 0f);
        }

        public void OnShortage(GameObject gameObject)
        {
            Debug.Log("Shortage!!!");
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
        }
    }

    public class TestBattery : IBattery
    {
        public float Voltage => 5f;
        public float Watt => 20f;
        public float Capacity { get; set; } = 30f;
    }

    public class TestController : IMachineComponent
    {
        public float[] PowerRequirement => new[] {5f};
        public float PowerUsing { get; set; }
        public float Watt => 5f;
        private readonly TestMotor motor;
        private Wire entry;
        private Action<Wire> entryMerge;

        public TestController(TestMotor motor)
        {
            this.motor = motor;
        }

        public void OnInstall(Machine machine)
        {
            entryMerge = w => entry = w;
            entry = machine.WireApply(entryMerge);
            machine.WireMerge(motor.Button, entry);
        }

        public void OnUninstall(Machine machine)
        {
            machine.RemoveWireMerge(entry, entryMerge);
            entryMerge = null;
        }

        public void Update(GameObject gameObject)
        {
            entry.Output(SignalType.Stable, 5f);
        }

        public void OnShortage(GameObject gameObject)
        {
        }
    }
}