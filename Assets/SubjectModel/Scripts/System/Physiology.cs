using System;
using Bolt;
using SubjectModel.Scripts.Event;
using UnityEngine;

namespace SubjectModel.Scripts.System
{
    [RequireComponent(typeof(Variables))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Physiology : MonoBehaviour
    {
        private const float RecoverSpeed = 30f;
        private const float CostWhenRunning = 50f;
        private const float RunnableStrength = 20f;
        private VariableDeclarations dec;
        private float runSpeed;
        private bool dying;
        public bool removeAfterDead;

        private void Start()
        {
            dec = GetComponent<Variables>().declarations;
            runSpeed = dec.IsDefined("RunSpeed") ? dec.Get<float>("RunSpeed") : 1.0f;
        }

        private void Update()
        {
            if (dec.Get<float>("Health") <= 0f || dec.Get<float>("Energy") <= 0f)
            {
                if (!dying)
                {
                    dying = true;
                    EventDispatchers.EdeDispatcher.DispatchEvent(gameObject);
                    if (removeAfterDead) Destroy(gameObject);
                }
            }
            else dying = false;

            dec.Set("Health", Math.Min(dec.Get<float>("Health"), dec.Get<float>("MaxHealth")));
            if (dec.IsDefined("Defence")) dec.Set("Defence", Math.Max(dec.Get<float>("Defence"), 0f));

            var strength = dec.Get<float>("Strength");
            var max = dec.Get<float>("Energy");
            strength += RecoverSpeed * Time.deltaTime;
            if (dec.IsDefined("Running"))
            {
                var speed = strength >= RunnableStrength ? runSpeed : 1.0f;
                dec.Set("RunSpeed", speed);
                if (dec.Get<bool>("Running") && speed > 1.0f &&
                    GetComponent<Rigidbody2D>().velocity != Vector2.zero) strength -= CostWhenRunning * Time.deltaTime;
            }

            if (strength > max) strength = max;
            dec.Set("Strength", strength);
        }

        private void OnDestroy()
        {
            EventDispatchers.EdeDispatcher.ClearEventListener(gameObject);
        }
    }
}