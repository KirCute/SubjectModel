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
            runSpeed = (float) GetComponent<Variables>().declarations.GetDeclaration("RunSpeed").value;
        }

        private void Update()
        {
            var variables = GetComponent<Variables>();
            var strength = (float) variables.declarations.GetDeclaration("Strength").value;
            var max = (float) variables.declarations.GetDeclaration("Energy").value;
            strength += RecoverSpeed * Time.deltaTime;
            if (strength > max) strength = max;
            var speed = strength >= (float) variables.declarations.GetDeclaration("RunnableStrength").value
                ? runSpeed
                : 1.0f;
            variables.declarations.GetDeclaration("RunSpeed").value = speed;
            if ((bool) variables.declarations.GetDeclaration("Running").value && speed > 1.0f &&
                GetComponent<Rigidbody2D>().velocity != Vector2.zero) strength -= CostWhenRunning * Time.deltaTime;
            variables.declarations.GetDeclaration("Strength").value = strength;
        }
    }
}