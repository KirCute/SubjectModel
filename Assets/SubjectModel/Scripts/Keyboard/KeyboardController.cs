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
            var speed = dec.Get<float>("Speed");
            if (dec.IsDefined("Running"))
            {
                var running = Input.GetKey(KeyCode.LeftShift);
                dec.Set("Running", running);
                speed *= running ? dec.Get<float>("RunSpeed") : 1.0f;
            }

            operated.GetComponent<Rigidbody2D>().velocity = dec.Get<int>("Standonly") > 0
                ? Vector2.zero
                : new Vector2(
                    Input.GetAxisRaw("Horizontal") * speed,
                    Input.GetAxisRaw("Vertical") * speed
                );
        }

        private void OnOperationTransfer(GameObject newOperatedObject)
        {
            operated = newOperatedObject;
        }
    }
}