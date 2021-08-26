using UnityEngine;

namespace SubjectModel.Scripts.Subject.Electronics
{
    public enum SignalType
    {
        Digital,
        Analog,
        Serial,
        OneWire,
    }

    public class Wire
    {
        private const float MaxVoltage = 5f;
        private const float SignalVoltage = 3.3f;

        private SignalType Type { get; set; }
        private object Signal { get; set; }

        public void Output(SignalType type, object value)
        {
            if (type == SignalType.Analog)
            {
                if ((float) value > MaxVoltage) value = MaxVoltage;
                if ((float) value < 0f) value = 0f;
            }
            Type = type;
            Signal = value;
        }

        public T Read<T>(SignalType type)
        {
            if (Signal == null) return default;
            return (T) (Type switch
            {
                SignalType.Digital => type switch
                {
                    SignalType.Digital => (bool) Signal,
                    SignalType.Analog => (bool) Signal ? MaxVoltage : 0f,
                    SignalType.Serial => (bool) Signal ? "~" : " ",
                    SignalType.OneWire => (bool) Signal,
                    _ => Signal
                },
                SignalType.Analog => type switch
                {
                    SignalType.Digital => (float) Signal >= SignalVoltage,
                    SignalType.Analog => (float) Signal,
                    SignalType.Serial => (float) Signal >= 0f ? "~" : " ",
                    SignalType.OneWire => (float) Signal,
                    _ => Signal
                },
                SignalType.Serial => type switch
                {
                    SignalType.Digital => Signal.ToString().Length > 0 ? 0f : SignalVoltage,
                    SignalType.Analog => Signal.ToString().Length > 0 ? 0f : SignalVoltage,
                    SignalType.Serial => Signal.ToString(),
                    SignalType.OneWire => Vector2.zero,
                    _ => Signal
                },
                SignalType.OneWire => type switch
                {
                    SignalType.Digital => Signal != null,
                    SignalType.Analog => Signal == null ? 0f : SignalVoltage,
                    SignalType.Serial => Signal.ToString(),
                    SignalType.OneWire => Signal,
                    _ => Signal
                },
                _ => Signal
            });
        }
    }
}