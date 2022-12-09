using System;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateNode
{
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
    }

    /// <summary>
    /// Node Graph Data
    /// </summary>
    [System.Serializable]
    public class UltimateGraphData
    {
        public List<UltimateNodeData> Nodes;
        public List<EdgeData> Edges;
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