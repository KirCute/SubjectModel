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

        private static readonly Pair<Color> TelescopeColor = new Pair<Color>()
            {First = new Vector4(.0f, .0f, 1f, .0f), Second = new Vector4(.0f, .0f, 1f, .5f)};

        private static readonly Pair<Color> HitColor = new Pair<Color>()
            {First = new Vector4(1f, 1f, 0f, .0f), Second = new Vector4(1f, 1f, 0f, 1f)};

        private static readonly Pair<Color> MissColor = new Pair<Color>()
            {First = new Vector4(1f, 0f, 0f, .0f), Second = new Vector4(1f, 0f, 0f, 1f)};

        public float loadingTime;
        public float distance;
        public float depth;
        public float damage;
        public int bulletRemain;
        public float deviation;
        public float maxRange;
        public bool telescope;
        private float trackTime;

        private void Start()
        {
            Physics2D.queriesStartInColliders = false;
        }

        private void Update()
        {
            var loading = (float) GetComponent<Variables>().declarations.GetDeclaration("Loading").value;
            loading -= Time.deltaTime;
            if (loading < .0f) loading = .0f;
            GetComponent<Variables>().declarations.GetDeclaration("Loading").value = loading;

            trackTime -= Time.deltaTime;
            if (trackTime > .0f) GetComponent<LineRenderer>().enabled = true;
            else
            {
                trackTime = .0f;
                GetComponent<LineRenderer>().enabled = telescope && loading <= .0f;
                if (telescope)
                    DrawLine(GetComponent<Rigidbody2D>().position,
                        Utils.Vector3To2(Camera.main.ScreenToWorldPoint(Input.mousePosition)), TelescopeColor);
            }
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
            var hit = DrawLine(shooterPosition, aim, HitColor, MissColor);
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

        private RaycastHit2D DrawLine(Vector2 from, Vector2 to, Pair<Color> success, Pair<Color> fault)
        {
            var origin = Utils.LengthenArrow(from, to, StartRange);
            var hit = Physics2D.Raycast(origin, to - origin, this.distance, ShootMask);
            var dist = hit.collider == null ? distance : hit.distance;
            to = Utils.LengthenArrow(origin, to, dist);
            var track = GetComponent<LineRenderer>();
            track.SetPosition(0, origin);
            track.SetPosition(1, to);
            var successful = hit.collider != null && hit.collider.gameObject.layer == 6;
            track.startColor = successful ? success.First : fault.First;
            track.endColor = successful ? success.Second : fault.Second;
            return hit;
        }

        private RaycastHit2D DrawLine(Vector2 from, Vector2 to, Pair<Color> color)
        {
            return DrawLine(from, to, color, color);
        }
    }
}