using System.Collections.Generic;
using System.Linq;
using Bolt;
using UnityEngine;

namespace SubjectModel.Scripts.InventorySystem
{
    /**
     * <summary>
     * 物品栏
     * 所有作战单位都必须挂载物品栏
     * 物品栏含有一个容器和一个武器选定项，只有武器被物品栏选定时才可以使用
     * </summary>
     */
    [RequireComponent(typeof(Variables))]
    public class Inventory : MonoBehaviour
    {
        public const int PlayerMaxSubCount = 20; //玩家物品栏的最大子物品数

        private readonly Container bag = new Container(); //物品栏的容器
        private readonly Container sub = new Container(); //武器的子物品候选格，是假容器，随时更新
        [HideInInspector] public int selecting = -1; //被选定的物品，未选定时为-1
        [HideInInspector] public int subSelecting; //被选定的子物品

        public IList<IItemStack> Contains => bag.Contains;
        public IList<IItemStack> SubContains => sub.Contains;

        private void Update()
        {
            bag.Cleanup(item => Remove(item));
            if (selecting >= 0 && selecting < bag.Contains.Count && bag.Contains[selecting] is Weapon tool)
                tool.Selecting(gameObject);
        }

        /**
         * <summary>
         * 从物品栏中删除一个物品
         * <param name="item">要删除的物品</param>
         * <returns>成功与否</returns>
         * </summary>
         */
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
                selecting = -1;
                if (item is Weapon tool) tool.LoseSelected(gameObject);
            }
            else if (index < selecting) selecting--;

            RebuildSubInventory();
            return true;
        }

        /**
         * <summary>
         * 向物品栏中添加物品
         * <param name="item">要添加的物品</param>
         * <returns>该物品</returns>
         * </summary>
         */
        public T Add<T>(T item) where T : IItemStack
        {
            if (item == null) return default;
            bag.Add(item);
            //if (bag.Contains.Count == 1) bag.Contains[selecting].OnSelected(gameObject);
            //else
            if (selecting >= 0 && ((Weapon) bag.Contains[selecting]).SubInventory()(item)) RebuildSubInventory();
            return item;
        }

        /**
         * <summary>
         * 得到当前选定的子物品
         * <param name="item">输出，当前选定的子物品</param>
         * <returns>成功与否</returns>
         * </summary>
         */
        public bool TryGetSubItem(out IItemStack item)
        {
            item = null;
            if (subSelecting >= sub.Contains.Count) return false;
            item = sub.Contains[subSelecting];
            return true;
        }

        /**
         * <summary>
         * 切换武器
         * <param name="target">目标武器所在格索引</param>
         * </summary>
         */
        public void SwitchTo(int target)
        {
            if (target == selecting || target >= bag.Contains.Count || !(bag.Contains[target] is Weapon targetTool))
                return;
            if (selecting >= 0 && selecting < bag.Contains.Count)
                ((Weapon) bag.Contains[selecting]).LoseSelected(gameObject);
            selecting = target;
            subSelecting = 0;
            RebuildSubInventory();
            if (target >= 0) targetTool.OnSelected(gameObject);
        }

        /**
         * <summary>
         * 武器主使用（持续）
         * <param name="pos">瞄准位置（玩家鼠标世界位置）</param>
         * </summary>
         */
        public void MasterUseKeep(Vector2 pos)
        {
            if (selecting < 0 || selecting >= bag.Contains.Count || !(bag.Contains[selecting] is Weapon tool)) return;
            tool.OnMasterUseKeep(gameObject, pos);
        }

        /**
         * <summary>
         * 武器主使用（单次）
         * <param name="pos">瞄准位置（玩家鼠标世界位置）</param>
         * </summary>
         */
        public void MasterUseOnce(Vector2 pos)
        {
            if (selecting < 0 || selecting >= bag.Contains.Count || !(bag.Contains[selecting] is Weapon tool)) return;
            tool.OnMasterUseOnce(gameObject, pos);
        }

        /**
         * <summary>
         * 武器主使用（由AI调用，单次）
         * <param name="pos">瞄准位置</param>
         * </summary>
         */
        public void MasterUse(Vector2 pos)
        {
            if (selecting < 0 || selecting >= bag.Contains.Count || !(bag.Contains[selecting] is Weapon tool)) return;
            tool.OnMasterUseOnce(gameObject, pos);
            tool.OnMasterUseKeep(gameObject, pos);
        }

        /**
         * <summary>武器副使用（持续）</summary>
         */
        public void SlaveUseKeep()
        {
            if (selecting < 0 || selecting >= bag.Contains.Count || !(bag.Contains[selecting] is Weapon tool)) return;
            tool.OnSlaveUseKeep(gameObject);
        }

        /**
         * <summary>武器副使用（单次）</summary>
         */
        public void SlaveUseOnce()
        {
            if (selecting < 0 || selecting >= bag.Contains.Count || !(bag.Contains[selecting] is Weapon tool)) return;
            tool.OnSlaveUseOnce(gameObject);
        }

        /**
         * <summary>武器副使用（由AI调用，单次）</summary>
         */
        public void SlaveUse()
        {
            if (selecting < 0 || selecting >= bag.Contains.Count || !(bag.Contains[selecting] is Weapon tool)) return;
            tool.OnSlaveUseOnce(gameObject);
            tool.OnSlaveUseKeep(gameObject);
        }

        /**
         * <summary>
         * 重构子物品栏
         * 应在切换武器或物品栏内容物改变时调用
         * </summary>
         */
        private void RebuildSubInventory()
        {
            sub.Contains.Clear();
            if (selecting < 0 || selecting >= bag.Contains.Count) return;
            foreach (var item in bag.Contains.Where(((Weapon) bag.Contains[selecting]).SubInventory())) sub.Add(item);
        }
    }
}