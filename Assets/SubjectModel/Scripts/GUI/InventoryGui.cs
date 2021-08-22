using System.Linq;
using System.Text;
using SubjectModel.Scripts.Event;
using SubjectModel.Scripts.InventorySystem;
using UnityEngine;
using UnityEngine.UI;

namespace SubjectModel.Scripts.GUI
{
    /**
     * <summary>
     * 屏幕展示物品栏
     * inventory变量有值时，会在屏幕左下角展示选定武器及其子物品
     * 需要挂在画布下的空物体上，该物体有“Selected”和“Sub”两个挂有Text的子物体
     * </summary>
     */
    public class InventoryGui : MonoBehaviour
    {
        public Inventory inventory;
        private Text selected;
        private Text sub;

        private void OnEnable()
        {
            EventDispatchers.OteDispatcher.AddEventListener(OnOperationTransfer);
        }

        private void Start()
        {
            var children = GetComponentsInChildren<Text>();
            selected = children.Where(text => text.name == "Selected").FirstOrDefault();
            sub = children.Where(text => text.name == "Sub").FirstOrDefault();
        }

        private void Update()
        {
            if (inventory == null) return;
            selected.text = inventory.selecting >= 0 && inventory.Contains.Count > inventory.selecting
                ? inventory.Contains[inventory.selecting].GetName()
                : "";
            sub.text = inventory.SubContains.Count == 0 ? "" : BuildSubString();
        }

        private string BuildSubString()
        {
            var sb = new StringBuilder();
            for (var i = inventory.SubContains.Count - 1; i >= 0; i--)
            {
                if (i >= Inventory.PlayerMaxSubCount) continue;
                sb.Append(inventory.subSelecting == i ? "> " : "  ");
                sb.Append($"{i + 1} - {inventory.SubContains[i].GetName()}");
                sb.Append("\n");
            }

            return sb.ToString();
        }

        private void OnDisable()
        {
            EventDispatchers.OteDispatcher.RemoveEventListener(OnOperationTransfer);
            selected.text = "";
            sub.text = "";
        }
        
        private void OnOperationTransfer(GameObject newObject)
        {
            if (newObject.TryGetComponent<Inventory>(out var inv))
            {
                inventory = inv;
                enabled = true;
            }
            else enabled = false;
        }
    }
}