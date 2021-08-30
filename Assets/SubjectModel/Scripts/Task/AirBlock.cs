using UnityEngine;

namespace SubjectModel.Scripts.Task
{
    /**
     * <summary>
     * 空气墙
     * 需要挂载在任务物体下，在遭遇，玩家失败/成功时，会改变子物体碰撞箱的启用状态
     * </summary>
     */
    public class AirBlock : MissionObject
    {
        protected override void Encounter()
        {
            foreach (var wall in GetComponentsInChildren<Collider2D>()) wall.enabled = true;
        }

        protected override void Failed()
        {
            foreach (var wall in GetComponentsInChildren<Collider2D>()) wall.enabled = false;
        }

        protected override void Defeated()
        {
            foreach (var wall in GetComponentsInChildren<Collider2D>()) wall.enabled = false;
        }
    }
}