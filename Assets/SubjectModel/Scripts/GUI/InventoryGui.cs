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
     * 控制主体更改时会同步更改显示的Inventory
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
            selected = children.Where(text => text.name == "Selected").FirstOrDefault(); //Selected子物体理论上只有一个
            sub = children.Where(text => text.name == "Sub").FirstOrDefault(); //Sub子物体理论上只有一个
        }

        private void Update()
        {
            if (inventory == null) return;
            selected.text = inventory.Selecting >= 0 && inventory.Contains.Count > inventory.Selecting
                ? inventory.Contains[inventory.Selecting].Name
                : "";
            sub.text = BuildSubString();
        }

        private string BuildSubString()
        {
            var sb = new StringBuilder("");
            for (var i = inventory.SubContains.Count - 1; i >= 0; i--)
            {
                if (i >= Inventory.PlayerMaxSubCount) continue;
                sb.Append(inventory.SubSelecting == i ? "> " : "  ");
                sb.Append($"{i + 1} - {inventory.SubContains[i].Name}");
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