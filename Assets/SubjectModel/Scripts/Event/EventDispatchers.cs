using SubjectModel.Scripts.Event.OperationTransferEvent;

namespace SubjectModel.Scripts.Event
{
    public static class EventDispatchers
    {
        public static readonly OperationTransferEventDispatcher OteDispatcher = new OperationTransferEventDispatcher();
    }
}