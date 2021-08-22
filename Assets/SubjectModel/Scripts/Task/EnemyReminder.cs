using Bolt;
using SubjectModel.Scripts.Event;
using SubjectModel.Scripts.System;
using UnityEngine;

namespace SubjectModel.Scripts.Task
{
    public class EnemyReminder : MissionObject
    {
        public GameObject boss;
        public bool cameraLock;
        
        private void Update()
        {
            if (GetComponentsInChildren<StateMachine>().Length > 0) return;
            EventDispatchers.EeDispatcher.DispatchEvent(completeIndex);
        }

        protected override void Encounter()
        {
            foreach (var enemy in GetComponentsInChildren<StateMachine>())
            {
                enemy.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                enemy.enabled = true;
                if (enemy.gameObject == boss) EventDispatchers.BossDispatcher.DispatchEvent(boss);
            }
            if (cameraLock) EventDispatchers.CteDispatcher.DispatchEvent(CameraManager.LayerScroll, transform);
        }

        protected override void Failed()
        {
            foreach (var enemy in GetComponentsInChildren<StateMachine>()) enemy.enabled = false;
            if (cameraLock) EventDispatchers.CteDispatcher.DispatchEvent(CameraManager.LayerScroll, null);
        }

        protected override void Defeated()
        {
            if (cameraLock) EventDispatchers.CteDispatcher.DispatchEvent(CameraManager.LayerScroll, null);
            // TODO: 生成掉落物
        }
    }
}