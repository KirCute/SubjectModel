using System;
using System.Collections.Generic;
using UnityEngine;

namespace SubjectModel.Scripts.Subject.Chemistry
{
    /**
     * <summary>
     * FlyingDrug落地后，产生的接触会给作战单位附加效果和沾染药品的圆形迷雾，所需附加的脚本。
     * 原则上不需要手动挂载到某一GameObject上。
     * </summary>
     */
    [RequireComponent(typeof(SpriteRenderer))]
    public class BuffInvoker : MonoBehaviour
    {
        //BuffInvoker的半径需在Prefab中修改
        private const float StartRange = 0.18f; //FlyingDrug初始化时距离发起攻击的作战单位中心的距离，必须大于作战单位碰撞箱半径
        private const float SelfAttackRange = 0.25f; //作战单位在此距离内发起攻击时，视为对自身使用，应大于且尽可能接近StartRange
        private const float StartColorAlpha = 0.5f; //BuffInvoker的初始透明度
        private const float DefaultKeepTime = 0.2f; //BuffInvoker的默认存在时间
        private const float MaxDistance = 3f; //FlyingDrug的射程
        private static readonly LayerMask DrugMask = 1 << 3 | 1 << 6 | 1 << 8; //可与FlyingDrug发生碰撞的物体所在层

        private float aliveTime; //总存在时间
        private float remainTime; //已存在时间
        private DrugStack stack; //需要被沾染的药品
        private readonly IList<Collider2D> stained = new List<Collider2D>(); //已经沾染过药品的碰撞箱列表，用于避免二次沾染

        /**
         * <summary>更新GameObject已存在的时间，并以此处理迷雾颜色和销毁。</summary>
         */
        private void Update()
        {
            remainTime += Time.deltaTime; //更新已存在时间
            if (remainTime >= aliveTime) Destroy(gameObject); //若存在时间已满，则销毁GameObject
            else
                gameObject.GetComponent<SpriteRenderer>().color = Utils.Vector3To4(
                    DrugDictionary.GetColor(stack.Ions[0]),
                    Utils.Map(.0f, aliveTime, StartColorAlpha, .0f, remainTime)); //依据已存在时间占总时间的百分比更新透明度
        }

        /**
         * <summary>在作战单位接触时，为作战单位附加效果和沾染药品。</summary>
         */
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer != 6 || !other.gameObject.TryGetComponent<BuffRenderer>(out var br) ||
                stained.Contains(other)) return; //若不是作战单位，作战单位免疫效果和炼金术（无BuffRenderer），已沾染过此药品，则直接返回
            br.Register(stack); //沾染
            stained.Add(other); //标记此碰撞箱避免二次沾染
        }

        /**
         * <summary>
         * BuffInvoker的GameObject的初始化，包括名称，位置和颜色。
         * 原则上只由Invoke方法调用。
         * <param name="drug">要沾染的药品</param>
         * <param name="position">BuffInvoker的出现位置</param>
         * <param name="keepTime">存在总时长，通常情况下不提供，如果要实现MC中滞留药水的效果可以提供一个较大数字</param>
         * </summary>
         */
        private void Initialize(DrugStack drug, Vector2 position, float keepTime = DefaultKeepTime)
        {
            gameObject.name = $"DrugCollider_{drug.Tag}_{position}"; //初始化名称
            transform.position = Utils.Vector2To3(position); //初始化位置
            GetComponent<SpriteRenderer>().color =
                Utils.Vector3To4(DrugDictionary.GetColor(drug.Ions[0]), StartColorAlpha); //初始化颜色
            aliveTime = keepTime;
            stack = drug;
        }

        /**
         * <summary>
         * 炼金术作战单位的投掷方法
         * <param name="drug">要沾染的药品</param>
         * <param name="position">投掷的目标位置（比如玩家的点击位置）</param>
         * <param name="hostPosition">作战单位自身的位置，用于处理射程和自投掷</param>
         * <param name="keepTime">存在总时长，通常情况下不提供，如果要实现MC中滞留药水的效果可以提供一个较大数字</param>
         * </summary>
         */
        public static void InvokeByThrower(DrugStack drug, Vector2 position, Vector2 hostPosition,
            float keepTime = DefaultKeepTime)
        {
            if (Utils.GetMagnitudeSquare2D(position, hostPosition) <= SelfAttackRange * SelfAttackRange) //若发生了自投掷
            {
                Invoke((DrugStack) drug.Fetch(1), position, keepTime); //从作战单位物品栏中直接取出一个玻封药品并发生落地
                return;
            }

            Physics2D.queriesStartInColliders = false;
            var origin =
                Utils.LengthenArrow(hostPosition, position, StartRange); //得到FlyingDrug的出现位置（距离作战单位的距离为StartRange）
            var distanceSquare = Utils.GetMagnitudeSquare2D(position, hostPosition); //得到目标位置距离作战单位的距离的平方
            var distance = distanceSquare < MaxDistance * MaxDistance //得到FlyingDrug（在不考虑碰撞时）的移动距离，用于FlyingDrug计算落地
                ? ((float) Math.Sqrt(distanceSquare) - StartRange) //若目标在射程内，直接用distanceSquare得到
                : (MaxDistance - StartRange); //若目标在射程外，则取最远射程
            var hit = Physics2D.Raycast(origin, position - origin, distance, DrugMask);
            if (hit.collider != null) distance = hit.distance; //（与上一步）用射线计算FlyingDrug碰撞可能，得到最终落地距离
            position = Utils.LengthenArrow(origin, position, distance); //用落地距离换算落地位置

            var flyingDrug =
                (GameObject) Instantiate(
                    Resources.Load("Prefab/FlyingDrug")); //生成FlyingDrug，注意碰撞问题已处理完毕，FlyingDrug主要用于延迟触发沾染和播放动画
            flyingDrug.GetComponent<FlyingDrug>()
                .Initialize((DrugStack) drug.Fetch(1), origin, position, keepTime); //FlyingDrug的初始化
        }

        /**
         * <summary>
         * 药品落地时构造BuffInvoker的方法。
         * 原则上只由FlyingDrug落地时和InvokeByThrower在无FlyingDrug的情况下（比如作战单位对自身原地投掷药品）调用。
         * 如果要实现类似场景固有毒气的效果则不属于上述的情况。
         * <param name="drug">要沾染的药品</param>
         * <param name="position">BuffInvoker的出现位置（FlyingDrug的落地位置或作战单位自身的位置）</param>
         * <param name="keepTime">存在总时长，通常情况下不提供，如果要实现MC中滞留药水的效果可以提供一个较大数字</param>
         * </summary>
         */
        public static void Invoke(DrugStack drug, Vector2 position, float keepTime = DefaultKeepTime)
        {
            var drugCollider = (GameObject) Instantiate(Resources.Load("Prefab/DrugCollider")); //生成BuffInvoker
            drugCollider.GetComponent<BuffInvoker>().Initialize(drug, position, keepTime); //初始化
        }
    }
}