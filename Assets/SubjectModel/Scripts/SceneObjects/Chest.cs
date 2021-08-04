using System.Collections.Generic;
using SubjectModel.Scripts.InventorySystem;
using UnityEngine;

namespace SubjectModel.Scripts.SceneObjects
{
    public class Chest : SceneObject
    {
        private static readonly Rect WindowRect = new Rect(280f, 156f, 1360f, 768f);
        private const float ItemWidth = 668f;

        private Inventory inventory;
        private Vector2 inventoryScroll;
        private Vector2 containScroll;
        public Container contains;

        protected override void Awake()
        {
            base.Awake();
            inventory = GameObject.FindWithTag("Player").GetComponent<Inventory>();
            contains = new Container(new List<IItemStack>());
            inventoryScroll = Vector2.zero;
            containScroll = Vector2.zero;
        }

        protected override void DrawGUI()
        {
            GUILayout.Window(1, WindowRect, id =>
            {
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical("Box");
                containScroll =
                    GUILayout.BeginScrollView(containScroll, false, false, GUILayout.Width(ItemWidth));
                for (var i = 0; i < contains.Contains.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"{i} - {contains.Contains[i].GetName()}", GUILayout.ExpandWidth(true));
                    if (i != 0 && GUILayout.Button("↑", GUILayout.ExpandWidth(false)))
                    {
                        var front = contains.Contains[i - 1];
                        var behind = contains.Contains[i];
                        contains.Contains[i - 1] = behind;
                        contains.Contains[i] = front;
                    }
                    else if (i != contains.Contains.Count - 1 && GUILayout.Button("↓", GUILayout.ExpandWidth(false)))
                    {
                        var front = contains.Contains[i];
                        var behind = contains.Contains[i + 1];
                        contains.Contains[i] = behind;
                        contains.Contains[i + 1] = front;
                    }
                    else if (GUILayout.Button("→", GUILayout.ExpandWidth(false)))
                    {
                        inventory.Add(contains.Contains[i].Fetch(1));
                        var deleted = false;
                        contains.Cleanup(item =>
                        {
                            contains.Remove(item);
                            deleted = true;
                        });
                        if (deleted) i--;
                    }
                    else if (GUILayout.Button("→→", GUILayout.ExpandWidth(false)))
                    {
                        inventory.Add(contains.Contains[i]);
                        contains.Remove(contains.Contains[i]);
                        i--;
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.BeginVertical("Box");
                inventoryScroll =
                    GUILayout.BeginScrollView(inventoryScroll, false, false, GUILayout.Width(ItemWidth));
                for (var i = 0; i < inventory.bag.Contains.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("←←", GUILayout.ExpandWidth(false)))
                    {
                        contains.Add(inventory.bag.Contains[i]);
                        inventory.Remove(inventory.bag.Contains[i]);
                        i--;
                    }
                    else if (GUILayout.Button("←", GUILayout.ExpandWidth(false)))
                    {
                        contains.Add(inventory.bag.Contains[i].Fetch(1));
                    }
                    else GUILayout.Label($"{i} - {inventory.bag.Contains[i].GetName()}", GUILayout.ExpandWidth(true));

                    if (i != 0 && GUILayout.Button("↑", GUILayout.ExpandWidth(false)))
                    {
                        var front = inventory.bag.Contains[i - 1];
                        var behind = inventory.bag.Contains[i];
                        if (inventory.selecting == i) behind.LoseSelected(inventory.gameObject);
                        else if (inventory.selecting == i - 1) front.LoseSelected(inventory.gameObject);
                        inventory.bag.Contains[i - 1] = behind;
                        inventory.bag.Contains[i] = front;
                        inventory.RebuildSubInventory();
                        if (inventory.selecting == i) front.OnSelected(inventory.gameObject);
                        else if (inventory.selecting == i - 1) behind.OnSelected(inventory.gameObject);
                    }
                    else if (i != inventory.bag.Contains.Count - 1 &&
                             GUILayout.Button("↓", GUILayout.ExpandWidth(false)))
                    {
                        var front = inventory.bag.Contains[i];
                        var behind = inventory.bag.Contains[i + 1];
                        if (inventory.selecting == i + 1) behind.LoseSelected(inventory.gameObject);
                        else if (inventory.selecting == i) front.LoseSelected(inventory.gameObject);
                        inventory.bag.Contains[i] = behind;
                        inventory.bag.Contains[i + 1] = front;
                        inventory.RebuildSubInventory();
                        if (inventory.selecting == i + 1) front.OnSelected(inventory.gameObject);
                        else if (inventory.selecting == i) behind.OnSelected(inventory.gameObject);
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