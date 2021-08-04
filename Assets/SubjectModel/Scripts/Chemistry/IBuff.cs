using UnityEngine;

namespace SubjectModel.Scripts.Chemistry
{
    public enum Buff
    {
        Empty,
        Slowness,
        Poison,
        Curing,
        Ghost,
        Corrosion,
        Rapid
    }

    public interface IBuff
    {
        void Appear(GameObject host);
        void Update(GameObject host);
        void UpdateAfterDelay(GameObject host);
        void Destroy(GameObject host);
        bool AfterDelay(GameObject host);
        bool Ended(GameObject host);
        float GetLevel();
        float GetRemainedTime();
        float GetTotalTime();
        void Append(float time);
        void LevelUp(GameObject host, float newLevel);
    }
}