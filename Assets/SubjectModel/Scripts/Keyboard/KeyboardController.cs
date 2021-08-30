using Bolt;
using UnityEngine;
using SubjectModel.Scripts.Event;
using SubjectModel.Scripts.System;

namespace SubjectModel.Scripts.Keyboard
{
    /**
     * <summary>
     * 键盘移动脚本
     * 挂在哪里都行，但更建议挂在Utils.Input上
     * entry是游戏开始时第一个被控制的物品，在游戏开始时，脚本会以此为参数触发OperationTransfer事件
     * 在监听到OperationTransfer事件时（包括游戏开始时的自触发事件），它会修改正在被控制的物体
     * 支持的键盘操作有：
     * WASD或上下左右：移动
     * （如果控制物体的Variables中具有Running: bool变量）左Shift：奔跑
     * </summary>
     */
    public class KeyboardController : MonoBehaviour
    {
        public GameObject entry;
        private GameObject operated;

        private void Awake()
        {
            EventDispatchers.OteDispatcher.AddEventListener(OnOperationTransfer);
        }

        private void Start()
        {
            EventDispatchers.OteDispatcher.DispatchEvent(entry);
        }

        private void Update()
        {
            var dec = operated.GetComponent<Variables>().declarations;
            if (dec.IsDefined("Running")) dec.Set("Running", Input.GetKey(KeyCode.LeftShift));
            operated.GetComponent<Movement>().Motivation =
                new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        private void OnOperationTransfer(GameObject newOperatedObject)
        {
            operated = newOperatedObject;
        }
    }
}