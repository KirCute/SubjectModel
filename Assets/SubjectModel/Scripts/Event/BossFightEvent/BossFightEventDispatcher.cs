using UnityEngine;

namespace SubjectModel.Scripts.Event.BossFightEvent
{
    public class BossFightEventDispatcher
    {
        private readonly BossFightEventListener listener = new BossFightEventListener();

        public void AddEventListener(BossFightEventListener.EventListenerDelegate callback)
        {
            listener.OnEvent += callback;
        }

        public void RemoveEventListener(BossFightEventListener.EventListenerDelegate callback)
        {
            listener.OnEvent -= callback;
        }

        /**
         * <summary>
         * Boss战开始时间
         * 主要由BossAssistance订阅以生成Boss状态信息GUI
         * </summary>
         * <param name="boss">Boss的游戏对象</param>
         */
        public void DispatchEvent(GameObject boss)
        {
            listener.Execute(boss);
        }
    }
}