using System;
using System.Collections.Generic;
using System.Linq;
using Bolt;
using UnityEngine;

namespace SubjectModel
{
    public interface ITemStack
    {
        public string GetName();
        public void OnMouseClickLeft(GameObject user, Vector2 pos);
        public void OnMouseClickRight(GameObject user, Vector2 pos);
        public void OnMouseClickLeftDown(GameObject user, Vector2 pos);
        public void OnMouseClickRightDown(GameObject user, Vector2 pos);
        public void OnMouseClickLeftUp(GameObject user, Vector2 pos);
        public void OnMouseClickRightUp(GameObject user, Vector2 pos);
        public void Selecting(GameObject user);
        public void OnSelected(GameObject user);
        public void LoseSelected(GameObject user);
        public int GetCount();
        public ITemStack Fetch();
        public Func<ITemStack, bool> SubInventory();
    }

    public abstract class Material : ITemStack
    {
        public abstract string GetName();
        public abstract int GetCount();
        public abstract ITemStack Fetch();

        public void OnMouseClickLeft(GameObject user, Vector2 pos)
        {
        }

        public void OnMouseClickRight(GameObject user, Vector2 pos)
        {
        }

        public void OnMouseClickLeftDown(GameObject user, Vector2 pos)
        {
        }

        public void OnMouseClickRightDown(GameObject user, Vector2 pos)
        {
        }

        public void OnMouseClickLeftUp(GameObject user, Vector2 pos)
        {
        }

        public void OnMouseClickRightUp(GameObject user, Vector2 pos)
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

        public Func<ITemStack, bool> SubInventory()
        {
            return item => false;
        }
    }

    [RequireComponent(typeof(Variables))]
    public class Inventory : MonoBehaviour
    {
        public IList<ITemStack> bag;
        public IList<ITemStack> sub;
        public int selecting;
        public int subSelecting;

        private void Awake()
        {
            bag = new List<ITemStack>();
            sub = new List<ITemStack>();
            selecting = 0;
            subSelecting = 0;
        }

        private void Update()
        {
            for (var i = 0; i < bag.Count; i++)
                if (bag[i].GetCount() <= 0)
                {
                    Remove(bag[i]);
                    i--;
                }
            if (selecting < bag.Count) bag[selecting].Selecting(gameObject);
        }

        public bool Remove(ITemStack item)
        {
            if (item == null) return true;
            if (!bag.Contains(item)) return false;
            var index = bag.IndexOf(item);
            var hasSub = TryGetSubItem(out var s);
            if (!bag.Remove(item)) return false;
            if (hasSub && s == item) subSelecting = 0;
            if (index == selecting)
            {
                subSelecting = 0;
                item.LoseSelected(gameObject);
                if (selecting == bag.Count) selecting--;
                if (selecting != -1) bag[selecting].OnSelected(gameObject);
                else selecting = 0;
            }
            else if (index < selecting) selecting--;
            RebuildSubInventory();
            return true;
        }

        public void Add(ITemStack item)
        {
            if (item == null) return;
            bag.Add(item);
            if (bag.Count == 1)
            {
                bag[selecting].OnSelected(gameObject);
                foreach (var i in bag.Where(bag[selecting].SubInventory())) sub.Add(i);
            }
            if (bag[selecting].SubInventory()(item)) sub.Add(item);
        }

        public bool TryGetSubItem(out ITemStack item)
        {
            item = null;
            if (subSelecting >= sub.Count) return false;
            item = sub[subSelecting];
            return true;
        }

        public void SwitchTo(int target)
        {
            if (target == selecting || target < 0 || target >= bag.Count) return;
            bag[selecting].LoseSelected(gameObject);
            selecting = target;
            subSelecting = 0;
            RebuildSubInventory();
            bag[target].OnSelected(gameObject);
        }

        public void LeftUse(Vector2 pos)
        {
            if (selecting >= bag.Count) return;
            bag[selecting].OnMouseClickLeft(gameObject, pos);
        }

        public void RightUse(Vector2 pos)
        {
            if (selecting >= bag.Count) return;
            bag[selecting].OnMouseClickRight(gameObject, pos);
        }

        public void LeftUseDown(Vector2 pos)
        {
            if (selecting >= bag.Count) return;
            bag[selecting].OnMouseClickLeftDown(gameObject, pos);
        }

        public void RightUseDown(Vector2 pos)
        {
            if (selecting >= bag.Count) return;
            bag[selecting].OnMouseClickRightDown(gameObject, pos);
        }

        public void LeftUseUp(Vector2 pos)
        {
            if (selecting >= bag.Count) return;
            bag[selecting].OnMouseClickLeftUp(gameObject, pos);
        }

        public void RightUseUp(Vector2 pos)
        {
            if (selecting >= bag.Count) return;
            bag[selecting].OnMouseClickRightUp(gameObject, pos);
        }

        private void RebuildSubInventory()
        {
            if (selecting >= bag.Count) return;
            sub.Clear();
            foreach (var item in bag.Where(bag[selecting].SubInventory())) sub.Add(item);
        }
    }
}