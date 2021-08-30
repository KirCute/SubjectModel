using System;
using Bolt;
using UnityEngine;

namespace SubjectModel.Scripts.System
{
    /**
     * <summary>
     * 移动脚本
     * 负责将物体动机与Variables中的数值结合确定最终的刚体速度。
     * 注意：所有通过2D刚体实现移动的作战单位都应通过此脚本间接控制刚体速度。
     * </summary>
     */
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Variables))]
    public class Movement : MonoBehaviour
    {
        [NonSerialized] public Vector2 Motivation = Vector2.zero; //动机，其它脚本希望物体的移动（方向）

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