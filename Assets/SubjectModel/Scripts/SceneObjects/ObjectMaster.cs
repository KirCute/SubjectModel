using UnityEngine;

namespace SubjectModel.Scripts.SceneObjects
{
    public class ObjectMaster : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer != 8 || !other.gameObject.TryGetComponent<SceneObject>(out var tool)) return;
            tool.approached = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer != 8 || !other.gameObject.TryGetComponent<SceneObject>(out var tool)) return;
            tool.approached = false;
        }
    }
}