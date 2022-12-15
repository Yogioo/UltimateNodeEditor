using System;
using System.Threading.Tasks;
using UnityEngine;

namespace UltimateNode
{
    [NodeGroup("Test AI")]
    public class TestAI : UltimateGraphEntity
    {
        #region Example

        public static void ConditionHPRange(AIFlowData i, float min, float max, [HidePort] ref AIFlowData o)
        {
            Debug.Log(min);
            float playerHP = 0.5f;
            var condition = min < playerHP && playerHP < max;
            o.ReturnValue = condition;
        }

        public static void ActionEat(AIFlowData i, [HidePort] ref AIFlowData o)
        {
            o.ReturnValue = true;
            Debug.Log("Player Eating");
        }

        public static void ActionWalk(AIFlowData i, [HidePort] ref AIFlowData o)
        {
            o.ReturnValue = true;
            Debug.Log("Player Walking False");
        }

        #endregion

        [ContextMenu("Test")]
        public void Test()
        {
        }

        public void GetThis(ref TestAI ai)
        {
            ai = this;
        }

        public void MoveAss(FlowData flowData, TestAI ai, ref TestAI mySelf)
        {
            mySelf = ai;
            ai.transform.position += Vector3.forward * 5;
        }

        public async void DelayToDebugLog(FlowData p_FlowData, float delayTime, object msg)
        {
            await Task.Delay(TimeSpan.FromSeconds(delayTime));
            Debug.Log(msg);
        }

        public void MoveTo(FlowData i, Vector4 targetPosition)
        {
            this.transform.position += (Vector3)targetPosition;
        }
        public async void TestAnimationCurve(FlowData i, AnimationCurve p_AnimationCurve)
        {
            float time = 0;
            float endTime = 5;

            while (time < endTime)
            {
                time += Time.deltaTime;
                await Task.Yield();
                var f = p_AnimationCurve.Evaluate(time/ endTime) ;
                Debug.Log(f);
            }
        }
    }
}