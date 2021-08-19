using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace SubjectModel.Scripts.InventorySystem
{
    /**
     * <summary>
     * 屏幕展示物品栏
     * inventory变量有值时，会在屏幕左下角展示选定武器及其子物品
     * </summary>
     */
    public class InventoryGui : MonoBehaviour
    {
        private Text selected;
        private Text sub;
        public Inventory inventory;

        private void Start()
        {
            foreach (var text in GetComponentsInChildren<Text>().Where(text => text.gameObject.name == "Selected"))
                selected = text;
            foreach (var text in GetComponentsInChildren<Text>().Where(text => text.gameObject.name == "Sub"))
                sub = text;
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
    }
}