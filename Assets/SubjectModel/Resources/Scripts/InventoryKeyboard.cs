using UnityEngine;


namespace SubjectModel
{
    [RequireComponent(typeof(Inventory))]
    public class InventoryKeyboard : MonoBehaviour
    {
        private Inventory inventory;
        private void Start()
        {
            inventory = GetComponent<Inventory>();
            Test.GeneratePlayerInventory(inventory);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) inventory.LeftUseDown(GetMousePosition());
            if (Input.GetMouseButtonDown(1)) inventory.RightUseDown(GetMousePosition());
            if (Input.GetMouseButton(0)) inventory.LeftUse(GetMousePosition());
            if (Input.GetMouseButton(1)) inventory.RightUse(GetMousePosition());
            if (Input.GetMouseButtonUp(0)) inventory.LeftUseUp(GetMousePosition());
            if (Input.GetMouseButtonUp(1)) inventory.RightUseUp(GetMousePosition());
            var alpha = GetAlphaDown();
            if (alpha != -1 && inventory.sub.Count > alpha) inventory.subSelecting = alpha;
            var mouseAxis = (int) (Input.GetAxisRaw("Mouse ScrollWheel") * 10);
            inventory.SwitchTo(inventory.selecting - mouseAxis);
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