using System;
using Bolt;
using UnityEngine;

namespace SubjectModel.Scripts.InventorySystem
{
    [RequireComponent(typeof(Inventory))]
    [RequireComponent(typeof(Variables))]
    public class InventoryKeyboard : MonoBehaviour
    {
        private Inventory inventory;

        private void Awake()
        {
            inventory = GetComponent<Inventory>();
        }

        private void Update()
        {
            if (GetComponent<Variables>().declarations.Get<int>("Standonly") != 0) return;
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
    }
}