using UnityEngine;

namespace SubjectModel
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class FlyingDrug : MonoBehaviour
    {
        private const float FlyingVelocity = 40.0f;
        private DrugStack buff;
        private Vector2 origin;
        private Vector2 target;
        private float distanceSquare;
        private float keepTime;

        public void Initialize(DrugStack buff, Vector2 origin, Vector2 target, float keepTime)
        {
            gameObject.name = "FlyingDrug_" + buff.Type;
            this.buff = buff;
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
            BuffInvoker.Invoke(buff, target, keepTime);
            Destroy(gameObject);
        }
    }
}