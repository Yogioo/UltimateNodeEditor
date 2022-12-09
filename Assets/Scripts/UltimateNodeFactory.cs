using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace UltimateNode
{
    public static class UltimateNodeFactory
    {
        public static void LoadGraph(UltimateGraphData graphData,
            out List<UltimateNodeBase> nodes, out List<Edge> edges)
        {
            nodes = new List<UltimateNodeBase>();
            edges = new List<Edge>();

            for (var i = 0; i < graphData.Nodes.Count; i++)
            {
                var ultimateNodeBase = LoadBaseNode(graphData.Nodes[i]);
                nodes.Add(ultimateNodeBase);
            }

            for (var i = 0; i < graphData.Edges.Count; i++)
            {
                var edgeData = graphData.Edges[i];
                UltimateNodeBase inputNode = nodes.FirstOrDefault(x => x.GUID == edgeData.InputNodeGUID);
                UltimateNodeBase outputNode = nodes.FirstOrDefault(x => x.GUID == edgeData.OutputNodeGUID);

                if (inputNode != null && outputNode != null)
                {
                    var edge = inputNode.ConnectInput(outputNode, edgeData.InputPortName, edgeData.OutputPortName);
                    edges.Add(edge);
                }
                else
                {
                    if (inputNode == null)
                    {
                        Debug.LogError($"Not Found Edge Connect Input Target:{edgeData.InputNodeGUID}");
                    }

                    if (outputNode == null)
                    {
                        Debug.LogError($"Not Found Edge Connect Output Target:{edgeData.OutputPortName}");
                    }
                }
            }
        }

        /// <summary>
        /// Generate Base Node 
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private static UltimateNodeBase LoadBaseNode(UltimateNodeData nodeData)
        {
            // Add Entry Node
            UltimateNodeBase loadedNode = new UltimateNodeBase
            {
                title = nodeData.Name,
                GUID = nodeData.GUID
            };
            loadedNode.SetPosition(nodeData.Position);

            loadedNode.portData = new Dictionary<string, object>();
            for (var i = 0; i < nodeData.PortData.Count; i++)
            {
                var portData = nodeData.PortData[i];
                if (portData.PortType == PortType.Input)
                {
                    loadedNode.AddInput(
                        portData.PortName,
                        (Orientation)portData.Orientation,
                        (Port.Capacity)portData.Capacity,
                        portData.PortValueType);
                }
                else if (portData.PortType == PortType.Output)
                {
                    loadedNode.AddOutput(
                        portData.PortName,
                        (Orientation)portData.Orientation,
                        (Port.Capacity)portData.Capacity,
                        portData.PortValueType);
                }

                loadedNode.portData.Add(portData.PortName, portData.OriginVal);
            }


            return loadedNode;
        }
    }
}