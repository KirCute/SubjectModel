using System;
using Bolt;
using SubjectModel.Scripts.Event;
using UnityEngine;

namespace SubjectModel.Scripts.System
{
    /**
     * <summary>
     * 生理系统
     * 负责处理EntityDead事件触发，精神自然消耗（暂未添加），体力消耗与回复，体力存量对奔跑速度，以及其它生理性质的数值维护
     * 原则上所有肉身作战单位均需挂载此脚本
     * </summary>
     */
    [RequireComponent(typeof(Variables))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Physiology : MonoBehaviour
    {
        private const float RecoverSpeed = 30f;
        private const float CostWhenRunning = 50f;
        private const float RunnableStrength = 20f;
        private VariableDeclarations dec;
        private float runSpeed; //用于保存初始RunSpeed以在体力恢复后同步恢复RunSpeed
        private bool dying; //是否濒死，用于保证EntityDead事件只触发一次
        public bool removeAfterDead; //是否在死亡后自动销毁

        private void Start()
        {
            dec = GetComponent<Variables>().declarations;
            runSpeed = dec.IsDefined("RunSpeed") ? dec.Get<float>("RunSpeed") : 1.0f;
        }

        private void Update()
        {
            if (dec.Get<float>("Health") <= 0f || dec.Get<float>("Energy") <= 0f) //判断是否死亡
            {
                if (!dying)
                {
                    dying = true;
                    EventDispatchers.EdeDispatcher.DispatchEvent(gameObject);
                    if (removeAfterDead) Destroy(gameObject);
                }
            }
            else dying = false;

            dec.Set("Health", Math.Min(dec.Get<float>("Health"), dec.Get<float>("MaxHealth"))); //保证血量低于血量上限
            if (dec.IsDefined("Defence")) dec.Set("Defence", Math.Max(dec.Get<float>("Defence"), 0f)); //保证防御大于0

            //处理体力消耗
            var strength = dec.Get<float>("Strength");
            var max = dec.Get<float>("Energy");
            strength += RecoverSpeed * Time.deltaTime;
            if (dec.IsDefined("Running")) //奔跑体力消耗
            {
                var speed = strength >= RunnableStrength ? runSpeed : 1.0f;
                dec.Set("RunSpeed", speed);
                if (dec.Get<bool>("Running") && speed > 1.0f &&
                    GetComponent<Rigidbody2D>().velocity != Vector2.zero) strength -= CostWhenRunning * Time.deltaTime;
            }

            strength = Math.Min(strength, max); //保证体力低于精神
            //提交结算结果
            dec.Set("Strength", strength);
            dec.Set("Energy", max);
        }

        private void OnDestroy()
        {
            EventDispatchers.EdeDispatcher.ClearEventListener(gameObject);
        }
    }
}