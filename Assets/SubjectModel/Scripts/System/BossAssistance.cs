using SubjectModel.Scripts.GUI;
using UnityEngine;

namespace SubjectModel.Scripts.System
{
    public delegate void RecoverAfterFight();

    [RequireComponent(typeof(Bar))]
    public class BossAssistance : MonoBehaviour
    {
        private bool fighting;
        private GameObject boss;
        private RecoverAfterFight raf;

        private void Reset()
        {
            var bar = GetComponent<Bar>();
            bar.source = "Health";
            bar.sourceEnd = "MaxHealth";
            bar.targetEnd = 1800f;
            bar.enabled = false;
        }

        private void Update()
        {
            if (boss == null && fighting) BossDead();
        }

        public void StartBossFight(GameObject theBoss, RecoverAfterFight itsRaf)
        {
            boss = theBoss;
            boss.GetComponent<HealthBarHelper>().enabled = false;
            GetComponent<Bar>().sourceObject = theBoss;
            GetComponent<Bar>().enabled = true;
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

            GetComponent<Bar>().sourceObject = null;
            GetComponent<Bar>().enabled = false;
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