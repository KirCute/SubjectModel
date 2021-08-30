using Bolt;
using UnityEngine;

namespace SubjectModel.Scripts.GUI
{
    /**
     * <summary>更新血条，体力条等（称为目标数据）条状GUI的脚本</summary>
     */
    [RequireComponent(typeof(RectTransform))]
    public class Bar : MonoBehaviour
    {
        public Variables sourceVariables; //目标数据所在的Variables
        public string source; //目标数据的名称
        public string sourceEnd; //目标数据最大值的名称
        public float targetEnd = 200f; //条状GUI的最大长度
        public float y = 15f; //条状GUI的宽度

        private void Update()
        {
            if (sourceVariables == null) return;
            var dec = sourceVariables.declarations;
            GetComponent<RectTransform>().sizeDelta = //修改GUI条长度
                new Vector2(Utils.Map(.0f, dec.Get<float>(sourceEnd), .0f, targetEnd, dec.Get<float>(source)), y);
        }

        private void OnDisable()
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(0f, y);
        }
    }
}