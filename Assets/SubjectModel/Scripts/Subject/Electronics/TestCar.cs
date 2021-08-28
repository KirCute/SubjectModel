using System.Collections.Generic;
using Bolt;
using SubjectModel.Scripts.InventorySystem;
using SubjectModel.Scripts.Subject.Chemistry;
using SubjectModel.Scripts.System;
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
            var sensor = new DistanceMagicSensor();
            machine.AddComponent(sensor);
            var catapult = new Catapult();
            machine.AddComponent(catapult);
            motor.Soul = GameObject.FindWithTag("Player");
            sensor.Soul = GameObject.FindWithTag("Player");
            machine.Connect(motor.Vec.Wire, sensor.Vec.Wire);
            machine.Connect(motor.Button.Wire, catapult.Button.Wire);
            machine.Connect(motor.Select.Wire, catapult.Select.Wire);
            machine.Connect(catapult.Direction.Wire, motor.Vec.Wire);
        }

        private void Start()
        {
            GetComponent<Inventory>().Add(new SealStack(new DrugStack
            (
                "CoSO4",
                new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.Get("Co"), Index = Elements.Get("Co").GetIndex(2),
                        Amount = 1f, Concentration = 1f
                    }
                },
                Element.Acid
            ), 10000));
        }
    }

    public class TestMotor : IMachineComponent
    {
        public IEnumerable<float> PowerRequirement => new[] {5f, 3f};
        public float Watt => 10f;
        public IBattery Battery { get; set; }
        public GameObject Soul;
        public WireInterface Vec;
        public WireInterface Select;
        public WireInterface Button;

        public void OnInstall(Machine machine)
        {
            Vec = machine.AddWireInterface(this);
            Select = machine.AddWireInterface(this);
            Button = machine.AddWireInterface(this);
        }

        public void OnUninstall(Machine machine)
        {
            machine.RemoveWireInterface(Vec);
            Vec = null;
            machine.RemoveWireInterface(Select);
            Select = null;
            machine.RemoveWireInterface(Button);
            Button = null;
        }

        public void Update(GameObject gameObject)
        {
            var dir = Vec.Read<Vector2>(SignalType.OneWire);
            gameObject.GetComponent<Movement>().Motivation = dir.magnitude > 1.5f ? dir : Vector2.zero;
            Select.Output(SignalType.OneWire, 0);
            Button.Output(SignalType.Digital,
                dir.magnitude <= BuffInvoker.MaxDistance &&
                Soul.GetComponent<Variables>().declarations.Get<float>("Health") < 70f);
        }
    }

    public class TestBattery : IBattery
    {
        public float Voltage => 5f;
        public float Watt => 40f;
        private float capacity = 30000f;

        public bool Cost(float amount)
        {
            if (capacity < amount) return false;
            capacity -= amount;
            return true;
        }
    }
}