using UnityEngine;

namespace SubjectModel.Scripts.Event.CameraTransferEvent
{
    public class CameraTransferEventListener
    {
        public delegate void EventListenerDelegate(int layer, Transform target);

        public event EventListenerDelegate OnEvent;

        public void Execute(int layer, Transform target)
        {
            OnEvent?.Invoke(layer, target);
        }
    }
}