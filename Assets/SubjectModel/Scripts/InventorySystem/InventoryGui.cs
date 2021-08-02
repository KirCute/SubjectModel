using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace SubjectModel.Scripts.InventorySystem
{
    public class InventoryGui : MonoBehaviour
    {
        private Inventory inventory;
        private Text previous;
        private Text selected;
        private Text next;
        private Text sub;

        private void Start()
        {
            inventory = GameObject.FindWithTag("Player").GetComponent<Inventory>();
            foreach (var text in GetComponentsInChildren<Text>().Where(text => text.gameObject.name == "Previous"))
                previous = text;
            foreach (var text in GetComponentsInChildren<Text>().Where(text => text.gameObject.name == "Selected"))
                selected = text;
            foreach (var text in GetComponentsInChildren<Text>().Where(text => text.gameObject.name == "Next"))
                next = text;
            foreach (var text in GetComponentsInChildren<Text>().Where(text => text.gameObject.name == "Sub"))
                sub = text;
        }

        private void Update()
        {
            previous.text = inventory.selecting == 0 ? "" : inventory.bag.Contains[inventory.selecting - 1].GetName();
            next.text = inventory.selecting >= inventory.bag.Contains.Count - 1
                ? ""
                : inventory.bag.Contains[inventory.selecting + 1].GetName();
            selected.text = inventory.bag.Contains.Count > inventory.selecting
                ? inventory.bag.Contains[inventory.selecting].GetName()
                : "";
            sub.text = inventory.sub.Contains.Count == 0 ? "" : BuildSubString();
        }

        private string BuildSubString()
        {
            var sb = new StringBuilder();
            for (var i = inventory.sub.Contains.Count - 1; i >= 0; i--)
            {
                sb.Append(inventory.subSelecting == i ? "> " : "  ");
                sb.Append(inventory.sub.Contains[i].GetName());
                sb.Append("\n");
            }

            return sb.ToString();
        }
    }
}