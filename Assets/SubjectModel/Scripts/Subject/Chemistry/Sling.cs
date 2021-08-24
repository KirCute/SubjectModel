using System;
using SubjectModel.Scripts.InventorySystem;
using UnityEngine;

namespace SubjectModel.Scripts.Subject.Chemistry
{
    /**
     * <summary>所有弹弓的子弹需要实现的接口</summary>
     */
    public interface IThrowable : IItemStack
    {
        /**
         * <summary>
         * 弹弓主使用（玩家在手持弹弓选定此物品时按左键触发）
         * 原则上只能由Sling调用
         * <param name="user">使用弹弓者的GameObject</param>
         * <param name="aim">使用时瞄准的位置（玩家鼠标的世界位置）</param>
         * </summary>
         */
        public void OnMasterThrow(GameObject user, Vector2 aim);

        /**
         * <summary>
         * 弹弓副使用（玩家在手持弹弓选定此物品时按R触发）
         * 原则上只能由Sling调用
         * <param name="user">使用弹弓者的GameObject</param>
         * </summary>
         */
        public void OnSlaveThrow(GameObject user);
    }

    /**
     * <summary>弹弓，最基础的炼金术武器</summary>
     */
    public class Sling : Weapon
    {
        public override string Name => "弹弓";

        public override void OnMasterUseKeep(GameObject user, Vector2 pos)
        {
        }

        public override void OnMasterUseOnce(GameObject user, Vector2 pos)
        {
            if (!user.GetComponent<Inventory>().TryGetSubItem(out var stone)) return;
            ((IThrowable) stone).OnMasterThrow(user, pos);
        }

        public override void OnSlaveUseKeep(GameObject user)
        {
        }

        public override void OnSlaveUseOnce(GameObject user)
        {
            if (!user.GetComponent<Inventory>().TryGetSubItem(out var stone)) return;
            ((IThrowable) stone).OnSlaveThrow(user);
        }

        public override void Selecting(GameObject user)
        {
        }

        public override void OnSelected(GameObject user)
        {
        }

        public override void LoseSelected(GameObject user)
        {
        }

        public override Func<IItemStack, bool> SubInventory()
        {
            return item => item is IThrowable;
        }
    }
}