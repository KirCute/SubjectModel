namespace SubjectModel.Scripts.Subject.Electronics
{
    public interface IBattery
    {
        public float Voltage { get; }
        public float Watt { get; }
        /**
         * <summary>
         * 电量消耗方法
         * 需由MachineComponent自行调用
         * (逐帧调用记得乘<code>Time.deltaTime</code>)
         * </summary>
         */
        public bool Cost(float amount);
    }
}