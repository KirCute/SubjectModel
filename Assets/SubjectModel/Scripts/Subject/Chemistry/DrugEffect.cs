using Bolt;
using UnityEngine;

namespace SubjectModel.Scripts.Subject.Chemistry
{
    /**
      * <summary>所有效果的接口</summary>
      */
    public interface IBuff
    {
        /**
         * <summary>
         * 效果在被添加时被调用
         * <param name="host">效果的宿主</param>
         * </summary>
         */
        void Appear(GameObject host);

        /**
         * <summary>
         * 效果在每帧更新时被调用
         * <param name="host">效果的宿主</param>
         * </summary>
         */
        void Update(GameObject host);

        /**
         * <summary>
         * 效果在结束时被调用
         * <param name="host">效果的宿主</param>
         * </summary>
         */
        void Destroy(GameObject host);

        /**
         * <summary>
         * 判断效果是否应当结束的方法
         * <param name="host">效果的宿主</param>
         * <returns>效果是否即将结束</returns>
         * </summary>
         */
        bool Ended(GameObject host);

        /**
         * <summary>
         * 得到效果等级的方法
         * <returns>效果等级</returns>
         * </summary>
         */
        float GetLevel();

        /**
         * <summary>
         * 得到效果已经历时间的方法
         * <returns>效果已经历时间</returns>
         * </summary>
         */
        float GetRemainedTime();

        /**
         * <summary>
         * 得到效果总时间的方法
         * <returns>效果总时间</returns>
         * </summary>
         */
        float GetTotalTime();

        /**
         * <summary>
         * 更改效果总时间的方法
         * 该方法会使已持续时间重新计算。
         * <param name="time">新的效果总时间</param>
         * </summary>
         */
        void Append(float time);

        /**
         * <summary>
         * 更改效果等级的方法
         * 对于在起始和结束时改变宿主状态的效果（如缓慢），应当重写此方法以重新计算其对宿主的影响。
         * <param name="newLevel">新的效果等级</param>
         * </summary>
         */
        void LevelUp(GameObject host, float newLevel);
    }

    /**
     * <summary>
     * （以内部类的形式）存放效果主类的工具类
     * 添加新的效果时，除了要在此处创建效果主类以外，还需要在ChemistryUtils中补充有关内容
     * 由于效果的实例对象都通过反射的方式创建，应额外注意那些构造函数参数与众不同的效果
     * </summary>
     */
    public static class DrugEffect
    {
        /**
         * <summary>所有能持续一段时间的效果的基类</summary>
         */
        public class RemainingBuff : IBuff
        {
            private float remainedTime; //已存在时间
            private float totalTime; //总时间
            private float level; //等级

            /**
             * <summary>
             * 默认构造器
             * <param name="remain">持续总时间</param>
             * <param name="level">等级</param>
             * </summary>
             */
            public RemainingBuff(float remain, float level = .0f)
            {
                this.remainedTime = .0f;
                this.totalTime = remain;
                this.level = level;
            }

            public virtual void Appear(GameObject host)
            {
            }

            public virtual void Update(GameObject host)
            {
                remainedTime += Time.deltaTime;
            }

            public virtual void Destroy(GameObject host)
            {
            }

            public virtual bool Ended(GameObject host)
            {
                return totalTime >= 0f && remainedTime >= totalTime;
            }

            public virtual float GetLevel()
            {
                return level;
            }

            public virtual float GetRemainedTime()
            {
                return remainedTime;
            }

            public virtual float GetTotalTime()
            {
                return totalTime;
            }

            public virtual void Append(float time)
            {
                totalTime = time;
                remainedTime = .0f;
            }

            public virtual void LevelUp(GameObject gameObject, float newLevel)
            {
                level = newLevel;
            }
        }

        /**
         * <summary>
         * 类似于中毒一样持续造成某项资源减少（或取负为增加）的效果
         * 此效果更适合用来做其它效果的基类而非效果本身。
         * <c>注意此效果的构造函数有三个参数</c>
         * </summary>
         */
        public class PoisonLike : RemainingBuff
        {
            private readonly string variable;

            public PoisonLike(string variable, float remain, float level) : base(remain, level)
            {
                this.variable = variable;
            }

            public override void Update(GameObject host)
            {
                base.Update(host);
                Invoke(variable, host, GetLevel());
            }

            private static void Invoke(string variable, GameObject host, float v)
            {
                var origin = host.GetComponent<Variables>().declarations.Get<float>(variable);
                host.GetComponent<Variables>().declarations.Set(variable, origin - v * Time.deltaTime);
            }
        }

        /**
         * <summary>缓慢</summary>
         */
        public class Slowness : RemainingBuff
        {
            public Slowness(float remain, float level) : base(remain, level)
            {
            }

            public override void Appear(GameObject host)
            {
                base.Appear(host);
                Speed(host, 1.0f / GetLevel());
            }

            public override void Destroy(GameObject host)
            {
                Speed(host, GetLevel());
                base.Destroy(host);
            }

            public override void LevelUp(GameObject host, float newLevel)
            {
                Speed(host, GetLevel() / newLevel);
                base.LevelUp(host, newLevel);
            }

            private static void Speed(GameObject host, float speed)
            {
                var origin = host.GetComponent<Variables>().declarations.Get<float>("Speed");
                host.GetComponent<Variables>().declarations.Set("Speed", origin * speed);
            }
        }

        /**
         * <summary>中毒</summary>
         */
        public class Poison : PoisonLike
        {
            public Poison(float remain, float level) : base("Health", remain, level)
            {
            }
        }

        /**
         * <summary>治疗</summary>
         */
        public class Curing : Poison
        {
            public Curing(float remain, float level) : base(remain, -level)
            {
            }
        }

        /**
         * <summary>负三价磷的效果，目前没想好名字</summary>
         */
        public class Ghost : RemainingBuff
        {
            private const float RatioSecToHealth = .05f; //效果每血量扣精神量与每秒扣精神量的比值

            private float reserveHealth; //触发效果时的血量，效果期间该值为最低血量
            private readonly float healthLost; //每血量扣精神量

            public Ghost(float remain, float lostPerSec) : base(remain, lostPerSec)
            {
                reserveHealth = .0f;
                healthLost = lostPerSec * RatioSecToHealth;
            }

            public override void Update(GameObject host)
            {
                base.Update(host);
                var variables = host.GetComponent<Variables>();

                var health = variables.declarations.Get<float>("Health"); //得到血量
                var lost = GetLevel() * Time.deltaTime; //得到当前帧扣精神量
                if (reserveHealth >= health) //在效果期间扣血
                {
                    lost += (reserveHealth - health) * healthLost; //将扣血量换算为精神损失
                    variables.declarations.Set("Health", reserveHealth); //回复扣除的血量
                }
                else reserveHealth = health; //在效果期间回血，则更新最低血量

                if (!variables.declarations.IsDefined("Energy")) return; //若宿主有精神条
                var origin = variables.declarations.Get<float>("Energy");
                variables.declarations.Set("Energy", origin - lost); //则扣除精神
            }
        }

        /**
         * <summary>腐蚀</summary>
         */
        public class Corrosion : PoisonLike
        {
            public Corrosion(float remain, float level) : base("Defence", remain, level)
            {
            }
        }

        /**
         * <summary>速度</summary>
         */
        public class Rapid : Slowness
        {
            public Rapid(float remain, float level) : base(remain, 1f / level)
            {
            }
        }

        /**
         * <summary>
         * 免疫，清除所有效果，效果期间不能再获得新的效果
         * 所以为什么免疫效果是一种效果？
         * 需要注意的是，由于免疫触发时会导致所有效果被清除，由系统提供的效果应在被清除后免疫触发前（即在效果的Destroy方法内）重新作用于作战单位。
         * 免疫不影响效果（主要指系统效果和免疫本身）的升级和延时。
         * <c>注意此效果的构造函数只有一个参数</c>
         * </summary>
         */
        public class Immune : RemainingBuff
        {
            public Immune(float remain) : base(remain, 1f)
            {
            }

            public override void Appear(GameObject host)
            {
                base.Appear(host);
                host.GetComponent<BuffRenderer>().ClearBuff();
                host.GetComponent<BuffRenderer>().immune = true;
            }

            public override void Destroy(GameObject host)
            {
                host.GetComponent<BuffRenderer>().immune = false;
                base.Destroy(host);
            }

            public override void LevelUp(GameObject host, float newLevel)
            {
                base.LevelUp(host, newLevel);
            }
        }
    }
}