using System;
using UnityEngine;

namespace SubjectModel
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BuffInvoker : MonoBehaviour
    {
        public const float StartRange = 0.18f;
        public const float SelfAttackRange = 0.25f;
        public const float StartColorAlpha = 0.5f;
        public const float DefaultKeepTime = 0.5f;
        public const float MaxDistance = 5f;
        private static readonly LayerMask DrugMask = 1 << 3 | 1 << 6;

        private float aliveTime;
        private DrugStack stack;
        private float remainTime;

        private void Start()
        {
            remainTime = .0f;
        }

        private void Update()
        {
            remainTime += Time.deltaTime;
            if (remainTime >= aliveTime) Destroy(gameObject);
            else
                gameObject.GetComponent<SpriteRenderer>().color = Utils.Vector3To4(DrugDictionary.GetColor(stack),
                    Utils.Map(.0f, aliveTime, StartColorAlpha, .0f, remainTime));
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer == 6 && other.gameObject.TryGetComponent<BuffRenderer>(out var br))
                br.Apply((IBuff) Activator.CreateInstance(DrugDictionary.GetTypeOfBuff(stack.Type), stack.Param));
        }

        private void Initialize(DrugStack buff, Vector2 position, float keepTime = DefaultKeepTime)
        {
            gameObject.name = "DrugCollider_" + buff.Type + "_" + position;
            transform.position = Utils.Vector2To3(position);
            GetComponent<SpriteRenderer>().color =
                Utils.Vector3To4(DrugDictionary.GetColor(buff), StartColorAlpha);
            aliveTime = keepTime;
            stack = buff;
        }

        public static void InvokeByThrower(DrugStack buff, Vector2 position, Vector2 hostPosition,
            float keepTime = DefaultKeepTime)
        {
            if (Utils.GetMagnitudeSquare2D(position, hostPosition) <= SelfAttackRange * SelfAttackRange)
            {
                Invoke(buff, position, keepTime);
                return;
            }

            Physics2D.queriesStartInColliders = false;
            var origin = Utils.LengthenArrow(hostPosition, position, StartRange);
            var distanceSquare = Utils.GetMagnitudeSquare2D(position, hostPosition);
            var distance = distanceSquare < MaxDistance * MaxDistance
                ? ((float) Math.Sqrt(distanceSquare) - StartRange)
                : (MaxDistance - StartRange);
            var hit = Physics2D.Raycast(origin, position - origin, distance, DrugMask);
            if (hit.collider != null) distance = hit.distance;
            position = Utils.LengthenArrow(origin, position, distance);

            var flyingDrug = (GameObject) GameObject.Instantiate(Resources.Load("Prefab/FlyingDrug"));
            flyingDrug.GetComponent<FlyingDrug>().Initialize(buff, origin, position, keepTime);
        }

        public static void Invoke(DrugStack buff, Vector2 position, float keepTime = DefaultKeepTime)
        {
            var drugCollider = (GameObject) GameObject.Instantiate(Resources.Load("Prefab/DrugCollider"));
            drugCollider.GetComponent<BuffInvoker>().Initialize(buff, position, keepTime);
        }
    }
}