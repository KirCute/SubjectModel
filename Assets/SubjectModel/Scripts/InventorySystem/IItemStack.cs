using System;
using UnityEngine;

namespace SubjectModel.Scripts.InventorySystem
{
    /**
     * <summary>
     * 物品基类
     * Container中只能存放实现了此接口的类的实例
     * </summary>
     */
    public interface IItemStack
    {
        /**
         * <summary>得到物品在物品栏中显示的名称</summary>
         */
        public string Name { get; }

        /**
         * <summary>得到物品的数量</summary>
         */
        public int Count { get; }

        /**
         * <summary>
         * 得到物品与另一物品堆叠的可能性
         * <param name="item">要判断的另一物品</param>
         * <returns>能否堆叠</returns>
         * </summary>
         */
        public bool CanMerge(IItemStack item);

        /**
         * <summary>
         * 堆叠两物品
         * 应先执行CanMerge判断是否可堆叠
         * <param name="item">要堆叠的物品</param>
         * </summary>
         */
        public void Merge(IItemStack item);

        /**
         * <summary>
         * 从一堆物品中取出count个
         * 若数量不足则全部取走
         * <returns>取出的count个物品</returns>
         * </summary>
         */
        public IItemStack Fetch(int count);
    }

    /**
     * <summary>不可堆叠物品的基类</summary>
     */
    public abstract class Unstackable : IItemStack
    {
        private bool fetched;
        public abstract string Name { get; }
        public int Count => fetched ? 0 : 1;

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

    /**
     * <summary>
     * 武器基类
     * 只有继承了此类的物品可以被玩家手持
     * </summary>
     */
    public abstract class Weapon : Unstackable
    {
        /**
         * <summary>
         * 持续主使用（玩家在按住左键时触发）
         * 原则上只能由Inventory调用
         * <param name="user">使用物品者的GameObject</param>
         * <param name="aim">使用时瞄准的位置（玩家鼠标的世界位置）</param>
         * </summary>
         */
        public abstract void OnMasterUseKeep(GameObject user, Vector2 aim);

        /**
         * <summary>
         * 单次主使用（玩家单击左键或按住左键的第一帧时触发）
         * 原则上只能由Inventory调用
         * <param name="user">使用物品者的GameObject</param>
         * <param name="aim">使用时瞄准的位置（玩家鼠标的世界位置）</param>
         * </summary>
         */
        public abstract void OnMasterUseOnce(GameObject user, Vector2 aim);

        /**
         * <summary>
         * 持续副使用（玩家在按住R时触发）
         * 原则上只能由Inventory调用
         * <param name="user">使用物品者的GameObject</param>
         * </summary>
         */
        public abstract void OnSlaveUseKeep(GameObject user);

        /**
         * <summary>
         * 持续副使用（玩家在单击R或按住R的第一帧时触发）
         * 原则上只能由Inventory调用
         * <param name="user">使用物品者的GameObject</param>
         * </summary>
         */
        public abstract void OnSlaveUseOnce(GameObject user);

        /**
         * <summary>
         * 手持时每帧更新方法
         * 原则上只能由Inventory调用
         * <param name="user">使用物品者的GameObject</param>
         * </summary>
         */
        public abstract void Selecting(GameObject user);

        /**
         * <summary>
         * 装备时调用的方法
         * 原则上只能由Inventory调用
         * <param name="user">使用物品者的GameObject</param>
         * </summary>
         */
        public abstract void OnSelected(GameObject user);

        /**
         * <summary>
         * 卸下装备时调用的方法
         * 原则上只能由Inventory调用
         * <param name="user">使用物品者的GameObject</param>
         * </summary>
         */
        public abstract void LoseSelected(GameObject user);

        /**
         * <summary>
         * 获取“判断一个物品是否为武器的子物品的方法”的方法
         * 原则上只能由Inventory调用
         * <returns>判断一个物品是否为武器的子物品的方法</returns>
         * </summary>
         */
        public abstract Func<IItemStack, bool> SubInventory();
    }
}