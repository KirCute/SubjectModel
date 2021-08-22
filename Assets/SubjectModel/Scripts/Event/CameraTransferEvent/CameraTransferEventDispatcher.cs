using UnityEngine;

namespace SubjectModel.Scripts.Event.CameraTransferEvent
{
    public class CameraTransferEventDispatcher
    {
        private readonly CameraTransferEventListener listener = new CameraTransferEventListener();

        public void AddEventListener(CameraTransferEventListener.EventListenerDelegate callback)
        {
            listener.OnEvent += callback;
        }

        public void RemoveEventListener(CameraTransferEventListener.EventListenerDelegate callback)
        {
            listener.OnEvent -= callback;
        }
        
        public void DispatchEvent(int layer, Transform target)
        {
            listener.Execute(layer, target);
        }
    }
}