using System;
using UnityEngine;

namespace UltimateNode
{
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
            o.ReturnValue = false;
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

        
        public void MoveAss(FlowData flowData,TestAI ai, ref TestAI mySelf)
        {
            mySelf = ai;
            ai.transform.position += Vector3.forward * 5;
        }
    }
}