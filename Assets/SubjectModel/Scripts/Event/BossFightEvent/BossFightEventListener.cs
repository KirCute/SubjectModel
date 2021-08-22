using UnityEngine;

namespace SubjectModel.Scripts.Event.BossFightEvent
{
    public class BossFightEventListener
    {
        public delegate void EventListenerDelegate(GameObject boss);

        public event EventListenerDelegate OnEvent;

        public void Execute(GameObject boss)
        {
            OnEvent?.Invoke(boss);
        }
    }
}