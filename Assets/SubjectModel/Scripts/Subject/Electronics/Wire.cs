namespace SubjectModel.Scripts.Subject.Electronics
{
    public enum SignalType
    {
        Stable,
        Serial
    }

    public class Wire
    {
        private const float SignalVoltage = 3.3f;

        private SignalType Type { get; set; }
        private object Signal { get; set; }

        public void Output(SignalType type, object value)
        {
            Type = type;
            Signal = value;
        }

        public T Read<T>(SignalType type)
        {
            if (Signal == null) return default;
            return (T) (Type switch
            {
                SignalType.Stable => type switch
                {
                    SignalType.Stable => (float) Signal,
                    SignalType.Serial => Signal.ToString(),
                    _ => Signal
                },
                SignalType.Serial => type switch
                {
                    SignalType.Stable => Signal.ToString().Length > 0 ? 0f : SignalVoltage,
                    SignalType.Serial => Signal.ToString(),
                    _ => Signal
                },
                _ => Signal
            });
        }
    }
}