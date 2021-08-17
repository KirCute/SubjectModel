using System.Collections.Generic;
using System.Linq;
using Bolt;
using UnityEngine;

namespace SubjectModel.Scripts.InventorySystem
{
    [RequireComponent(typeof(Variables))]
    public class Inventory : MonoBehaviour
    {
        public const int PlayerMaxSubCount = 20;
        
        private Container bag;
        private Container sub;
        public int selecting;
        public int subSelecting;

        public IList<IItemStack> Contains => bag.Contains;
        public IList<IItemStack> SubContains => sub.Contains;

        private void Awake()
        {
            bag = new Container();
            sub = new Container();
            selecting = -1;
            subSelecting = 0;
        }

        private void Update()
        {
            bag.Cleanup(item => Remove(item));
            if (selecting >= 0 && selecting < bag.Contains.Count && bag.Contains[selecting] is Weapon tool)
                tool.Selecting(gameObject);
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
                selecting = -1;
                if (item is Weapon tool) tool.LoseSelected(gameObject);
            }
            else if (index < selecting) selecting--;

            RebuildSubInventory();
            return true;
        }

        public T Add<T>(T item) where T : IItemStack
        {
            if (item == null) return default;
            bag.Add(item);
            //if (bag.Contains.Count == 1) bag.Contains[selecting].OnSelected(gameObject);
            //else
            if (selecting >= 0 && ((Weapon) bag.Contains[selecting]).SubInventory()(item)) RebuildSubInventory();
            return item;
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
            if (target == selecting || target >= bag.Contains.Count || !(bag.Contains[target] is Weapon targetTool))
                return;
            if (selecting >= 0 && selecting < bag.Contains.Count)
                ((Weapon) bag.Contains[selecting]).LoseSelected(gameObject);
            selecting = target;
            subSelecting = 0;
            RebuildSubInventory();
            if (target >= 0) targetTool.OnSelected(gameObject);
        }

        public void MasterUseKeep(Vector2 pos)
        {
            if (selecting < 0 || selecting >= bag.Contains.Count || !(bag.Contains[selecting] is Weapon tool)) return;
            tool.OnMasterUseKeep(gameObject, pos);
        }

        public void MasterUseOnce(Vector2 pos)
        {
            if (selecting < 0 || selecting >= bag.Contains.Count || !(bag.Contains[selecting] is Weapon tool)) return;
            tool.OnMasterUseOnce(gameObject, pos);
        }

        public void SlaveUseKeep()
        {
            if (selecting < 0 || selecting >= bag.Contains.Count || !(bag.Contains[selecting] is Weapon tool)) return;
            tool.OnSlaveUseKeep(gameObject);
        }

        public void SlaveUseOnce()
        {
            if (selecting < 0 || selecting >= bag.Contains.Count || !(bag.Contains[selecting] is Weapon tool)) return;
            tool.OnSlaveUseOnce(gameObject);
        }

        private void RebuildSubInventory()
        {
            sub.Contains.Clear();
            if (selecting < 0 || selecting >= bag.Contains.Count) return;
            foreach (var item in bag.Contains.Where(((Weapon) bag.Contains[selecting]).SubInventory())) sub.Add(item);
        }
    }
}