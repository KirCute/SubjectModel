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
        
        /**
         * <summary>
         * 相机追踪目标切换事件
         * 主要由CameraManager订阅负责切换追踪主体
         * 需要播放剧情时，应触发此事件
         * 注意CameraKeyboard会在发生OperationTransfer时同时触发此事件，这使得追踪目标保持为操作主体。
         * </summary>
         * <param name="layer">
         * 追踪目标的层数（优先级），越小则越优先。
         * 0层为系统层，用于显示UI
         * 1层为剧情层，在播放剧情前（target为剧情发生位置的游戏对象）后（target为null）应使用此层
         * 2层为强制卷轴层（含Boss战时不可移动的卷轴）
         * 3层为操作主体层，OperationTransfer发生时会修改此层的追踪目标
         * </param>
         * <param name="target">
         * 追踪目标
         * 为null时表示释放改层的追踪目标，这将使得相机选择优先级更低的目标进行追踪
         * </param>
         */
        public void DispatchEvent(int layer, Transform target)
        {
            listener.Execute(layer, target);
        }
    }
}