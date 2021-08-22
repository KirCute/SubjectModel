using System.Collections.Generic;
using UnityEngine;

namespace SubjectModel.Scripts.Event.EntityDeadEvent
{
    public class EntityDeadEventDispatcher
    {
        private readonly Dictionary<GameObject, EntityDeadEventListener> listeners =
            new Dictionary<GameObject, EntityDeadEventListener>();

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

        public void DispatchEvent(GameObject entity)
        {
            if (listeners.ContainsKey(entity)) listeners[entity].Execute();
        }
    }
}