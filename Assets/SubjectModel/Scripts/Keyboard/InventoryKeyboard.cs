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
     * 与物品栏挂在同一物体下时，该物品栏可用键鼠操作
     * </summary>
     */
    public class InventoryKeyboard : MonoBehaviour
    {
        private Inventory inventory;
        private VariableDeclarations variables;

        private void Awake()
        {
            inventory = GetComponent<Inventory>();
            EventDispatchers.OteDispatcher.AddEventListener(OnOperationTransfer);
        }

        private void Update()
        {
            if (variables.Get<int>("Standonly") != 0) return;
            if (Input.GetMouseButtonDown(0)) inventory.MasterUseOnce(GetMousePosition());
            if (Input.GetKeyDown(KeyCode.R)) inventory.SlaveUseOnce();
            if (Input.GetMouseButton(0)) inventory.MasterUseKeep(GetMousePosition());
            if (Input.GetKey(KeyCode.R)) inventory.SlaveUseKeep();
            var alpha = GetAlphaDown();
            if (alpha == -1) alpha = inventory.subSelecting + (int) (Input.GetAxisRaw("Mouse ScrollWheel") * 10);
            //inventory.SwitchTo(inventory.selecting + mouseAxis);
            if (alpha >= 0 && Math.Min(inventory.SubContains.Count, Inventory.PlayerMaxSubCount) > alpha)
                inventory.subSelecting = alpha;
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