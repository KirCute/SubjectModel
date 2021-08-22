namespace SubjectModel.Scripts.Event.EncounterEvent
{
    public class EncounterEventListener
    {
        public delegate void EventListenerDelegate();

        public event EventListenerDelegate OnEvent;

        public void Execute()
        {
            OnEvent?.Invoke();
        }
    }
}