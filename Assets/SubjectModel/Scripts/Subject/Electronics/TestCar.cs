using System;
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
            machine.WireMerge(motor.Vec, sensor.Vec);
            machine.WireMerge(motor.Button, catapult.Button);
            machine.WireMerge(motor.Select, catapult.Select);
            machine.WireMerge(catapult.Direction, motor.Vec);
        }

        private void Start()
        {
            GetComponent<Inventory>().Add(new DrugStack
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
                Element.Acid,
                10000
            ));
        }
    }

    public class TestMotor : IMachineComponent
    {
        public IEnumerable<float> PowerRequirement => new[] {5f, 3f};
        public float Watt => 10f;
        public IBattery Battery { get; set; }
        public GameObject Soul;
        public Wire Vec;
        private Action<Wire> vecMerge;
        public Wire Select;
        private Action<Wire> selectMerge;
        public Wire Button;
        private Action<Wire> buttonMerge;

        public void OnInstall(Machine machine)
        {
            vecMerge = w => Vec = w;
            machine.WireApply(vecMerge);
            selectMerge = w => Select = w;
            machine.WireApply(selectMerge);
            buttonMerge = w => Button = w;
            machine.WireApply(buttonMerge);
        }

        public void OnUninstall(Machine machine)
        {
            machine.RemoveWireMerge(Vec, vecMerge);
            vecMerge = null;
            machine.RemoveWireMerge(Select, selectMerge);
            selectMerge = null;
            machine.RemoveWireMerge(Button, buttonMerge);
            buttonMerge = null;
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