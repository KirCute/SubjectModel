namespace SubjectModel.Scripts.Subject.Electronics
{
    public interface IBattery
    {
        public float Voltage { get; }
        public float Watt { get; }
        public float Capacity { get; set; }
    }
}