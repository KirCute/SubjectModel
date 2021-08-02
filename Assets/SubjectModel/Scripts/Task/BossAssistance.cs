using Bolt;
using UnityEngine;

namespace SubjectModel.Scripts.Task
{
    public delegate void RecoverAfterFight();

    [RequireComponent(typeof(RectTransform))]
    public class BossAssistance : MonoBehaviour
    {
        public GameObject boss;
        public bool fighting;
        private RecoverAfterFight raf;
        private float maxHealth;

        private void Update()
        {
            if (boss != null)
            {
                var health = boss.GetComponent<Variables>().declarations.Get<float>("Health");
                GetComponent<RectTransform>().sizeDelta = new Vector2(
                    Utils.Map(.0f, maxHealth, .0f, 1800f, health), 15f);
            }
            else if (fighting) BossDead();
        }

        public void StartBossFight(GameObject theBoss, RecoverAfterFight itsRaf)
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(900f, 7.5f);
            boss = theBoss;
            maxHealth = boss.GetComponent<Variables>().declarations.Get<float>("Health");
            raf += itsRaf;
            fighting = true;
        }

        public void BossDead()
        {
            boss = null;
            maxHealth = .0f;
            if (raf != null)
            {
                raf();
                raf = null;
            }

            GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 7.5f);
            fighting = false;
        }
    }
}