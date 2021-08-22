using System.Linq;
using SubjectModel.Scripts.Subject.Chemistry;
using UnityEngine;
using UnityEngine.UI;

namespace SubjectModel.Scripts.GUI
{
    /**
     * <summary>
     * 更新作战单位沾染情况的GUI
     * 需要挂在画布下的空物体上，该物体有“Oxidizer”和“Reducer”两个挂有Text的子物体
     * </summary>
     */
    public class StainGui : MonoBehaviour
    {
        public BuffRenderer buffRenderer;
        private Text oxidizer;
        private Text reducer;

        private void Start()
        {
            var children = GetComponentsInChildren<Text>();
            oxidizer = children.Where(text => text.name == "Oxidizer").FirstOrDefault();
            reducer = children.Where(text => text.name == "Reducer").FirstOrDefault();
        }

        private void Update()
        {
            if (buffRenderer == null) return;
            buffRenderer.GetReactPotential(out var o, out var r);
            oxidizer.text = $"↑ {(float.IsNegativeInfinity(o) ? "-" : $"{o}")}";
            reducer.text = $"↓ {(float.IsPositiveInfinity(r) ? "-" : $"{r}")}";
        }

        private void OnDisable()
        {
            oxidizer.text = "";
            reducer.text = "";
        }
    }
}