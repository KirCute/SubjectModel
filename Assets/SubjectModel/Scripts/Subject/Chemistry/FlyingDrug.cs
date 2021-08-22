using UnityEngine;

namespace SubjectModel.Scripts.Subject.Chemistry
{
    /**
     * <summary>
     * 在空中抛掷的玻封药品所附加的药品
     * FlyingDrug物体不处理在空中遇到碰撞的情况，只处理延迟落地和动画
     * 有关碰撞处理的内容见<code>BuffInvoker.InvokeByThrower(DrugStack, Vector2, Vector2, float)</code>
     * </summary>
     */
    [RequireComponent(typeof(Rigidbody2D))]
    public class FlyingDrug : MonoBehaviour
    {
        private const float FlyingVelocity = 40.0f; //飞行速度
        private DrugStack drug; //所携带的药品
        private Vector2 origin; //起点
        private Vector2 target; //药品落地位置，用于落地时生成药品迷雾
        private float distanceSquare; //运动距离平方，用于计算落地时机
        private float remainTime; //药品的keepTime

        /**
         * <summary>
         * 负责FlyingDrug物品的初始化
         * <param name="drugStack">携带药品</param>
         * <param name="originPos">起点</param>
         * <param name="targetPos">落地点</param>
         * <param name="keepTime">药品的keepTime，FlyingDrug的整个生命周期中都不会修改</param>
         * </summary>
         */
        public void Initialize(DrugStack drugStack, Vector2 originPos, Vector2 targetPos, float keepTime)
        {
            gameObject.name = "FlyingDrug_" + drugStack.Tag;
            drug = drugStack;
            origin = originPos;
            GetComponent<Rigidbody2D>().position = originPos;
            GetComponent<Rigidbody2D>().velocity =
                Utils.LengthenArrow(originPos, targetPos, FlyingVelocity); //设置刚体运动速度
            target = targetPos;
            distanceSquare = Utils.GetMagnitudeSquare2D(originPos, targetPos); //计算移动距离
            remainTime = keepTime;
        }

        private void Update()
        {
            var position = GetComponent<Rigidbody2D>().position;
            if (Utils.GetMagnitudeSquare2D(position, origin) >= distanceSquare)
                Invoke(); //若移动距离达到目标值则落地，不用target来判断是因为误差过大很可能导致最终不落地
        }

        /**
         * <summary>
         * 落地方法
         * 原则上只有FlyingDrug的Update可以调用。
         * </summary>
         */
        private void Invoke()
        {
            BuffInvoker.Invoke(drug, target, remainTime);
            Destroy(gameObject);
        }
    }
}