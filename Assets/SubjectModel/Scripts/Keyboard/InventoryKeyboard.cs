using System;
using Bolt;
using SubjectModel.Scripts.Event;
using SubjectModel.Scripts.InventorySystem;
using UnityEngine;

namespace SubjectModel.Scripts.Keyboard
{
    /**
     * <summary>
     * 键鼠操作物品栏脚本
     * 挂在哪里都行，但更建议挂在Utils.Input上
     * 在监听到OperationTransfer事件时（包括游戏开始时事件），它会修改正在被控制的物品栏（到被控制物体的物品栏）
     * 若被控制物品没有物品栏或没有variables，它将被禁用
     * 注意：可被控制物品的variables中必须具有Standonly变量用来判断是否有优先级更高的操作
     * 支持的操作有：
     * 左键（单点或长按）：选定中物品的主使用
     * R（单点或长按）：选定中物品的副使用
     * 主键盘数字键或滚轮：切换子物品栏选定物品
     * </summary>
     */
    public class InventoryKeyboard : MonoBehaviour
    {
        private Inventory inventory;
        private VariableDeclarations variables;

        private void Awake()
        {
            EventDispatchers.OteDispatcher.AddEventListener(OnOperationTransfer);
        }

        private void Update()
        {
            if (variables.Get<int>("Standonly") != 0) return; //只有在玩家可自由移动（无剧情、GUI）时，才能用键鼠操作物品栏
            if (Input.GetMouseButtonDown(0)) inventory.MasterUseOnce(GetMousePosition()); //左键单点触发单次主使用
            if (Input.GetKeyDown(KeyCode.R)) inventory.SlaveUseOnce(); //R单点触发单次副使用
            if (Input.GetMouseButton(0)) inventory.MasterUseKeep(GetMousePosition()); //左键长按触发连续主使用
            if (Input.GetKey(KeyCode.R)) inventory.SlaveUseKeep();; //R键长按触发连续副使用
            var alpha = GetAlphaDown(); //是否按下主键盘数字键
            if (alpha == -1) alpha = inventory.SubSelecting + (int) (Input.GetAxisRaw("Mouse ScrollWheel") * 10); //若不，以滚轮滚动情况切换副武器
            //inventory.SwitchTo(inventory.selecting + mouseAxis); //曾经滚轮用来切换主武器
            if (alpha >= 0 && Math.Min(inventory.SubContains.Count, Inventory.PlayerMaxSubCount) > alpha)
                inventory.SubSelecting = alpha;
        }

        private static Vector2 GetMousePosition()
        {
            return Camera.main == null
                ? Vector2.zero
                : Utils.Vector3To2(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

        private static int GetAlphaDown()
        {
            if (Input.GetKey(KeyCode.Alpha1)) return 0;
            if (Input.GetKey(KeyCode.Alpha2)) return 1;
            if (Input.GetKey(KeyCode.Alpha3)) return 2;
            if (Input.GetKey(KeyCode.Alpha4)) return 3;
            if (Input.GetKey(KeyCode.Alpha5)) return 4;
            if (Input.GetKey(KeyCode.Alpha6)) return 5;
            if (Input.GetKey(KeyCode.Alpha7)) return 6;
            if (Input.GetKey(KeyCode.Alpha8)) return 7;
            if (Input.GetKey(KeyCode.Alpha9)) return 8;
            if (Input.GetKey(KeyCode.Alpha0)) return 9;
            return -1;
        }

        private void OnOperationTransfer(GameObject newObject)
        {
            if (newObject.TryGetComponent<Inventory>(out var inv) && newObject.TryGetComponent<Variables>(out var v))
            {
                inventory = inv;
                variables = v.declarations;
                enabled = true;
            }
            else enabled = false;
        }
    }
}