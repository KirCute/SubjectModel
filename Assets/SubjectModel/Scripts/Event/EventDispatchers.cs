using SubjectModel.Scripts.Event.CameraTransferEvent;
using SubjectModel.Scripts.Event.EncounterEvent;
using SubjectModel.Scripts.Event.EntityDeadEvent;
using SubjectModel.Scripts.Event.OperationTransferEvent;
using SubjectModel.Scripts.Event.BossFightEvent;

namespace SubjectModel.Scripts.Event
{
    /**
     * <summary>
     * 事件发布器获取类
     * 要添加一个新的事件，首先需要在SubjectModel.Scripts.Event.XXXEvent(XXX为事件名)下创建Dispatcher（发布器）和Listener（订阅器）
     * 其中Listener类中有该事件的委托（形参为事件触发时所需的参数）和事件，同时实现Execute方法，原则上只有发布器可以调用该方法。
     * Dispatcher类负责保管事件的订阅器，要求实现AddEventListener和RemoveEventListener方法来添加，删除订阅器，还要实现DispatchEvent方法用于触发事件。
     * 最后，在该类下创建Dispatcher类的唯一静态只读实例。
     *
     * 要订阅一个事件，在OnEnable中调用Dispatcher的AddEventListener方法，在OnDisable中调用Dispatcher的RemoveEventListener方法
     * 要触发一个事件，调用Dispatcher的DispatchEvent方法
     *
     * 对于已有事件的用法，详见事件的Dispatcher类
     * </summary>
     */
    public static class EventDispatchers
    {
        public static readonly OperationTransferEventDispatcher OteDispatcher = new OperationTransferEventDispatcher();
        public static readonly EncounterEventDispatcher EeDispatcher = new EncounterEventDispatcher();
        public static readonly CameraTransferEventDispatcher CteDispatcher = new CameraTransferEventDispatcher();
        public static readonly EntityDeadEventDispatcher EdeDispatcher = new EntityDeadEventDispatcher();
        public static readonly BossFightEventDispatcher BossDispatcher = new BossFightEventDispatcher();
    }
}