using UnityEngine;

namespace UltimateNode
{
    [StaticNodeGroup]
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

    [StaticNodeGroup]
    public static class CustomDebug
    {
        public static void Log(FlowData inputFlow, object objectMsg)
        {
            Debug.Log(objectMsg);
        }
        public static void LogWarning(FlowData inputFlow, object objectMsg)
        {
            Debug.LogWarning(objectMsg);
        }
        public static void LogError(FlowData inputFlow, object objectMsg)
        {
            Debug.LogError(objectMsg);
        }
    }

}