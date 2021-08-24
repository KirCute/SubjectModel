using UnityEngine;

namespace SubjectModel.Scripts.Subject.Electronics
{
    public interface IMachineComponent
    {
        public float[] PowerRequirement { get; }
        public float PowerUsing { get; set; }
        public float Watt { get; }
        public void OnInstall(Machine machine);
        public void OnUninstall(Machine machine);
        public void Update(GameObject gameObject);
        public void OnShortage(GameObject gameObject);
    }
}