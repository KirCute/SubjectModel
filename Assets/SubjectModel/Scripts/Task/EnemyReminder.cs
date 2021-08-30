using Bolt;
using SubjectModel.Scripts.Event;
using SubjectModel.Scripts.System;
using UnityEngine;

namespace SubjectModel.Scripts.Task
{
    /**
     * <summary>
     * 怪物生成器
     * 订阅遭遇事件使怪物显性。
     * 在遭遇且有Boss时触发BossFight事件。
     * 在怪物全部被击杀时触发成功事件。
     * 注意：只有挂载了StateMachine的物体会被识别为怪物。
     * </summary>
     */
    public class EnemyReminder : MissionObject
    {
        public GameObject boss; //若为子物体之一，指定其为Boss，否则认为此次遭遇无Boss
        public bool cameraLock; //是否锁定相机
        
        private void Update()
        {
            if (GetComponentsInChildren<StateMachine>().Length > 0) return;
            EventDispatchers.EeDispatcher.DispatchEvent(completeIndex); //触发成功事件
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