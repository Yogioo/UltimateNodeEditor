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
        public static void Select(FlowData i, [MultiPort] out FlowData o)
        {
            o = i;
        }

        public static void ConditionHPRange(FlowData i, float min, float max, out FlowData True,
            out FlowData False)
        {
            Debug.Log(min);
            float playerHP = 0.5f;
            True = False = null;
            var condition = min < playerHP && playerHP < max;
            if (condition)
            {
                True = i;
            }
            else
            {
                False = i;
            }
        }

        public static void ActionEat(FlowData i)
        {
            Debug.Log("Player Eating");
        }

        public static void ActionWalk(FlowData i)
        {
            Debug.Log("Player Walking");
        }
    }
}