using Bolt;
using Cinemachine;
using UnityEngine;

namespace SubjectModel.Scripts.Task
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class EnemyReminder : MonoBehaviour
    {
        public bool cameraLock;
        public bool triggered;

        private void Update()
        {
            if (!triggered || GetComponentsInChildren<Transform>().Length > 1) return;
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (triggered || !other.gameObject.CompareTag("Player")) return;
            triggered = true;
            GetComponent<BoxCollider2D>().enabled = false;
            if (TryGetComponent<PolygonCollider2D>(out var wall)) wall.enabled = true;
            foreach (var enemy in GetComponentsInChildren<StateMachine>()) enemy.enabled = true;
            if (cameraLock)
                GameObject.FindWithTag("Cinemachine").GetComponent<CinemachineVirtualCamera>().Follow = transform;
        }

        private void OnDestroy()
        {
            if (cameraLock && GameObject.FindWithTag("Player") != null && GameObject.FindWithTag("Cinemachine") != null)
                GameObject.FindWithTag("Cinemachine").GetComponent<CinemachineVirtualCamera>().Follow =
                    GameObject.FindWithTag("Player").transform;
        }
    }
}