using System.Linq;
using SubjectModel.Scripts.InventorySystem;
using SubjectModel.Scripts.Subject.Firearms;
using UnityEngine;

namespace SubjectModel.Scripts.SceneObjects
{
    /**
     * <summary>枪械整备台</summary>
     */
    public class FirearmTable : SceneObject
    {
        private static readonly Rect WindowRect = new Rect(280f, 156f, 1366f, 768f);
        private const float ItemWidth = 668f;

        private Vector2 magazineScroll = Vector2.zero;
        private Vector2 bulletScroll = Vector2.zero;
        private Magazine selecting;

        protected override void DrawGUI()
        {
            GUILayout.Window(1, WindowRect, id =>
            {
                var inv = GameObject.FindWithTag("Player").GetComponent<Inventory>();
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical("Box");
                magazineScroll = GUILayout.BeginScrollView(magazineScroll, false, false, GUILayout.Width(ItemWidth));
                foreach (var magazine in inv.Contains
                    .Where(i => i is Magazine)
                    .Select(i => (Magazine) i))
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(selecting == magazine ? "." : "o", GUILayout.ExpandWidth(false)))
                        selecting = magazine;
                    GUILayout.Label(magazine.GetName(), GUILayout.ExpandWidth(true));
                    if (GUILayout.Button("→", GUILayout.ExpandWidth(false))) magazine.Release(inv);
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.BeginVertical("Box");
                bulletScroll = GUILayout.BeginScrollView(bulletScroll, false, false, GUILayout.Width(ItemWidth));
                foreach (var bullet in inv.Contains
                    .Where(selecting == null ? i => i is Bullet : selecting.AppropriateBullet())
                    .Select(i => (Bullet) i))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(bullet.GetName(), GUILayout.ExpandWidth(true));
                    if (selecting != null && GUILayout.Button("←", GUILayout.ExpandWidth(false)))
                        selecting.Load(inv, bullet);
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }, buttonText);
        }

        protected override void Close()
        {
            selecting = null;
            base.Close();
        }
    }
}