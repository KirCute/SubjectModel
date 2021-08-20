using Bolt;
using SubjectModel.Scripts.Chemistry;
using SubjectModel.Scripts.GUI;
using UnityEngine;

namespace SubjectModel.Scripts.System
{
    public delegate void RecoverAfterFight();

    public class BossAssistance : MonoBehaviour
    {
        private bool fighting;
        private GameObject boss;
        private RecoverAfterFight raf;

        /*
        private void Reset()
        {
            var bar = GetComponent<Bar>();
            bar.source = "Health";
            bar.sourceEnd = "MaxHealth";
            bar.targetEnd = 1800f;
            bar.enabled = false;
        }
        */

        private void Update()
        {
            if (boss == null && fighting) BossDead();
        }

        public void StartBossFight(GameObject theBoss, RecoverAfterFight itsRaf)
        {
            boss = theBoss;
            boss.GetComponent<HealthBarHelper>().enabled = false;

            foreach (var bar in GetComponentsInChildren<Transform>())
                switch (bar.name)
                {
                    case "Health Bar":
                        bar.GetComponent<Bar>().sourceVariables = theBoss.GetComponent<Variables>();
                        bar.GetComponent<Bar>().enabled = true;
                        break;
                    case "Stain":
                        if (theBoss.TryGetComponent<BuffRenderer>(out var br))
                        {
                            bar.GetComponent<StainGui>().buffRenderer = br;
                            bar.GetComponent<StainGui>().enabled = true;
                        }

                        break;
                }

            raf += itsRaf;
            fighting = true;
        }

        public void BossDead()
        {
            if (raf != null)
            {
                raf();
                raf = null;
            }

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

            fighting = false;
        }

        public void InterruptFighting()
        {
            if (!fighting) return;
            fighting = false;
            Destroy(boss);
            BossDead();
        }
    }
}