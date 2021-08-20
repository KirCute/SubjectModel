using Bolt;
using SubjectModel.Scripts.Event;
using UnityEngine;

namespace SubjectModel.Scripts.Development
{
    /**
     * <summary>
     * 用来强制转换观察对象的脚本
     * 纯测试用，勿随便乱挂
     * </summary>
     */
    public class OperationTransfer : MonoBehaviour
    {
        public bool trans;

        private void Update()
        {
            if (!trans) return;
            trans = false;
            if (TryGetComponent<StateMachine>(out var sm)) sm.enabled = false;
            EventDispatchers.OteDispatcher.DispatchEvent(gameObject);
        }
    }
}