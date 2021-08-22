using Cinemachine;
using SubjectModel.Scripts.Event;
using UnityEngine;

namespace SubjectModel.Scripts.System
{
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