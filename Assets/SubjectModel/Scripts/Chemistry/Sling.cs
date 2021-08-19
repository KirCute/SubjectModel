using System;
using SubjectModel.Scripts.InventorySystem;
using UnityEngine;

namespace SubjectModel.Scripts.Chemistry
{
    /**
     * <summary>所有弹弓的子弹需要实现的接口</summary>
     */
    public interface IThrowable : IItemStack
    {
        public void OnMasterThrow(GameObject user, Vector2 aim);
        public void OnSlaveThrow(GameObject user);
    }

    /**
     * <summary>弹弓，最基础的炼金术武器</summary>
     */
    public class Sling : Weapon
    {
        public override string GetName()
        {
            return "弹弓";
        }

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