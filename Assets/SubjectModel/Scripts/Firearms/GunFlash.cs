using SubjectModel.Scripts.Development;
using UnityEngine;

namespace SubjectModel.Scripts.Firearms
{
    /**
     * <summary>
     * 枪械系统的附属脚本
     * 主要用来处理枪械在射击时的闪光效果和瞄准线的渲染。
     * 此外，它负责处理在射击时寻找被击中的物体，这意味着必须挂载这个脚本才可以使用枪支。
     * 尽管不是所有作战单位都会使用枪，但还是建议所有作战单位都挂载此脚本。
     * </summary>
     */
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(LineRenderer))]
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
        public bool shootAble;
        public Vector2 aimPos = Vector2.zero;

        private void Awake()
        {
            Physics2D.queriesStartInColliders = false;
        }

        private void Update()
        {
            trackTime -= Time.deltaTime;
            if (trackTime > .0f) GetComponent<LineRenderer>().enabled = true;
            else
            {
                trackTime = .0f;
                GetComponent<LineRenderer>().enabled = shootAble && sight;
                if (shootAble && sight && Camera.main != null)
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