using UnityEngine;

namespace SubjectModel
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class FlyingDrug : MonoBehaviour
    {
        private const float FlyingVelocity = 40.0f;
        private DrugStack drug;
        private Vector2 origin;
        private Vector2 target;
        private float distanceSquare;
        private float keepTime;

        public void Initialize(DrugStack drug, Vector2 origin, Vector2 target, float keepTime)
        {
            gameObject.name = "FlyingDrug_" + drug.Tag;
            this.drug = drug;
            this.origin = origin;
            GetComponent<Rigidbody2D>().position = origin;
            GetComponent<Rigidbody2D>().velocity =
                Utils.LengthenArrow(origin, target, FlyingVelocity);
            this.target = target;
            this.distanceSquare = Utils.GetMagnitudeSquare2D(origin, target);
            this.keepTime = keepTime;
        }

        private void Update()
        {
            var position = GetComponent<Rigidbody2D>().position;
            if (Utils.GetMagnitudeSquare2D(position, origin) >= distanceSquare) Invoke();
        }

        private void Invoke()
        {
            BuffInvoker.Invoke(drug, target, keepTime);
            Destroy(gameObject);
        }
    }
}