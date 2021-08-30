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
        public readonly IList<IItemStack> Contains; //箱子的内容物
        public IItemStack this[int index] //用于美化代码
        {
            get => Contains[index];
            set => Contains[index] = value;
        }

        public int Count => Contains.Count; //用于美化代码

        public Container(IList<IItemStack> contains)
        {
            Contains = contains;
        }

        public Container() : this(new List<IItemStack>())
        {
        }

        /**
         * <summary>
         * 从容器中删除某一物品
         * <param name="item">要删除的物品</param>
         * <returns>成功与否（若没有找到该物品则不成功）</returns>
         * </summary>
         */
        public bool Remove(IItemStack item)
        {
            if (item == null) return true;
            return Contains.Contains(item) && Contains.Remove(item);
        }

        /**
         * <summary>
         * 向容器中添加物品
         * <param name="item">要添加的物品</param>
         * <returns>该物品</returns>
         * </summary>
         */
        public T Add<T>(T item) where T : IItemStack
        {
            if (item == null) return default;
            foreach (var i in Contains) //遍历物品
            {
                if (!i.CanMerge(item)) continue; //判断物品是否可以堆叠
                i.Merge(item); //若可以，则堆叠
                return (T) i;
            }

            Contains.Add(item);
            return item;
        }

        /**
         * <summary><code>Cleanup(Action&lt;IItemStack&gt;)</code>的默认实现</summary>
         */
        public void Cleanup()
        {
            Cleanup(item => Remove(item));
        }

        /**
         * <summary>
         * 用于清空所有数量为0的物品
         * 应当在每次修改容器内容物后均调用
         * <param name="action">清除某物品的具体实现</param>
         * </summary>
         */
        public void Cleanup(Action<IItemStack> action)
        {
            for (var i = 0; i < Contains.Count; i++)
                if (Contains[i].Count <= 0)
                {
                    action(Contains[i]);
                    i--;
                }
        }
    }
}