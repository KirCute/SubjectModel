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
            runSpeed = (float) GetComponent<Variables>().declarations.Get("RunSpeed");
        }

        private void Update()
        {
            var variables = GetComponent<Variables>();
            var strength = (float) variables.declarations.Get("Strength");
            var max = (float) variables.declarations.Get("Energy");
            strength += RecoverSpeed * Time.deltaTime;
            if (strength > max) strength = max;
            var speed = strength >= (float) variables.declarations.Get("RunnableStrength")
                ? runSpeed
                : 1.0f;
            variables.declarations.Set("RunSpeed", speed);
            if ((bool) variables.declarations.Get("Running") && speed > 1.0f &&
                GetComponent<Rigidbody2D>().velocity != Vector2.zero) strength -= CostWhenRunning * Time.deltaTime;
            variables.declarations.Set("Strength", strength);
        }
    }
}