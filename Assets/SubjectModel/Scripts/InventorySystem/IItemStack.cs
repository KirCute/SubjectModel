using System;
using UnityEngine;

namespace SubjectModel.Scripts.InventorySystem
{
    public interface IItemStack
    {
        public string GetName();
        public void OnMasterUseKeep(GameObject user, Vector2 pos);
        public void OnMasterUseOnce(GameObject user, Vector2 pos);
        public void OnSlaveUseKeep(GameObject user);
        public void OnSlaveUseOnce(GameObject user);
        public void Selecting(GameObject user);
        public void OnSelected(GameObject user);
        public void LoseSelected(GameObject user);
        public int GetCount();
        public bool CanMerge(IItemStack item);
        public void Merge(IItemStack item);
        public IItemStack Fetch(int count);
        public Func<IItemStack, bool> SubInventory();
    }

    public abstract class Material : IItemStack
    {
        public abstract string GetName();
        public abstract int GetCount();
        public abstract IItemStack Fetch(int count);
        public abstract bool CanMerge(IItemStack item);
        public abstract void Merge(IItemStack item);

        public void OnMasterUseKeep(GameObject user, Vector2 pos)
        {
        }

        public void OnMasterUseOnce(GameObject user, Vector2 pos)
        {
        }

        public void OnSlaveUseKeep(GameObject user)
        {
        }

        public void OnSlaveUseOnce(GameObject user)
        {
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

        public Func<IItemStack, bool> SubInventory()
        {
            return item => false;
        }
    }
}