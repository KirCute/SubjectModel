using System;
using Bolt;
using UnityEngine;

namespace SubjectModel.Scripts.GUI
{
    [RequireComponent(typeof(Variables))]
    public class HealthBarHelper : MonoBehaviour
    {
        private GameObject stateBar;

        private void Start()
        {
            stateBar = (GameObject) Instantiate(Resources.Load("Prefab/Enemy State Bar"),
                GameObject.FindWithTag("EnemiesStateBar").transform);
            stateBar.name = name;
            foreach (var bar in stateBar.GetComponentsInChildren<Transform>())
                switch (bar.name)
                {
                    case "Health Bar":
                        bar.GetComponent<Bar>().sourceObject = gameObject;
                        break;
                }
        }

        private void Update()
        {
            if (Camera.main == null) return;
            stateBar.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        }

        private void OnDestroy()
        {
            Destroy(stateBar);
        }
    }
}