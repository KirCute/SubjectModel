using System;
using Bolt;
using UnityEngine;

namespace SubjectModel.Scripts.System
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Variables))]
    public class Movement : MonoBehaviour
    {
        [NonSerialized] public Vector2 Motivation = Vector2.zero;

        private void Update()
        {
            var dec = GetComponent<Variables>().declarations;
            var speed = dec.Get<float>("Speed") *
                        (dec.IsDefined("Standonly") && dec.Get<int>("Standonly") > 0 ? 0f : 1f) *
                        (dec.IsDefined("Running") && dec.Get<bool>("Running") ? dec.Get<float>("RunSpeed") : 1f);
            GetComponent<Rigidbody2D>().velocity = Utils.LengthenVector(Motivation, speed);
        }
    }
}