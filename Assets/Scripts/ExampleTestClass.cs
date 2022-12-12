using UnityEngine;

namespace UltimateNode
{
    [NodeGroup]
    public class CustomCalc
    {
        public static void Add(FlowData inputFlow, out FlowData outputFlow, float a, float b, out float result,
            ref float testR,
            Transform targetTrans)
        {
            outputFlow = inputFlow;
            result = a + b;
        }

        public static void Multiply(float a, float b, out float c)
        {
            c = a * b;
        }

        public static void DelayToMove(Transform target, float delayTime, Vector3 offset)
        {
        }
    }

    [NodeGroup]
    public static class CustomDebug
    {
        public static void Log(FlowData inputFlow, float logInfo)
        {
            Debug.Log(logInfo);
        }
    }

    [NodeGroup]
    public static class AI
    {
        public static void OnAIThink([MultiPort] ref AIFlowData o)
        {
            o = new AIFlowData();
        }

        public static void Select(AIFlowData i, [MultiPort] ref AIFlowData o)
        {
            o = i;
        }

        public static void Sequence(AIFlowData i, [MultiPort] ref AIFlowData o)
        {
            o = i;
        }

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
    }
}