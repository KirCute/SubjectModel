using SubjectModel.Scripts.Event;
using SubjectModel.Scripts.Event.OperationTransferEvent;
using SubjectModel.Scripts.System;
using UnityEngine;

namespace SubjectModel.Scripts.Keyboard
{
    /**
     * <summary>
     * 控制摄像头瞄准对象的脚本
     * 控制主体更改时会同步更改瞄准对象
     * </summary>
     */
    public class CameraKeyboard : MonoBehaviour
    {
        private static readonly OperationTransferEventListener.EventListenerDelegate Listener = obj =>
            EventDispatchers.CteDispatcher.DispatchEvent(CameraManager.LayerPlayer, obj.transform);

        private void OnEnable()
        {
            EventDispatchers.OteDispatcher.AddEventListener(Listener);
        }

        private void OnDisable()
        {
           EventDispatchers.OteDispatcher.RemoveEventListener(Listener);
        }
    }
}