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
        private float remainTime;

        public void Initialize(DrugStack drugStack, Vector2 originPos, Vector2 targetPos, float keepTime)
        {
            gameObject.name = "FlyingDrug_" + drugStack.Tag;
            this.drug = drugStack;
            this.origin = originPos;
            GetComponent<Rigidbody2D>().position = originPos;
            GetComponent<Rigidbody2D>().velocity =
                Utils.LengthenArrow(originPos, targetPos, FlyingVelocity);
            this.target = targetPos;
            this.distanceSquare = Utils.GetMagnitudeSquare2D(originPos, targetPos);
            this.remainTime = keepTime;
        }

        private void Update()
        {
            var position = GetComponent<Rigidbody2D>().position;
            if (Utils.GetMagnitudeSquare2D(position, origin) >= distanceSquare) Invoke();
        }

        private void Invoke()
        {
            BuffInvoker.Invoke(drug, target, remainTime);
            Destroy(gameObject);
        }
    }
}