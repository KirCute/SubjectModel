using Bolt;
using UnityEngine;

namespace SubjectModel
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
                var health = (float) boss.GetComponent<Variables>().declarations.Get("Health");
                GetComponent<RectTransform>().sizeDelta = new Vector2(
                    Utils.Map(.0f, maxHealth, .0f, 900f, health), 7.5f);
            }
            else if (fighting) BossDead();
        }

        public void StartBossFight(GameObject boss, RecoverAfterFight raf)
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(900f, 7.5f);
            this.boss = boss;
            maxHealth = (float) boss.GetComponent<Variables>().declarations.Get("Health");
            this.raf += raf;
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