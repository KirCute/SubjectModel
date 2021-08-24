using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SubjectModel.Scripts.Subject.Electronics
{
    public class Machine : MonoBehaviour
    {
        private readonly Dictionary<Wire, List<Action<Wire>>> wires = new Dictionary<Wire, List<Action<Wire>>>();
        private readonly Dictionary<float, float> powerSupply = new Dictionary<float, float>();
        private readonly List<IBattery> batteries = new List<IBattery>();
        private readonly List<IMachineComponent> components = new List<IMachineComponent>();

        private void Update()
        {
            foreach (var component in components)
            {
                var bty = batteries.Where(b => Math.Abs(b.Voltage - component.PowerUsing) < .1f && b.Capacity > 0f);
                var battery = bty.ToList();
                if (battery.Count == 0) component.OnShortage(gameObject);
                else
                {
                    battery[0].Capacity -= component.Watt * Time.deltaTime;
                    component.Update(gameObject);
                }
            }
        }

        public void AddComponent(IMachineComponent component)
        {
            components.Add(component);
            component.OnInstall(this);
            foreach (var voltage in component.PowerRequirement)
            {
                if (!powerSupply.ContainsKey(voltage)) continue;
                component.PowerUsing = voltage;
                break;
            }
        }

        public bool RemoveComponent(IMachineComponent component)
        {
            if (!components.Contains(component)) return false;
            component.OnUninstall(this);
            component.PowerUsing = 0f;
            return components.Remove(component);
        }

        public bool RemoveBattery(IBattery battery)
        {
            if (!batteries.Contains(battery)) return false;
            powerSupply[battery.Voltage] -= battery.Watt;
            if (powerSupply[battery.Voltage] <= .1f) powerSupply.Remove(battery.Voltage);
            return batteries.Remove(battery);
        }

        public void AddBattery(IBattery battery)
        {
            batteries.Add(battery);
            if (!powerSupply.ContainsKey(battery.Voltage)) powerSupply.Add(battery.Voltage, .0f);
            powerSupply[battery.Voltage] += battery.Watt;
        }

        public Wire WireApply(Action<Wire> merge)
        {
            var ret = new Wire();
            wires.Add(ret, new List<Action<Wire>> {merge});
            return ret;
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

        private bool IsPowerShortage()
        {
            var requirements = powerSupply.Keys.ToDictionary(k => k, k => 0f);
            foreach (var component in components.Where(c => c.PowerUsing > .1f))
                requirements[component.PowerUsing] += component.Watt;
            return requirements.Any(e => e.Value > powerSupply[e.Key]);
        }
    }
}