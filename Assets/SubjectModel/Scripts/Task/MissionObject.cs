using SubjectModel.Scripts.Event;
using UnityEngine;

namespace SubjectModel.Scripts.Task
{
    /**
     * <summary>
     * 任务物体基类
     * 任务情形分为三种：遭遇，失败，成功
     * 遭遇：由MissionTrigger触发，判据为玩家到达指定位置。
     * 失败：由MissionObject触发，判据为玩家死亡。
     * 成功：由EnemyReminder触发，判据为怪物被全部击杀
     * </summary>
     */
    public abstract class MissionObject : MonoBehaviour
    {
        public int missionIndex; //遭遇任务代号
        public int completeIndex; //成功任务代号
        private bool encounter; //是否正在遭遇，用于防止监听到错误的PlayerDead事件

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