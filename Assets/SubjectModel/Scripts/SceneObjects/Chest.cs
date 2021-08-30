using SubjectModel.Scripts.InventorySystem;
using UnityEngine;

namespace SubjectModel.Scripts.SceneObjects
{
    /**
     * <summary>箱子</summary>
     */
    public class Chest : SceneObject
    {
        private static readonly Rect WindowRect = new Rect(280f, 156f, 1366f, 768f);
        private const float ItemWidth = 668f;

        private Inventory inventory;
        private Vector2 inventoryScroll = Vector2.zero;
        private Vector2 containScroll = Vector2.zero;
        public readonly Container Contains = new Container();

        protected override void Awake()
        {
            base.Awake();
            inventory = GameObject.FindWithTag("Player").GetComponent<Inventory>();
        }

        protected override void DrawGUI()
        {
            GUILayout.Window(1, WindowRect, id =>
            {
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical("Box");
                containScroll = GUILayout.BeginScrollView(containScroll, false, false, GUILayout.Width(ItemWidth));
                for (var i = 0; i < Contains.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"{i} - {Contains[i].Name}", GUILayout.ExpandWidth(true));
                    if (i != 0 && GUILayout.Button("↑", GUILayout.ExpandWidth(false)))
                    {
                        var front = Contains[i - 1];
                        var behind = Contains[i];
                        Contains[i - 1] = behind;
                        Contains[i] = front;
                    }
                    else if (i != Contains.Count - 1 && GUILayout.Button("↓", GUILayout.ExpandWidth(false)))
                    {
                        var front = Contains[i];
                        var behind = Contains[i + 1];
                        Contains[i] = behind;
                        Contains[i + 1] = front;
                    }
                    else if (GUILayout.Button("→", GUILayout.ExpandWidth(false)))
                    {
                        inventory.Add(Contains[i].Fetch(1));
                        var deleted = false;
                        Contains.Cleanup(item =>
                        {
                            Contains.Remove(item);
                            deleted = true;
                        });
                        if (deleted) i--;
                    }
                    else if (GUILayout.Button("→→", GUILayout.ExpandWidth(false)))
                    {
                        inventory.Add(Contains[i]);
                        Contains.Remove(Contains[i]);
                        i--;
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.BeginVertical("Box");
                inventoryScroll = GUILayout.BeginScrollView(inventoryScroll, false, false, GUILayout.Width(ItemWidth));
                for (var i = 0; i < inventory.Contains.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("←←", GUILayout.ExpandWidth(false)))
                    {
                        Contains.Add(inventory.Contains[i]);
                        inventory.Remove(inventory.Contains[i]);
                        i--;
                    }
                    else if (GUILayout.Button("←", GUILayout.ExpandWidth(false)))
                    {
                        Contains.Add(inventory.Contains[i].Fetch(1));
                    }
                    else GUILayout.Label($"{i} - {inventory.Contains[i].Name}", GUILayout.ExpandWidth(true));

                    if (i != 0 && GUILayout.Button("↑", GUILayout.ExpandWidth(false)))
                    {
                        var front = inventory.Contains[i - 1];
                        var behind = inventory.Contains[i];
                        if (inventory.Selecting == i) inventory.selecting--;
                        else if (inventory.Selecting == i - 1) inventory.selecting++;
                        /*    behind.LoseSelected(inventory.gameObject);
                        else if (inventory.selecting == i - 1) front.LoseSelected(inventory.gameObject);
                        inventory.RebuildSubInventory();
                        if (inventory.selecting == i) front.OnSelected(inventory.gameObject);
                        else if (inventory.selecting == i - 1) behind.OnSelected(inventory.gameObject);
                        */
                        inventory.Contains[i - 1] = behind;
                        inventory.Contains[i] = front;
                    }
                    else if (i != inventory.Contains.Count - 1 &&
                             GUILayout.Button("↓", GUILayout.ExpandWidth(false)))
                    {
                        var front = inventory.Contains[i];
                        var behind = inventory.Contains[i + 1];
                        if (inventory.Selecting == i + 1) inventory.selecting--;
                        else if (inventory.Selecting == i) inventory.selecting++;
                        /*    behind.LoseSelected(inventory.gameObject);
                        else if (inventory.selecting == i) front.LoseSelected(inventory.gameObject);
                        inventory.RebuildSubInventory();
                        if (inventory.selecting == i + 1) front.OnSelected(inventory.gameObject);
                        else if (inventory.selecting == i) behind.OnSelected(inventory.gameObject);
                        */
                        inventory.Contains[i] = behind;
                        inventory.Contains[i + 1] = front;
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }, buttonText);
        }
    }
}