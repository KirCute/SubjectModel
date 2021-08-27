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
        
        /**
         * <summary>
         * 操作主体更改时触发的事件
         * 注意并非操作主体更改导致该事件触发，而是该事件触发导致操作主体修改，因此当需要修改操作主体时可以通过触发此事件实现
         * CameraKeyboard会在该事件触发时同时触发一个CameraTransfer事件，用于保证摄像头的跟踪对象为操作主体
         * </summary>
         */
        public void DispatchEvent(GameObject newObject)
        {
            listener.Execute(newObject);
        }
    }
}