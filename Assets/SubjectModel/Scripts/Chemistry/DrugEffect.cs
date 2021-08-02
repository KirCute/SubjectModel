using Bolt;
using UnityEngine;

namespace SubjectModel.Scripts.Chemistry
{
    public static class DrugEffect
    {
        public class RemainingBuff : IBuff
        {
            private float remainedTime;
            private float totalTime;
            private float level;

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
            }

            public virtual void UpdateAfterDelay(GameObject host)
            {
                remainedTime += Time.deltaTime;
            }

            public virtual void Destroy(GameObject host)
            {
            }

            public virtual bool AfterDelay(GameObject host)
            {
                return true;
            }

            public virtual bool Ended(GameObject host)
            {
                return remainedTime >= totalTime;
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
            }

            public virtual void LevelUp(GameObject gameObject, float newLevel)
            {
                level = newLevel;
            }
        }

        public class PoisonLike : RemainingBuff
        {
            private readonly string variable;

            public PoisonLike(string variable, float remain, float level) : base(remain, level)
            {
                this.variable = variable;
            }

            public override void UpdateAfterDelay(GameObject host)
            {
                base.UpdateAfterDelay(host);
                Invoke(variable, host, GetLevel());
            }

            private static void Invoke(string variable, GameObject host, float v)
            {
                var origin = host.GetComponent<Variables>().declarations.Get<float>(variable);
                host.GetComponent<Variables>().declarations.Set(variable, origin - v * Time.deltaTime);
            }
        }

        public class IIIFe : RemainingBuff
        {
            private bool firstUpdate;

            public IIIFe(float remain, float level) : base(remain, level)
            {
                this.firstUpdate = true;
            }

            public override void UpdateAfterDelay(GameObject host)
            {
                base.UpdateAfterDelay(host);
                if (!firstUpdate) return;
                firstUpdate = false;
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

        public class IICu : PoisonLike
        {
            public IICu(float remain, float level) : base("Health", remain, level)
            {
            }
        }

        public class IICo : IICu
        {
            public IICo(float remain, float level) : base(remain, -level)
            {
            }
        }

        public class PIII : RemainingBuff
        {
            private const float RatioSecToHealth = .05f;
            
            private float reserveHealth;
            private readonly float healthLost;

            public PIII(float remain, float lostPerSec) : base(remain, lostPerSec)
            {
                reserveHealth = .0f;
                healthLost = lostPerSec * RatioSecToHealth;
            }

            public override void UpdateAfterDelay(GameObject host)
            {
                base.UpdateAfterDelay(host);
                var variables = host.GetComponent<Variables>();

                var health = variables.declarations.Get<float>("Health");
                var lost = GetLevel() * Time.deltaTime;
                if (reserveHealth >= health)
                {
                    lost += (reserveHealth - health) * healthLost;
                    variables.declarations.Set("Health", reserveHealth);
                }
                else reserveHealth = health;

                if (!variables.declarations.IsDefined("Energy")) return;
                var origin = variables.declarations.Get<float>("Energy");
                variables.declarations.Set("Energy", origin - lost);
            }
        }

        public class H : PoisonLike
        {
            public H(float remain, float level) : base("Defence", remain, level)
            {
            }
        }

        public class IIFe : IIIFe
        {
            public IIFe(float remain, float level) : base(remain, 1f / level)
            {
            }
        }
    }
}