using Bolt;
using Cinemachine;
using Ludiq;
using SubjectModel.Scripts.System;
using UnityEngine;

namespace SubjectModel.Scripts.Task
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(PolygonCollider2D))]
    public class BossSpawner : MonoBehaviour
    {
        public string bossName;
        public string bossResource;
        public float bossHealth;
        public float bossDefence;
        public float bossSpeed;
        public Vector2 bossSpawnPosition;
        public StateMacro bossAi;
        public bool triggered;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (triggered || !other.gameObject.CompareTag("Player")) return;
            triggered = true;
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<PolygonCollider2D>().enabled = true;
            GameObject.FindWithTag("Cinemachine").GetComponent<CinemachineVirtualCamera>().Follow = transform;

            void Raf()
            {
                GetComponent<PolygonCollider2D>().enabled = false;
                GameObject.FindWithTag("Cinemachine").GetComponent<CinemachineVirtualCamera>().Follow =
                    other.gameObject.transform;
            }

            var boss = (GameObject) Instantiate(Resources.Load(bossResource), transform);
            boss.GetComponent<Rigidbody2D>().position = bossSpawnPosition;
            boss.name = bossName;
            boss.tag = "Boss";
            var variables = boss.GetComponent<Variables>().declarations;
            variables.Set("MaxHealth", bossHealth);
            variables.Set("Health", bossHealth);
            variables.Set("Defence", bossDefence);
            variables.Set("Speed", bossSpeed);
            if (bossAi != null)
            {
                var machine = boss.GetComponent<StateMachine>();
                machine.nest.source = GraphSource.Macro;
                machine.nest.macro = bossAi;
                machine.enabled = true;
            }

            GameObject.FindWithTag("BossAssistance").GetComponent<BossAssistance>().StartBossFight(boss, Raf);
        }
    }
}