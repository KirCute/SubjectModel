using UnityEngine;

namespace SubjectModel.Scripts.Event.OperationTransferEvent
{
    public class OperationTransferEventListener
    {
        public delegate void EventListenerDelegate(GameObject newObject);

        public event EventListenerDelegate OnEvent;

        public void Execute(GameObject newObject)
        {
            OnEvent?.Invoke(newObject);
        }
    }
}