using Cinemachine;
using SubjectModel.Scripts.Event;
using UnityEngine;

namespace SubjectModel.Scripts.System
{
    /**
     * <summary>
     * 相机管理脚本
     * 订阅CameraTransfer事件以持续追踪目标物体
     * 需要挂载在CinemachineVirtualCamera上
     * </summary>
     */
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CameraManager : MonoBehaviour
    {
        public const int LayerSystem = 0;
        public const int LayerStory = 1;
        public const int LayerScroll = 2;
        public const int LayerPlayer = 3;

        private readonly Transform[] layerTarget = {null, null, null, null};

        private void OnEnable()
        {
            EventDispatchers.CteDispatcher.AddEventListener(OnCameraTransfer);
        }

        /**
         * <summary>
         * 得到从第index层开始的追踪物体（无参时即为实际追踪物体）
         * </summary>
         */
        private Transform GetTarget(int index = 0)
        {
            while (layerTarget[index] == null && index < layerTarget.Length - 1) index++;
            return layerTarget[index];
        }

        private void OnCameraTransfer(int layer, Transform target)
        {
            layerTarget[layer] = target;
            GetComponent<CinemachineVirtualCamera>().Follow = GetTarget();
        }

        private void OnDisable()
        {
            EventDispatchers.CteDispatcher.RemoveEventListener(OnCameraTransfer);
        }
    }
}