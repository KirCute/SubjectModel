using System;
using System.Collections.Generic;
using System.Linq;
using Bolt;
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
            if (bag.Contains.Count == 1)
            {
                bag.Contains[selecting].OnSelected(gameObject);
                foreach (var i in bag.Contains.Where(bag.Contains[selecting].SubInventory())) sub.Add(i);
            }

            if (bag.Contains[selecting].SubInventory()(item)) sub.Add(item);
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

    public class Container
    {
        public readonly IList<IItemStack> Contains;

        public Container(IList<IItemStack> contains)
        {
            Contains = contains;
        }

        public bool Remove(IItemStack item)
        {
            if (item == null) return true;
            return Contains.Contains(item) && Contains.Remove(item);
        }

        public void Add(IItemStack item)
        {
            if (item == null) return;
            foreach (var i in Contains)
            {
                if (!i.CanMerge(item)) continue;
                i.Merge(item);
                return;
            }

            Contains.Add(item);
        }

        public void Cleanup()
        {
            Cleanup(item => Remove(item));
        }

        public void Cleanup(Action<IItemStack> action)
        {
            for (var i = 0; i < Contains.Count; i++)
                if (Contains[i].GetCount() <= 0)
                {
                    action(Contains[i]);
                    i--;
                }
        }
    }
}