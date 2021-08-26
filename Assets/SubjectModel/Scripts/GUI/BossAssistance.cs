using Bolt;
using SubjectModel.Scripts.Event;
using SubjectModel.Scripts.Subject.Chemistry;
using UnityEngine;

namespace SubjectModel.Scripts.GUI
{
    /**
     * <summary>
     * Boss状态条
     * 需挂载在BossAssistance GUI物品上
     * </summary>
     */
    public class BossAssistance : MonoBehaviour
    {
        private GameObject boss; //Boss, 在BossFightEvent触发时更新，在Boss的EntityDeadEvent触发时变为null

        private void OnEnable()
        {
            EventDispatchers.BossDispatcher.AddEventListener(OnBossFightStart);
        }

        private void OnBossFightStart(GameObject entity)
        {
            boss = entity;
            EventDispatchers.EdeDispatcher.AddEventListener(OnBossFightEnd, boss);
            EventDispatchers.EdeDispatcher.AddEventListener(OnBossFightEnd, GameObject.FindWithTag("Player"));
            boss.GetComponent<HealthBarHelper>().enabled = false; //关闭Boss的一般敌人状态条
            foreach (var bar in GetComponentsInChildren<Transform>())
                switch (bar.name) //通过名称更新BossAssistance子物品的状态
                {
                    case "Health Bar": //血条
                        bar.GetComponent<Bar>().sourceVariables = boss.GetComponent<Variables>();
                        bar.GetComponent<Bar>().enabled = true;
                        break;
                    case "Stain": //沾染信息
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
            foreach (var bar in GetComponentsInChildren<Transform>())
                switch (bar.name) //还原BossAssistance子物品的状态
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
    }
}