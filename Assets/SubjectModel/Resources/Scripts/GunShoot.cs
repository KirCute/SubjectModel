using Bolt;
using UnityEngine;

namespace SubjectModel
{
    [RequireComponent(typeof(Variables))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class GunShoot : MonoBehaviour
    {
        private const float StartRange = 0.18f;
        private static readonly LayerMask ShootMask = 1 << 3 | 1 << 6;

        public float loadingTime;
        public float distance;
        public float depth;
        public float damage;
        public int bulletRemain;
        public float deviation;
        public float maxRange;
        private float trackTime;

        private void Update()
        {
            var loading = (float) GetComponent<Variables>().declarations.GetDeclaration("Loading").value;
            loading -= Time.deltaTime;
            if (loading < .0f) loading = .0f;
            GetComponent<Variables>().declarations.GetDeclaration("Loading").value = loading;

            trackTime -= Time.deltaTime;
            if (trackTime < .0f) trackTime = .0f;
            GetComponent<LineRenderer>().enabled = trackTime > .0f;
        }

        public void Shoot(Vector2 aim)
        {
            if (bulletRemain <= 0) return;
            if ((float) GetComponent<Variables>().declarations.GetDeclaration("Loading").value > .0f) return;
            aim.x = Utils.GenerateGaussian(aim.x, deviation * distance, maxRange);
            aim.y = Utils.GenerateGaussian(aim.y, deviation * distance, maxRange);
            var shooterPosition = GetComponent<Rigidbody2D>().position;
            if (shooterPosition == aim) return;

            bulletRemain--;
            GetComponent<Variables>().declarations.GetDeclaration("Loading").value = loadingTime;
            Physics2D.queriesStartInColliders = false;
            var origin = Utils.LengthenArrow(shooterPosition, aim, StartRange);
            var hit = Physics2D.Raycast(origin, aim - origin, this.distance, ShootMask);

            var dist = hit.collider == null ? distance : hit.distance;
            aim = Utils.LengthenArrow(origin, aim, dist);
            var track = GetComponent<LineRenderer>();
            track.SetPosition(1, origin);
            track.SetPosition(0, aim);
            trackTime = .1f;

            if (hit.collider == null || hit.collider.gameObject.layer != 6) return;
            var variables = hit.collider.GetComponent<Variables>();
            var defence = variables.declarations.IsDefined("Defence")
                ? (float) variables.declarations.GetDeclaration("Defence").value
                : .0f;
            var health = (float) variables.declarations.GetDeclaration("Health").value;
            variables.declarations.GetDeclaration("Health").value =
                health - (defence > depth ? Utils.Map(.0f, defence, .0f, damage, depth) : damage);
        }
    }
}