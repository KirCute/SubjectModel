using UnityEngine;

namespace SubjectModel.Scripts.SceneObjects
{
    /**
     * <summary>
     * 玩家的“手”
     * 需要挂载在玩家的子物体上用于检测玩家可右键点击的范围。
     * </summary>
     */
    [RequireComponent(typeof(CircleCollider2D))]
    public class ObjectMaster : MonoBehaviour
    {
        private void OnTriggerStay2D(Collider2D other)
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