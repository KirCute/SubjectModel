using Bolt;
using UnityEngine;

namespace SubjectModel.Scripts.SceneObjects
{
    [RequireComponent(typeof(SpriteRenderer))]
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
            var color = approached && playerVariables.Get<int>("Standonly") == 0;
            GetComponent<SpriteRenderer>().color = color ? HighLight : LowLight;
        }

        private void OnMouseOver()
        {
            if (!approached || playerVariables.Get<int>("Standonly") != 0) return;
            if (Input.GetMouseButtonDown(1)) Open();
        }

        private void OnGUI()
        {
            if (!drawing) return;
            DrawGUI();
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)) Close();
        }

        private void Open()
        {
            drawing = true;
            playerVariables.Set("Standonly", playerVariables.Get<int>("Standonly") + 1);
        }

        private void Close()
        {
            drawing = false;
            playerVariables.Set("Standonly", playerVariables.Get<int>("Standonly") - 1);
        }

        protected abstract void DrawGUI();
    }
}