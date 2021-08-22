using Bolt;
using SubjectModel.Scripts.Event;
using UnityEngine;

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
            var runSpeed = dec.IsDefined("RunSpeed") ? dec.Get<float>("RunSpeed") : 1.0f;
            var running = Input.GetKey(KeyCode.LeftShift);
            dec.Set("Running", running);
            var run = running ? runSpeed : 1.0f;
            var speed = dec.Get<float>("Speed");
            operated.GetComponent<Rigidbody2D>().velocity = dec.Get<int>("Standonly") > 0
                ? Vector2.zero
                : new Vector2(
                    Input.GetAxisRaw("Horizontal") * speed * run,
                    Input.GetAxisRaw("Vertical") * speed * run
                );
        }

        private void OnOperationTransfer(GameObject newOperatedObject)
        {
            operated = newOperatedObject;
        }
    }
}