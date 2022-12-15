using System;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateNode
{
    [NodeGroup]
    public class Calc
    {
        public static void Add(float a, float b, out float r)
        {
            r = a + b;
        }

        public static void Multiply(float a, float b, out float r)
        {
            r = a * b;
        }

        public static void Subtract(float a, float b, out float r)
        {
            r = a - b;
        }

        public static void Divide(float a, float b, out float r)
        {
            r = a / b;
        }
    }

    [NodeGroup]
    public static class UnityDebug
    {
        public static void Log(FlowData i, object objectMsg)
        {
            Debug.Log(objectMsg);
        }

        public static void LogWarning(FlowData i, object objectMsg)
        {
            Debug.LogWarning(objectMsg);
        }

        public static void LogError(FlowData i, object objectMsg)
        {
            Debug.LogError(objectMsg);
        }
    }

    [NodeGroup]
    public static class Variable
    {
        public static void InputString(string input, out string output)
        {
            output = input;
        }

        public static void InputBoolean(bool input, out bool output)
        {
            output = input;
        }

        public static void InputFloat(float input, out float output)
        {
            output = input;
        }

        public static void InputInt(int input, out int output)
        {
            output = input;
        }

        public static void InputVector4(Vector4 input, out Vector4 output)
        {
            output = input;
        }

        public static void InputVector3(Vector3 input, out Vector3 output)
        {
            output = input;
        }

        public static void InputVector2(Vector2 input, out Vector2 output)
        {
            output = input;
        }

        public static void BreakVector4(Vector4 i, out float x, out float y, out float z, out float w)
        {
            x = i.x;
            y = i.y;
            z = i.z;
            w = i.w;
        }

        public static void BreakVector3(Vector3 i, out float x, out float y, out float z)
        {
            x = i.x;
            y = i.y;
            z = i.z;
        }

        public static void BreakVector2(Vector2 i, out float x, out float y)
        {
            x = i.x;
            y = i.y;
        }

        public static void MergeVector4(float x, float y, float z, float w, out Vector4 output)
        {
            output = new Vector4(x, y, z, w);
        }

        public static void MergeVector3(float x, float y, float z, out Vector3 output)
        {
            output = new Vector3(x, y, z);
        }

        public static void MergeVector2(float x, float y, out Vector3 output)
        {
            output = new Vector2(x, y);
        }
    }

    [NodeGroup]
    public static class Logic
    {
        public static void If(FlowData i, bool input, out FlowData True, out FlowData False)
        {
            False = True = null;
            if (input)
            {
                True = i;
            }
            else
            {
                False = i;
            }
        }

        #region Not Recommand

        // // 1. Input Value type : object
        // // 2. Compare Function  CompareEqual
        // // 3. Compare Value List<object>
        // // 4. Select Target : FlowData
        // public static void Switch(FlowData i, object inputValue,Func<object,object,bool> compareFunction,List<object> compareValueArr, out List<FlowData> True)
        // {
        //     True = new List<FlowData>(compareValueArr.Count);
        //     for (var i1 = 0; i1 < compareValueArr.Count; i1++)
        //     {
        //         if (compareFunction?.Invoke(inputValue, compareValueArr[i1]) == true)
        //         {
        //             True[i1] = i;
        //         }
        //     }
        // }

        // public static void For(FlowData i, List<object> arr, out FlowData o)
        // {
        //     for (var i1 = 0; i1 < arr.Count; i1++)
        //     {
        //         
        //     }
        // }

        //foreach

        #endregion
    }
}