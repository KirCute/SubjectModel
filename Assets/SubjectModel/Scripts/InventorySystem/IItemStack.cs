using System;
using UnityEngine;

namespace SubjectModel.Scripts.InventorySystem
{
    public interface IItemStack
    {
        public string GetName();
        public int GetCount();
        public bool CanMerge(IItemStack item);
        public void Merge(IItemStack item);
        public IItemStack Fetch(int count);
    }

    public abstract class Weapon : Unstackable
    {
        public abstract void OnMasterUseKeep(GameObject user, Vector2 pos);
        public abstract void OnMasterUseOnce(GameObject user, Vector2 pos);
        public abstract void OnSlaveUseKeep(GameObject user);
        public abstract void OnSlaveUseOnce(GameObject user);
        public abstract void Selecting(GameObject user);
        public abstract void OnSelected(GameObject user);
        public abstract void LoseSelected(GameObject user);
        public abstract Func<IItemStack, bool> SubInventory();

        public void OnMasterUse(GameObject user, Vector2 pos)
        {
            OnMasterUseOnce(user, pos);
            OnMasterUseKeep(user, pos);
        }

        public void OnSlaveUse(GameObject user)
        {
            OnSlaveUseOnce(user);
            OnSlaveUseKeep(user);
        }
    }

    public abstract class Unstackable : IItemStack
    {
        private bool fetched;
        public abstract string GetName();

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

        public virtual IItemStack Fetch(int count)
        {
            if (count > 0) fetched = true;
            return this;
        }
    }
}