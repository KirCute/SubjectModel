using UnityEngine;

namespace SubjectModel.Scripts.Firearms
{
    public class GunFlash : MonoBehaviour
    {
        private const float StartRange = 0.18f;
        private static readonly LayerMask ShootMask = 1 << 3 | 1 << 6 | 1 << 8;

        private static readonly Pair<Color> SightLineColor = new Pair<Color>()
            {First = new Vector4(.0f, .0f, 1f, .0f), Second = new Vector4(.0f, .0f, 1f, .5f)};

        private static readonly Pair<Color> HitColor = new Pair<Color>()
            {First = new Vector4(1f, 1f, 0f, .0f), Second = new Vector4(1f, 1f, 0f, 1f)};

        private static readonly Pair<Color> MissColor = new Pair<Color>()
            {First = new Vector4(1f, 0f, 0f, .0f), Second = new Vector4(1f, 0f, 0f, 1f)};

        private float trackTime;
        public float distance;
        public bool sight;
        public Vector2 aimPos;

        private void Awake()
        {
            Physics2D.queriesStartInColliders = false;
            sight = true;
            aimPos = Vector2.zero;
        }

        private void Update()
        {
            trackTime -= Time.deltaTime;
            if (trackTime > .0f) GetComponent<LineRenderer>().enabled = true;
            else
            {
                trackTime = .0f;
                GetComponent<LineRenderer>().enabled = sight;
                if (sight && Camera.main != null)
                    DrawLine(GetComponent<Rigidbody2D>().position, aimPos, SightLineColor);
            }
        }

        public Collider2D Shoot(Vector2 shooterPosition, Vector2 aim)
        {
            var hit = DrawLine(shooterPosition, aim, HitColor, MissColor);
            trackTime = .1f;
            return hit.collider == null || hit.collider.gameObject.layer != 6 ? null : hit.collider;
        }

        private RaycastHit2D DrawLine(Vector2 from, Vector2 to, Pair<Color> success, Pair<Color> fault)
        {
            var origin = Utils.LengthenArrow(from, to, StartRange);
            var hit = Physics2D.Raycast(origin, to - origin, distance, ShootMask);
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