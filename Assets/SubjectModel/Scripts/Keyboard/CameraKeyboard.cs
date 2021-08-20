using Cinemachine;
using SubjectModel.Scripts.Event;
using UnityEngine;

namespace SubjectModel.Scripts.Keyboard
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CameraKeyboard : MonoBehaviour
    {
        private void Awake()
        {
            EventDispatchers.OteDispatcher.AddEventListener(OnOperationTransfer);
        }

        private void OnOperationTransfer(GameObject newObject)
        {
            GetComponent<CinemachineVirtualCamera>().Follow = newObject.transform;
        }
    }
}