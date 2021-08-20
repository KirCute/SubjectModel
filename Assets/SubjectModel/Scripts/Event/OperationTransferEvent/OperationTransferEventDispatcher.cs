using UnityEngine;

namespace SubjectModel.Scripts.Event.OperationTransferEvent
{
    public class OperationTransferEventDispatcher
    {
        private readonly OperationTransferEventListener listener = new OperationTransferEventListener();

        public void AddEventListener(OperationTransferEventListener.EventListenerDelegate callback)
        {
            listener.OnEvent += callback;
        }

        public void RemoveEventListener(OperationTransferEventListener.EventListenerDelegate callback)
        {
            listener.OnEvent -= callback;
        }
        
        public void DispatchEvent(GameObject newObject)
        {
            listener.Execute(newObject);
        }
    }
}