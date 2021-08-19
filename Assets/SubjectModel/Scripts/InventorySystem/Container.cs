using System;
using System.Collections.Generic;

namespace SubjectModel.Scripts.InventorySystem
{
    /**
     * <summary>
     * 广义容器
     * 用于管理任何可以存放IItemStack的东西，例如物品栏，箱子。
     * 相比List&lt;IItemStack&gt;，它最大的优点是可以自动实现物品的堆叠，详见Add&lt;T&gt;(T)。
     * </summary>
     */
    public class Container
    {
        public readonly IList<IItemStack> Contains;

        public Container(IList<IItemStack> contains)
        {
            Contains = contains;
        }

        public Container() : this(new List<IItemStack>())
        {
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