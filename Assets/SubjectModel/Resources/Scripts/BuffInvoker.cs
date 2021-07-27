using System;
using System.Collections.Generic;
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
        private IList<Collider2D> stained;

        private void Awake()
        {
            remainTime = .0f;
            stained = new List<Collider2D>();
        }

        private void Update()
        {
            remainTime += Time.deltaTime;
            if (remainTime >= aliveTime) Destroy(gameObject);
            else
                gameObject.GetComponent<SpriteRenderer>().color = Utils.Vector3To4(
                    DrugDictionary.GetColor(stack.Ions[0]),
                    Utils.Map(.0f, aliveTime, StartColorAlpha, .0f, remainTime));
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer != 6 || !other.gameObject.TryGetComponent<BuffRenderer>(out var br) ||
                stained.Contains(other)) return;
            br.Register(stack);
            stained.Add(other);
        }

        private void Initialize(DrugStack drug, Vector2 position, float keepTime = DefaultKeepTime)
        {
            gameObject.name = "DrugCollider_" + drug.Tag + "_" + position;
            transform.position = Utils.Vector2To3(position);
            GetComponent<SpriteRenderer>().color =
                Utils.Vector3To4(DrugDictionary.GetColor(drug.Ions[0]), StartColorAlpha);
            aliveTime = keepTime;
            stack = drug;
        }

        public static void InvokeByThrower(DrugStack drug, Vector2 position, Vector2 hostPosition,
            float keepTime = DefaultKeepTime)
        {
            if (Utils.GetMagnitudeSquare2D(position, hostPosition) <= SelfAttackRange * SelfAttackRange)
            {
                Invoke((DrugStack) drug.Fetch(), position, keepTime);
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
            flyingDrug.GetComponent<FlyingDrug>().Initialize((DrugStack) drug.Fetch(), origin, position, keepTime);
        }

        public static void Invoke(DrugStack drug, Vector2 position, float keepTime = DefaultKeepTime)
        {
            var drugCollider = (GameObject) GameObject.Instantiate(Resources.Load("Prefab/DrugCollider"));
            drugCollider.GetComponent<BuffInvoker>().Initialize(drug, position, keepTime);
        }
    }
}