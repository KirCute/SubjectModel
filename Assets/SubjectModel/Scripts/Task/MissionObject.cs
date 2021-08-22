using SubjectModel.Scripts.Event;
using UnityEngine;

namespace SubjectModel.Scripts.Task
{
    public abstract class MissionObject : MonoBehaviour
    {
        public int missionIndex;
        public int completeIndex;
        private bool encounter;

        protected abstract void Encounter();
        protected abstract void Failed();
        protected abstract void Defeated();
        
        private void OnEnable()
        {
            EventDispatchers.EeDispatcher.AddEventListener(OnEncounter, missionIndex);
            EventDispatchers.EeDispatcher.AddEventListener(OnDefeated, completeIndex);
            EventDispatchers.EdeDispatcher.AddEventListener(OnPlayerDead, GameObject.FindWithTag("Player"));
        }

        private void OnEncounter()
        {
            encounter = true;
            EventDispatchers.EeDispatcher.RemoveEventListener(OnEncounter, missionIndex);
            Encounter();
        }

        private void OnDefeated()
        {
            Defeated();
            enabled = false;
        }

        private void OnPlayerDead()
        {
            if (!encounter) return;
            Failed();
            EventDispatchers.EeDispatcher.AddEventListener(OnEncounter, missionIndex);
            encounter = false;
        }

        private void OnDisable()
        {
            EventDispatchers.EeDispatcher.RemoveEventListener(OnEncounter, missionIndex);
            EventDispatchers.EeDispatcher.RemoveEventListener(OnDefeated, completeIndex);
            EventDispatchers.EdeDispatcher.RemoveEventListener(OnPlayerDead, GameObject.FindWithTag("Player"));
        }
    }
}