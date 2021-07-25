using System;
using Bolt;
using UnityEngine;

namespace SubjectModel
{
    [RequireComponent(typeof(Variables))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(LineRenderer))]
    public class GunShoot : MonoBehaviour
    {
        private const float MaxDistance = 15f;
        private const float StartRange = 0.18f;
        private static readonly LayerMask ShootMask = 1 << 3 | 1 << 6;

        private static readonly Pair<Color> TelescopeColor = new Pair<Color>()
            {First = new Vector4(.0f, .0f, 1f, .0f), Second = new Vector4(.0f, .0f, 1f, .5f)};

        private static readonly Pair<Color> HitColor = new Pair<Color>()
            {First = new Vector4(1f, 1f, 0f, .0f), Second = new Vector4(1f, 1f, 0f, 1f)};

        private static readonly Pair<Color> MissColor = new Pair<Color>()
            {First = new Vector4(1f, 0f, 0f, .0f), Second = new Vector4(1f, 0f, 0f, 1f)};
        
        public int bulletContains = 20;
        public int bulletRemain;
        public bool telescope;
        public bool switchingMagazine;
        public Firearm firearm;
        private float trackTime;

        private void Awake()
        {
            firearm = GunDictionary.GetDefaultInventory()[0];
        }

        private void Start()
        {
            Physics2D.queriesStartInColliders = false;
            switchingMagazine = false;
        }

        private void Update()
        {
            var loading = GetComponent<Variables>().declarations.Get<float>("Loading");
            loading -= Time.deltaTime;
            if (loading <= .0f)
            {
                loading = .0f;
                if (switchingMagazine)
                {
                    switchingMagazine = false;
                    GetComponent<Variables>().declarations.Set("Standonly", 
                        GetComponent<Variables>().declarations.Get<int>("Standonly") - 1);
                    bulletRemain = bulletContains;
                }
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                switchingMagazine = true;
                loading = firearm.Data[FirearmComponent.Reload];
                    GetComponent<Variables>().declarations.Set("Standonly", 
                        GetComponent<Variables>().declarations.Get<int>("Standonly") + 1);
            }
            GetComponent<Variables>().declarations.Set("Loading", loading);

            trackTime -= Time.deltaTime;
            if (trackTime > .0f) GetComponent<LineRenderer>().enabled = true;
            else
            {
                trackTime = .0f;
                GetComponent<LineRenderer>().enabled = telescope && loading <= .0f;
                if (telescope && Camera.main != null)
                    DrawLine(GetComponent<Rigidbody2D>().position,
                        Utils.Vector3To2(Camera.main.ScreenToWorldPoint(Input.mousePosition)), TelescopeColor);
            }
        }

        public void Shoot(Vector2 aim)
        {
            if (bulletRemain <= 0) return;
            if (GetComponent<Variables>().declarations.Get<float>("Loading") > .0f) return;
            aim.x = Utils.GenerateGaussian(aim.x, firearm.Data[FirearmComponent.Deviation] * MaxDistance, firearm.Data[FirearmComponent.MaxRange]);
            aim.y = Utils.GenerateGaussian(aim.y, firearm.Data[FirearmComponent.Deviation] * MaxDistance, firearm.Data[FirearmComponent.MaxRange]);
            var shooterPosition = GetComponent<Rigidbody2D>().position;
            if (shooterPosition == aim) return;

            bulletRemain--;
            if (bulletRemain != 0) GetComponent<Variables>().declarations.Set("Loading", firearm.Data[FirearmComponent.Loading]);
            var hit = DrawLine(shooterPosition, aim, HitColor, MissColor);
            trackTime = .1f;

            if (hit.collider == null || hit.collider.gameObject.layer != 6) return;
            var variables = hit.collider.GetComponent<Variables>();
            var defence = variables.declarations.IsDefined("Defence")
                ? variables.declarations.Get<float>("Defence")
                : .0f;
            var health = variables.declarations.Get<float>("Health");
            variables.declarations.Set("Health", 
                health - (defence > firearm.Data[FirearmComponent.Depth] ? Utils.Map(.0f, defence, .0f, firearm.Data[FirearmComponent.Damage], firearm.Data[FirearmComponent.Depth]) : firearm.Data[FirearmComponent.Damage]));
        }

        private RaycastHit2D DrawLine(Vector2 from, Vector2 to, Pair<Color> success, Pair<Color> fault)
        {
            var origin = Utils.LengthenArrow(from, to, StartRange);
            var hit = Physics2D.Raycast(origin, to - origin, MaxDistance, ShootMask);
            var dist = hit.collider == null ? MaxDistance : hit.distance;
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