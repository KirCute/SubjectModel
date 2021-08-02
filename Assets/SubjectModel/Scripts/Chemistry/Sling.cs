using System;
using SubjectModel.Scripts.InventorySystem;
using UnityEngine;

namespace SubjectModel.Scripts.Chemistry
{
    public class Sling : IItemStack
    {
        private bool fetched;

        public Sling()
        {
            fetched = false;
        }

        public string GetName()
        {
            return "弹弓";
        }

        public void OnMasterUseKeep(GameObject user, Vector2 pos)
        {
            if (!user.GetComponent<Inventory>().TryGetSubItem(out var stone)) return;
            stone.OnMasterUseKeep(user, pos);
        }

        public void OnMasterUseOnce(GameObject user, Vector2 pos)
        {
            if (!user.GetComponent<Inventory>().TryGetSubItem(out var stone)) return;
            stone.OnMasterUseOnce(user, pos);
        }

        public void OnSlaveUseKeep(GameObject user)
        {
            if (!user.GetComponent<Inventory>().TryGetSubItem(out var stone)) return;
            stone.OnSlaveUseKeep(user);
        }

        public void OnSlaveUseOnce(GameObject user)
        {
            if (!user.GetComponent<Inventory>().TryGetSubItem(out var stone)) return;
            stone.OnSlaveUseOnce(user);
        }

        public void Selecting(GameObject user)
        {
        }

        public void OnSelected(GameObject user)
        {
        }

        public void LoseSelected(GameObject user)
        {
        }

        public int GetCount()
        {
            return fetched ? 0 : 1;
        }

        public bool CanMerge(IItemStack item)
        {
            return false;
        }

        public void Merge(IItemStack item)
        {
        }

        public IItemStack Fetch(int count)
        {
            if (count > 0) fetched = true;
            return count == 0 ? new Sling {fetched = true} : new Sling();
        }

        public Func<IItemStack, bool> SubInventory()
        {
            return item => item.GetType() == typeof(DrugStack);
        }
    }
}