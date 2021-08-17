using System;
using System.Collections.Generic;

namespace SubjectModel.Scripts.InventorySystem
{
    public class Container
    {
        public readonly IList<IItemStack> Contains;

        public Container(IList<IItemStack> contains)
        {
            Contains = contains;
        }

        public Container()
        {
            Contains = new List<IItemStack>();
        }

        public bool Remove(IItemStack item)
        {
            if (item == null) return true;
            return Contains.Contains(item) && Contains.Remove(item);
        }

        public T Add<T>(T item) where T : IItemStack
        {
            if (item == null) return default;
            foreach (var i in Contains)
            {
                if (!i.CanMerge(item)) continue;
                i.Merge(item);
                return default;
            }

            Contains.Add(item);
            return item;
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