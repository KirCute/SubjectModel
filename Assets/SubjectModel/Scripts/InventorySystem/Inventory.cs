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
        [HideInInspector] public int selecting = -1; //被选定的物品，未选定时为-1，只有调换选择物品位置（测试窗口和箱子）时才可以修改此值，要切换武器，修改Selecting

        public IList<IItemStack> Contains => bag.Contains;
        public IList<IItemStack> SubContains => sub.Contains;
        public int SubSelecting { get; set; }

        public int Selecting
        {
            get => selecting;
            set
            {
                if (value == selecting || value >= bag.Count || !(bag[value] is Weapon targetTool))
                    return;
                if (selecting >= 0 && selecting < bag.Count)
                    ((Weapon) bag[selecting]).LoseSelected(gameObject);
                selecting = value;
                SubSelecting = 0; //（和下句）重新配置子物品栏
                RebuildSubInventory();
                if (value >= 0) targetTool.OnSelected(gameObject);
            }
        }

        private void Update()
        {
            bag.Cleanup(item => Remove(item));
            if (Selecting >= 0 && selecting < bag.Count && bag[Selecting] is Weapon tool)
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
            if (!bag.Remove(item)) return false;
            if (hasSub && item == s) SubSelecting = 0; //如果删除的是正在被选定的子物品，则重置选定
            if (index < Selecting) selecting--; //如果删除的物品索引小于选定的物品，此时选定物品索引前移一格，故selecting自减1以保证bag[selecting]仍是选定物品
            if (index == Selecting) Selecting = -1; //如果删除的是选定物品，重置选定
            else RebuildSubInventory(); //若不是，则重新生成子物品栏
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
            //if (bag.Count == 1) bag[selecting].OnSelected(gameObject);
            //else
            if (Selecting >= 0 && ((Weapon) bag[Selecting]).SubInventory()(item)) RebuildSubInventory();
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
            if (SubSelecting >= sub.Count) return false;
            item = sub[SubSelecting];
            return true;
        }

        /**
         * <summary>
         * 武器主使用（持续）
         * <param name="pos">瞄准位置（玩家鼠标世界位置）</param>
         * </summary>
         */
        public void MasterUseKeep(Vector2 pos)
        {
            if (Selecting < 0 || Selecting >= bag.Count || !(bag[Selecting] is Weapon tool)) return;
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
            if (Selecting < 0 || Selecting >= bag.Count || !(bag[Selecting] is Weapon tool)) return;
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
            if (Selecting < 0 || Selecting >= bag.Count || !(bag[Selecting] is Weapon tool)) return;
            tool.OnMasterUseOnce(gameObject, pos);
            tool.OnMasterUseKeep(gameObject, pos);
        }

        /**
         * <summary>武器副使用（持续）</summary>
         */
        public void SlaveUseKeep()
        {
            if (Selecting < 0 || Selecting >= bag.Count || !(bag[Selecting] is Weapon tool)) return;
            tool.OnSlaveUseKeep(gameObject);
        }

        /**
         * <summary>武器副使用（单次）</summary>
         */
        public void SlaveUseOnce()
        {
            if (Selecting < 0 || Selecting >= bag.Count || !(bag[Selecting] is Weapon tool)) return;
            tool.OnSlaveUseOnce(gameObject);
        }

        /**
         * <summary>武器副使用（由AI调用，单次）</summary>
         */
        public void SlaveUse()
        {
            if (Selecting < 0 || Selecting >= bag.Count || !(bag[Selecting] is Weapon tool)) return;
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
            if (Selecting < 0 || Selecting >= bag.Count) return;
            foreach (var item in bag.Contains.Where(((Weapon) bag[Selecting]).SubInventory())) sub.Add(item);
            if (SubSelecting >= sub.Count) SubSelecting = sub.Count - 1;
        }
    }
}