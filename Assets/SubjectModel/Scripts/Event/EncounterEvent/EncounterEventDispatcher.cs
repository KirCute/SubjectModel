using System.Collections.Generic;

namespace SubjectModel.Scripts.Event.EncounterEvent
{
    public class EncounterEventDispatcher
    {
        private readonly Dictionary<int, EncounterEventListener> listeners =
            new Dictionary<int, EncounterEventListener>();

        /**
         * <param name="missions">要订阅的遭遇代号</param>
         */
        public void AddEventListener(EncounterEventListener.EventListenerDelegate callback, params int[] missions)
        {
            foreach (var mission in missions)
            {
                if (!listeners.ContainsKey(mission)) listeners.Add(mission, new EncounterEventListener());
                listeners[mission].OnEvent += callback;
            }
        }

        public void RemoveEventListener(EncounterEventListener.EventListenerDelegate callback, params int[] missions)
        {
            foreach (var mission in missions)
                if (listeners.ContainsKey(mission))
                    listeners[mission].OnEvent -= callback;
        }

        public void ClearEventListener(params int[] missions)
        {
            foreach (var mission in missions) listeners.Remove(mission);
        }

        /**
         * <summary>
         * 玩家在遭遇敌人（此时SpriteRenderer仍被禁用），或触发剧情时触发的事件
         * 对于多分支任务，需额外订阅线路交汇处的mission以取消对玩家未选择支线的订阅
         * </summary>
         * <param name="mission">任务代号，不同于其它遭遇情形的唯一整数</param>
         */
        public void DispatchEvent(int mission)
        {
            if (listeners.ContainsKey(mission)) listeners[mission].Execute();
        }
    }
}