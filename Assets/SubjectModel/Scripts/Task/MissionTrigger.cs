using SubjectModel.Scripts.Event;
using UnityEngine;

namespace SubjectModel.Scripts.Task
{
    [RequireComponent(typeof(Collider2D))]
    public class MissionTrigger : MonoBehaviour
    {
        public int missionIndex;
        public int completeIndex;
        private bool encounter;

        private void OnEnable()
        {
            EventDispatchers.EeDispatcher.AddEventListener(OnDefeated, completeIndex);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (encounter || !other.gameObject.CompareTag("Player")) return;
            encounter = true;
            EventDispatchers.EdeDispatcher.AddEventListener(OnFailed, other.gameObject);
            EventDispatchers.EeDispatcher.DispatchEvent(missionIndex);
        }

        private void OnFailed()
        {
            encounter = false;
            EventDispatchers.EdeDispatcher.RemoveEventListener(OnFailed, GameObject.FindWithTag("Player"));
        }

        private void OnDefeated()
        {
            if (encounter)
                EventDispatchers.EdeDispatcher.RemoveEventListener(OnFailed, GameObject.FindWithTag("Player"));
            enabled = false;
            Destroy(gameObject);
        }

        private void OnDisable()
        {
            EventDispatchers.EeDispatcher.RemoveEventListener(OnDefeated, completeIndex);
        }
    }
}