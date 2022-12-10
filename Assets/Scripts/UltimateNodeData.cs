using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UltimateNode
{
    [System.Serializable]
    public class UltimateGroupData
    {
        public string Name;
        public string GUID;
        public Rect Position;
    }

    /// <summary>
    /// Node Data
    /// </summary>
    [System.Serializable]
    public class UltimateNodeData
    {
        public string Name;
        public string GUID;
        public Rect Position;
        public List<PortData> PortData;

        public UltimateNodeData()
        {
            PortData = new List<PortData>();
        }

        public UltimateNodeData(MethodInfo methodInfo) : this()
        {
            Name = methodInfo.Name;

            // Input Port
            var parameterInfos = methodInfo.GetParameters();
            for (var i = 0; i < parameterInfos.Length; i++)
            {
                var parameterInfo = parameterInfos[i];
                this.PortData.Add(new PortData()
                {
                    Capacity = 0,
                    Orientation = 0,
                    PortName = parameterInfo.Name,
                    PortType = PortType.Input,
                    PortValueType = parameterInfo.ParameterType,
                    OriginVal = parameterInfo.ParameterType.IsValueType
                        ? Activator.CreateInstance(parameterInfo.ParameterType)
                        : null,
                });
            }

            // Output Port
            if ((methodInfo.ReturnParameter.ParameterType != typeof(void)))
            {
                var returnType = methodInfo.ReturnType;
                this.PortData.Add(new PortData()
                {
                    Capacity = 0,
                    Orientation = 0,
                    PortName = methodInfo.ReturnParameter.Name,
                    PortType = PortType.Output,
                    PortValueType = methodInfo.ReturnType,
                    OriginVal = methodInfo.ReturnType.IsValueType
                        ? Activator.CreateInstance(methodInfo.ReturnType)
                        : null,
                });
            }
        }
    }

    /// <summary>
    /// Node Graph Data
    /// </summary>
    [System.Serializable]
    public class UltimateGraphData
    {
        public List<UltimateNodeData> Nodes;
        public List<UltimateGroupData> Groups; 
        public List<EdgeData> Edges;

        public UltimateGraphData()
        {
            Nodes = new List<UltimateNodeData>();
            Edges = new List<EdgeData>();
        }
    }

    [System.Serializable]
    public class PortData
    {
        public string PortName;
        public Type PortValueType;

        /// <summary>
        /// 0 is Input, 1 is Output
        /// </summary>
        public PortType PortType;

        /// <summary>
        /// Port Connect Direction
        /// </summary>
        public int Orientation;

        /// <summary>
        /// if equal 1 Port be able to connect multi edge
        /// </summary>
        public int Capacity;

        /// <summary>
        /// Port Data
        /// </summary>
        public object OriginVal;
    }

    [System.Serializable]
    public class EdgeData
    {
        public string InputNodeGUID;
        public string OutputNodeGUID;
        public string InputPortName;
        public string OutputPortName;
    }

    public enum PortType
    {
        None,
        Input,
        Output
    }
}