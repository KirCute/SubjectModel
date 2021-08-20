using System.Linq;
using Bolt;
using SubjectModel.Scripts.Task;
using UnityEngine;

namespace SubjectModel.Scripts.System
{
    /**
     * <summary>
     * 负责处理玩家死亡自动复活的脚本
     * 需要挂在玩家物体上
     * </summary>
     */
    [RequireComponent(typeof(Variables))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerRespawner : MonoBehaviour
    {
        public Vector2 respawnPoint = Vector2.zero;

        private void Update()
        {
            var dec = GetComponent<Variables>().declarations;
            if (!(dec.Get<float>("Health") <= 0f) && !(dec.Get<float>("Energy") <= 0)) return;
            GameObject.FindWithTag("BossAssistance").GetComponent<BossAssistance>().InterruptFighting();
            foreach (var reminder in GameObject.FindGameObjectsWithTag("EnemyReminder")
                .Where(r => r.GetComponent<EnemyReminder>().triggered)) Destroy(reminder);
            GetComponent<Rigidbody2D>().position = respawnPoint;
        }
    }
}