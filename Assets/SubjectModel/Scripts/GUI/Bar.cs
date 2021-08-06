using System;
using Bolt;
using UnityEngine;

namespace SubjectModel.Scripts.GUI
{
    [RequireComponent(typeof(RectTransform))]
    public class Bar : MonoBehaviour
    {
        public GameObject sourceObject;
        public string source;
        public string sourceEnd;
        public float targetEnd = 200f;
        public float y = 15f;

        private void Update()
        {
            if (sourceObject == null || !sourceObject.TryGetComponent<Variables>(out var variables)) return;
            var dec = variables.declarations;
            GetComponent<RectTransform>().sizeDelta =
                new Vector2(Utils.Map(.0f, dec.Get<float>(sourceEnd), .0f, targetEnd, dec.Get<float>(source)), y);
        }

        private void OnDisable()
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(0f, y);
        }
    }
}