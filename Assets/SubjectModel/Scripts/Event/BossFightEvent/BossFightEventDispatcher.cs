using UnityEngine;

namespace SubjectModel.Scripts.Event.BossFightEvent
{
    public class BossFightEventDispatcher
    {
        private readonly BossFightEventListener listener = new BossFightEventListener();

        public void AddEventListener(BossFightEventListener.EventListenerDelegate callback)
        {
            listener.OnEvent += callback;
        }

        public void RemoveEventListener(BossFightEventListener.EventListenerDelegate callback)
        {
            listener.OnEvent -= callback;
        }

        public void DispatchEvent(GameObject boss)
        {
            listener.Execute(boss);
        }
    }
}