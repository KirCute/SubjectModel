using SubjectModel.Scripts.Event;
using UnityEngine;

namespace SubjectModel.Scripts.System
{
    /**
     * <summary>
     * 负责处理玩家死亡自动复活的脚本
     * 需要挂在玩家物体上
     * </summary>
     */
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerRespawner : MonoBehaviour
    {
        public Vector2 respawnPoint = Vector2.zero;

        private void OnEnable()
        {
            EventDispatchers.EdeDispatcher.AddEventListener(Respawn, gameObject);
        }

        private void Respawn()
        {
            GetComponent<Rigidbody2D>().position = respawnPoint;
            EventDispatchers.OteDispatcher.DispatchEvent(gameObject);
        }

        private void OnDisable()
        {
            EventDispatchers.EdeDispatcher.RemoveEventListener(Respawn, gameObject);
        }
    }
}