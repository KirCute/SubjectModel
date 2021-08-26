using System;
using System.Collections.Generic;
using System.Linq;
using SubjectModel.Scripts.Subject.Chemistry;
using UnityEngine;

namespace SubjectModel.Scripts.Subject.Electronics
{
    public class Machine : MonoBehaviour
    {
        private readonly Dictionary<Wire, List<Action<Wire>>> wires = new Dictionary<Wire, List<Action<Wire>>>();
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

        public void WireApply(Action<Wire> merge)
        {
            var ret = new Wire();
            wires.Add(ret, new List<Action<Wire>> {merge});
            merge(ret);
        }

        public void RemoveWireMerge(Wire wire, Action<Wire> merge)
        {
            if (!wires.ContainsKey(wire) || !wires[wire].Contains(merge)) return;
            merge(null);
            wires[wire].Remove(merge);
        }

        public void WireMerge(Wire a, Wire b)
        {
            if (!wires.ContainsKey(a) || !wires.ContainsKey(b)) return;
            var retain = wires[a].Count >= wires[b].Count ? a : b;
            var remove = wires[a].Count >= wires[b].Count ? b : a;
            foreach (var merge in wires[remove])
            {
                wires[retain].Add(merge);
                merge(retain);
            }

            wires.Remove(remove);
        }

        private void ChangeBattery(IMachineComponent component, IBattery target)
        {
            if (component.Battery == null) componentsWithoutBattery.Remove(component);
            else components[component.Battery].Remove(component);
            if (target == null) componentsWithoutBattery.Add(component);
            else components[target].Add(component);
            component.Battery = target;
        }
    }
}