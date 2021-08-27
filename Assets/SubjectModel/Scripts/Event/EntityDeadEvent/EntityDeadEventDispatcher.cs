using System.Collections.Generic;
using UnityEngine;

namespace SubjectModel.Scripts.Event.EntityDeadEvent
{
    public class EntityDeadEventDispatcher
    {
        private readonly Dictionary<GameObject, EntityDeadEventListener> listeners =
            new Dictionary<GameObject, EntityDeadEventListener>();

        /**
         * <param name="objs">在objs中的任意一个死亡时，都会调用callback</param>
         */
        public void AddEventListener(EntityDeadEventListener.EventListenerDelegate callback, params GameObject[] objs)
        {
            foreach (var obj in objs)
            {
                if (obj == null) continue;
                if (!listeners.ContainsKey(obj)) listeners.Add(obj, new EntityDeadEventListener());
                listeners[obj].OnEvent += callback;
            }
        }

        public void RemoveEventListener(EntityDeadEventListener.EventListenerDelegate callback,
            params GameObject[] objs)
        {
            foreach (var obj in objs)
                if (obj != null && listeners.ContainsKey(obj))
                    listeners[obj].OnEvent -= callback;
        }

        public void ClearEventListener(params GameObject[] objs)
        {
            foreach (var obj in objs) listeners.Remove(obj);
        }

        /**
         * <summary>
         * 实体死亡事件
         * 主要用于处理掉落物生成和玩家重生
         * 原则上只能由负责实体生命周期的脚本（多数为Physiology）触发
         * </summary>
         * <param name="entity">死亡的实体，该参数不会传入事件委托</param>
         */
        public void DispatchEvent(GameObject entity)
        {
            if (listeners.ContainsKey(entity)) listeners[entity].Execute();
        }
    }
}