namespace SubjectModel.Scripts.Event.EntityDeadEvent
{
    public class EntityDeadEventListener
    {
        public delegate void EventListenerDelegate();

        public event EventListenerDelegate OnEvent;

        public void Execute()
        {
            OnEvent?.Invoke();
        }
    }
}