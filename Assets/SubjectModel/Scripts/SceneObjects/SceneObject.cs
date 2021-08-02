using Bolt;
using UnityEngine;

namespace SubjectModel.Scripts.SceneObjects
{
    public abstract class SceneObject : MonoBehaviour
    {
        private static readonly Vector2 Deviation = new Vector2(0f, 1.5f);
        
        public string ButtonText;
        private bool approached;
        private bool drawing;
        private Vector2 buttonSize;

        protected virtual void Start()
        {
            buttonSize = new Vector2(ButtonText.Length * 10.5f + 10f, 21f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player")) approached = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player")) approached = false;
        }

        private void OnGUI()
        {
            if (drawing) DrawGUI();
            var variable = GameObject.FindWithTag("Player").GetComponent<Variables>().declarations;
            if (variable.Get<int>("Standonly") != 0 || !approached || Camera.main == null) return;
            var position = Camera.main.WorldToScreenPoint(Utils.Vector3To2(transform.position) + Deviation);
            if (!GUI.Button(new Rect(position, buttonSize), ButtonText)) return;
            drawing = true;
            variable.Set("Standonly", variable.Get<int>("Standonly") + 1);
        }

        protected void OnClose()
        {
            drawing = false;
            var variable = GameObject.FindWithTag("Player").GetComponent<Variables>().declarations;
            variable.Set("Standonly", variable.Get<int>("Standonly") - 1);
        }

        protected abstract void DrawGUI();
    }
}