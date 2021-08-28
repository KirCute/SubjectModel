using System;
using System.Collections.Generic;
using System.Linq;
using SubjectModel.Scripts.Subject.Chemistry;
using UnityEngine;

namespace SubjectModel.Scripts.Subject.Electronics
{
    public class Machine : MonoBehaviour
    {
        private readonly WireManager wireManager = new WireManager();
        private readonly IList<WireInterface> wires = new List<WireInterface>();
        private readonly IList<IMachineComponent> componentsWithoutBattery = new List<IMachineComponent>();

        private readonly Dictionary<IBattery, IList<IMachineComponent>> components =
            new Dictionary<IBattery, IList<IMachineComponent>>();

        private void Start()
        {
            if (TryGetComponent<BuffRenderer>(out var br)) br.immune = true; //所有机械均免疫效果
        }

        private void Update()
        {
            foreach (var c in components.Values.SelectMany(cl => cl)) c.Update(gameObject);
        }

        public void AddComponent(IMachineComponent component)
        {
            component.Battery = null;
            componentsWithoutBattery.Add(component);
            component.OnInstall(this);
            foreach (var require in component.PowerRequirement)
            {
                var battery = components.Keys.Where(b =>
                    Math.Abs(b.Voltage - require) < .1f && b.Watt >= components[b].Sum(c => c.Watt)).ToList();
                if (battery.Count == 0) continue;
                ChangeBattery(component, battery[0]);
                break;
            }
        }

        public bool RemoveComponent(IMachineComponent component)
        {
            foreach (var c in from cl in components.Values from c in cl where c == component select c)
            {
                c.OnUninstall(this);
                ChangeBattery(c, null);
                componentsWithoutBattery.Remove(c);
                return true;
            }

            return false;
        }

        public void AddBattery(IBattery battery)
        {
            components.Add(battery, new List<IMachineComponent>());
        }

        public bool RemoveBattery(IBattery battery)
        {
            if (!components.ContainsKey(battery)) return false;
            foreach (var component in components[battery]) ChangeBattery(component, null);
            return components.Remove(battery);
        }

        private void ChangeBattery(IMachineComponent component, IBattery target)
        {
            if (component.Battery == null) componentsWithoutBattery.Remove(component);
            else components[component.Battery].Remove(component);
            if (target == null) componentsWithoutBattery.Add(component);
            else components[target].Add(component);
            component.Battery = target;
        }

        public WireInterface AddWireInterface(IMachineComponent component, Wire wire)
        {
            var ret = wireManager.GetNewInterface();
            ret.Component = component;
            ret.Wire = wire;
            wires.Add(ret);
            return ret;
        }

        public WireInterface AddWireInterface(IMachineComponent component)
        {
            return AddWireInterface(component, wireManager.GetNewWire());
        }

        public void RemoveWireInterface(WireInterface wire)
        {
            if (!wires.Contains(wire)) return;
            wire.Component = null;
            wires.Remove(wire);
        }

        public void Connect(Wire retain, Wire remove)
        {
            foreach (var wire in wires)
                if (wire.Wire == remove)
                    wire.Wire = retain;
        }

        public void Disconnect(Wire wire, params WireInterface[] outer)
        {
            var newWire = wireManager.GetNewWire();
            foreach (var i in outer)
                if (i.Wire == wire)
                    i.Wire = newWire;
            newWire.CheckBackToPool();
        }
        
        private class WireManager
        {
            private readonly Queue<Wire> wirePool = new Queue<Wire>();
            private readonly Queue<WireInterface> interfacePool = new Queue<WireInterface>();

            public Wire GetNewWire()
            {
                return wirePool.Count > 0 ? wirePool.Dequeue() : new Wire(wirePool);
            }

            public WireInterface GetNewInterface()
            {
                return interfacePool.Count > 0 ? interfacePool.Dequeue() : new WireInterface(interfacePool);
            }
        }
    }
}