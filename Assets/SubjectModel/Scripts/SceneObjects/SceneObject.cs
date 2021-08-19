using Bolt;
using UnityEngine;

namespace SubjectModel.Scripts.SceneObjects
{
    /**
     * <summary>
     * 场景中任何可以被玩家右键使用的物品的基类
     * </summary>
     */
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class SceneObject : MonoBehaviour
    {
        private static readonly Color LowLight = new Vector4(0.9f, 0.9f, 0.9f, 1f);
        private static readonly Color HighLight = new Vector4(1f, 1f, 1f, 1f);

        private VariableDeclarations playerVariables;
        private bool drawing;
        public string buttonText;
        public bool approached;

        protected virtual void Awake()
        {
            playerVariables = GameObject.FindWithTag("Player").GetComponent<Variables>().declarations;
        }

        protected virtual void Update()
        {
            var color = approached && playerVariables.Get<int>("Standonly") == 0; //只有玩家在可移动状态可以与场景物品交互
            GetComponent<SpriteRenderer>().color = color ? HighLight : LowLight;
        }

        protected virtual void OnMouseOver()
        {
            if (!approached || playerVariables.Get<int>("Standonly") != 0) return; //只有玩家在可移动状态可以与场景物品交互
            if (Input.GetMouseButtonDown(1)) Open();
        }

        private void OnGUI()
        {
            if (!drawing) return;
            DrawGUI();
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)) Close();
        }

        protected virtual void Open()
        {
            drawing = true;
            playerVariables.Set("Standonly", playerVariables.Get<int>("Standonly") + 1); //使玩家进入不可以动状态
        }

        protected virtual void Close()
        {
            drawing = false;
            playerVariables.Set("Standonly", playerVariables.Get<int>("Standonly") - 1); //使玩家回到可移动状态
        }

        protected abstract void DrawGUI();
    }
}