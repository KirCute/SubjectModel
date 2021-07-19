using Bolt;
using Cinemachine;
using Ludiq;
using UnityEngine;
using UnityEngine.Serialization;

namespace SubjectModel
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

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<PolygonCollider2D>().enabled = true;
            GameObject.FindWithTag("Cinemachine").GetComponent<CinemachineVirtualCamera>().Follow = transform;

            void Raf()
            {
                GetComponent<PolygonCollider2D>().enabled = false;
                GameObject.FindWithTag("Cinemachine").GetComponent<CinemachineVirtualCamera>().Follow =
                    other.gameObject.transform;
            }

            var boss = (GameObject) GameObject.Instantiate(Resources.Load(bossResource));
            boss.name = bossName;
            boss.tag = "Boss";
            boss.GetComponent<Rigidbody2D>().position = bossSpawnPosition;
            boss.GetComponent<Variables>().declarations.GetDeclaration("MaxHealth").value = bossHealth;
            boss.GetComponent<Variables>().declarations.GetDeclaration("Health").value = bossHealth;
            boss.GetComponent<Variables>().declarations.GetDeclaration("Defence").value = bossDefence;
            boss.GetComponent<Variables>().declarations.GetDeclaration("Speed").value = bossSpeed;
            if (bossAi != null)
            {
                boss.GetComponent<StateMachine>().nest.source = GraphSource.Macro;
                boss.GetComponent<StateMachine>().nest.macro = bossAi;
            }

            GameObject.FindWithTag("BossAssistance").GetComponent<BossAssistance>().StartBossFight(boss, Raf);
        }
    }
}