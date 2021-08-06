using System.Linq;
using Bolt;
using SubjectModel.Scripts.Task;
using UnityEngine;

namespace SubjectModel.Scripts.System
{
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