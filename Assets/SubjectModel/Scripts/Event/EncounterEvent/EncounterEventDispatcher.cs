using System.Collections.Generic;
using UnityEngine;

namespace SubjectModel.Scripts.Event.EncounterEvent
{
    public class EncounterEventDispatcher
    {
        private readonly Dictionary<int, EncounterEventListener> listeners =
            new Dictionary<int, EncounterEventListener>();

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

        public void DispatchEvent(int mission)
        {
            if (listeners.ContainsKey(mission)) listeners[mission].Execute();
        }
    }
}