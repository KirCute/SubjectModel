using Bolt;
using SubjectModel.Scripts.Chemistry;
using SubjectModel.Scripts.Event;
using SubjectModel.Scripts.GUI;
using UnityEngine;

namespace SubjectModel.Scripts.System
{
    public class BossAssistance : MonoBehaviour
    {
        private GameObject boss;

        private void OnEnable()
        {
            EventDispatchers.BossDispatcher.AddEventListener(OnBossFightStart);
        }

        private void OnBossFightStart(GameObject entity)
        {
            boss = entity;
            EventDispatchers.EdeDispatcher.AddEventListener(OnBossFightEnd, boss);
            EventDispatchers.EdeDispatcher.AddEventListener(OnBossFightEnd, GameObject.FindWithTag("Player"));
            boss.GetComponent<HealthBarHelper>().enabled = false;
            foreach (var bar in GetComponentsInChildren<Transform>())
                switch (bar.name)
                {
                    case "Health Bar":
                        bar.GetComponent<Bar>().sourceVariables = boss.GetComponent<Variables>();
                        bar.GetComponent<Bar>().enabled = true;
                        break;
                    case "Stain":
                        if (boss.TryGetComponent<BuffRenderer>(out var br))
                        {
                            bar.GetComponent<StainGui>().buffRenderer = br;
                            bar.GetComponent<StainGui>().enabled = true;
                        }

                        break;
                }
        }

        private void OnBossFightEnd()
        {
            EventDispatchers.EdeDispatcher.RemoveEventListener(OnBossFightEnd, boss);
            EventDispatchers.EdeDispatcher.RemoveEventListener(OnBossFightEnd, GameObject.FindWithTag("Player"));
            boss.GetComponent<HealthBarHelper>().enabled = false;
            foreach (var bar in GetComponentsInChildren<Transform>())
                switch (bar.name)
                {
                    case "Health Bar":
                        bar.GetComponent<Bar>().sourceVariables = null;
                        bar.GetComponent<Bar>().enabled = false;
                        break;
                    case "Stain":
                        bar.GetComponent<StainGui>().buffRenderer = null;
                        bar.GetComponent<StainGui>().enabled = false;
                        break;
                }
        }

        private void OnDisable()
        {
            EventDispatchers.BossDispatcher.RemoveEventListener(OnBossFightStart);
        }

        /*
        private void Update()
        {
            if (boss == null && fighting) BossDead();
        }

        public void StartBossFight(GameObject theBoss, int next)
        {
            nextMission = next;
            boss = theBoss;
        }

        public void BossDead()
        {
            EventDispatchers.EeDispatcher.DispatchEvent(nextMission);

            fighting = false;
        }
        
        public void InterruptFighting()
        {
            if (!fighting) return;
            fighting = false;
            Destroy(boss);
            BossDead();
        }
        */
    }
}