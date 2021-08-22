using Bolt;
using SubjectModel.Scripts.Subject.Chemistry;
using UnityEngine;

namespace SubjectModel.Scripts.GUI
{
    /**
     * <summary>
     * 更新一般敌人状态条GUI的脚本
     * 需要挂载在作战单位上
     * </summary>
     */
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
                        bar.GetComponent<Bar>().sourceVariables = gameObject.GetComponent<Variables>();
                        break;
                    case "Stain":
                        bar.GetComponent<StainGui>().buffRenderer = gameObject.GetComponent<BuffRenderer>();
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