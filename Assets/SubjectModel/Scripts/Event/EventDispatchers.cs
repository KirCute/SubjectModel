using SubjectModel.Scripts.Event.CameraTransferEvent;
using SubjectModel.Scripts.Event.EncounterEvent;
using SubjectModel.Scripts.Event.EntityDeadEvent;
using SubjectModel.Scripts.Event.OperationTransferEvent;
using SubjectModel.Scripts.Event.BossFightEvent;

namespace SubjectModel.Scripts.Event
{
    public static class EventDispatchers
    {
        public static readonly OperationTransferEventDispatcher OteDispatcher = new OperationTransferEventDispatcher();
        public static readonly EncounterEventDispatcher EeDispatcher = new EncounterEventDispatcher();
        public static readonly CameraTransferEventDispatcher CteDispatcher = new CameraTransferEventDispatcher();
        public static readonly EntityDeadEventDispatcher EdeDispatcher = new EntityDeadEventDispatcher();
        public static readonly BossFightEventDispatcher BossDispatcher = new BossFightEventDispatcher();
    }
}