using Bolt;
using UnityEngine;

namespace SubjectModel
{
    [RequireComponent(typeof(Variables))]
    public class Strength : MonoBehaviour
    {
        private const float RecoverSpeed = 15f;
        private const float CostWhenRunning = 50f;
        private float runSpeed;

        private void Start()
        {
            runSpeed = GetComponent<Variables>().declarations.Get<float>("RunSpeed");
        }

        private void Update()
        {
            var variables = GetComponent<Variables>();
            var strength = variables.declarations.Get<float>("Strength");
            var max = variables.declarations.Get<float>("Energy");
            strength += RecoverSpeed * Time.deltaTime;
            if (strength > max) strength = max;
            var speed = strength >= variables.declarations.Get<float>("RunnableStrength") 
                ? runSpeed : 1.0f;
            variables.declarations.Set("RunSpeed", speed);
            if (variables.declarations.Get<bool>("Running") && speed > 1.0f &&
                GetComponent<Rigidbody2D>().velocity != Vector2.zero) strength -= CostWhenRunning * Time.deltaTime;
            variables.declarations.Set("Strength", strength);
        }
    }
}