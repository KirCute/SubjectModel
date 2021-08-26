using Bolt;
using UnityEngine;
using SubjectModel.Scripts.Event;
using SubjectModel.Scripts.System;

namespace SubjectModel.Scripts.Keyboard
{
    public class KeyboardController : MonoBehaviour
    {
        public GameObject entry;
        private GameObject operated;

        private void Awake()
        {
            EventDispatchers.OteDispatcher.AddEventListener(OnOperationTransfer);
        }

        private void Start()
        {
            EventDispatchers.OteDispatcher.DispatchEvent(entry);
        }

        private void Update()
        {
            var dec = operated.GetComponent<Variables>().declarations;
            if (dec.IsDefined("Running")) dec.Set("Running", Input.GetKey(KeyCode.LeftShift));
            operated.GetComponent<Movement>().Motivation =
                new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        private void OnOperationTransfer(GameObject newOperatedObject)
        {
            operated = newOperatedObject;
        }
    }
}