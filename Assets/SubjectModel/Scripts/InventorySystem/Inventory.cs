using System.Collections.Generic;
using System.Linq;
using Bolt;
using UnityEngine;

namespace SubjectModel.Scripts.InventorySystem
{
    [RequireComponent(typeof(Variables))]
    public class Inventory : MonoBehaviour
    {
        public Container bag;
        public Container sub;
        public int selecting;
        public int subSelecting;

        private void Awake()
        {
            bag = new Container(new List<IItemStack>());
            sub = new Container(new List<IItemStack>());
            selecting = 0;
            subSelecting = 0;
        }

        private void Update()
        {
            bag.Cleanup(item => Remove(item));
            if (selecting < bag.Contains.Count) bag.Contains[selecting].Selecting(gameObject);
        }

        public bool Remove(IItemStack item)
        {
            if (item == null) return true;
            var index = bag.Contains.IndexOf(item);
            var hasSub = TryGetSubItem(out var s);
            var res = bag.Remove(item);
            if (!res) return false;
            if (hasSub && s == item) subSelecting = 0;
            if (index == selecting)
            {
                subSelecting = 0;
                item.LoseSelected(gameObject);
                if (selecting == bag.Contains.Count) selecting--;
                if (selecting != -1) bag.Contains[selecting].OnSelected(gameObject);
                else selecting = 0;
            }
            else if (index < selecting) selecting--;

            RebuildSubInventory();
            return true;
        }

        public void Add(IItemStack item)
        {
            bag.Add(item);
            if (bag.Contains.Count == 1) bag.Contains[selecting].OnSelected(gameObject);
            else if (bag.Contains[selecting].SubInventory()(item)) RebuildSubInventory();
        }

        public bool TryGetSubItem(out IItemStack item)
        {
            item = null;
            if (subSelecting >= sub.Contains.Count) return false;
            item = sub.Contains[subSelecting];
            return true;
        }

        public void SwitchTo(int target)
        {
            if (target == selecting || target < 0 || target >= bag.Contains.Count) return;
            bag.Contains[selecting].LoseSelected(gameObject);
            selecting = target;
            subSelecting = 0;
            RebuildSubInventory();
            bag.Contains[target].OnSelected(gameObject);
        }

        public void MasterUseKeep(Vector2 pos)
        {
            if (selecting >= bag.Contains.Count) return;
            bag.Contains[selecting].OnMasterUseKeep(gameObject, pos);
        }

        public void MasterUseOnce(Vector2 pos)
        {
            if (selecting >= bag.Contains.Count) return;
            bag.Contains[selecting].OnMasterUseOnce(gameObject, pos);
        }

        public void SlaveUseKeep()
        {
            if (selecting >= bag.Contains.Count) return;
            bag.Contains[selecting].OnSlaveUseKeep(gameObject);
        }

        public void SlaveUseOnce()
        {
            if (selecting >= bag.Contains.Count) return;
            bag.Contains[selecting].OnSlaveUseOnce(gameObject);
        }

        public void RebuildSubInventory()
        {
            if (selecting >= bag.Contains.Count) return;
            sub.Contains.Clear();
            foreach (var item in bag.Contains.Where(bag.Contains[selecting].SubInventory())) sub.Add(item);
        }
    }
}