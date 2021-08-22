using UnityEngine;

namespace SubjectModel.Scripts.Task
{
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