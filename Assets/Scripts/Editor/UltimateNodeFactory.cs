using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UltimateNode.Editor
{
    public static class UltimateNodeFactory
    {
        /// <summary>
        /// Load Graph View By Graph Data
        /// </summary>
        /// <param name="p_UltimateGraphView"></param>
        /// <param name="p_GraphData"></param>
        /// <param name="nodes"></param>
        /// <param name="edges"></param>
        public static void LoadGraph(UltimateGraphView p_UltimateGraphView, UltimateGraphData p_GraphData)
        {
            var nodes = new List<UltimateNodeView>();
           
            foreach (var nodeData in p_GraphData.Nodes)
            {
                var ultimateNodeView = p_UltimateGraphView.AddNodeView(nodeData);
                nodes.Add(ultimateNodeView);
            }

            foreach (var edgeData in p_GraphData.Edges)
            {
                p_UltimateGraphView.AddEdgeView(edgeData);
            }
        }

        /// <summary>
        /// Generate Base Node by Node Data
        /// Data => View
        /// </summary>
        /// <param name="nodeData"></param>
        /// <returns></returns>
        public static UltimateNodeView GenerateBaseNode(UltimateNodeData nodeData)
        {
            // Add Entry Node
            UltimateNodeView loadedNode = new UltimateNodeView(nodeData)
            {
                title = nodeData.Name,
            };
            loadedNode.SetPosition(nodeData.Position);

            for (var i = 0; i < nodeData.PortData.Count; i++)
            {
                var portData = nodeData.PortData[i];
                UltimatePortView newPort = null;
                var portValType = portData.PortValueType;
                if (portData.PortType == PortType.Input)
                {
                    newPort = loadedNode.AddInput(
                        portData.PortName,
                        (Orientation)portData.Orientation,
                        (Port.Capacity)portData.Capacity,
                        portValType);
                }
                else if (portData.PortType == PortType.Output)
                {
                    newPort = loadedNode.AddOutput(
                        portData.PortName,
                        (Orientation)portData.Orientation,
                        (Port.Capacity)portData.Capacity,
                        portValType);
                }

                if (newPort == null)
                {
                    Debug.LogError($"No Such Port Type:{portData.PortType}");
                }

                if (portData.IsHide)
                {
                    newPort.style.display = DisplayStyle.None;
                }

                var fieldInfo = portData.GetType().GetField(nameof(portData.OriginVal));
                VisualElement element = null;

                if (portData.OriginVal == null)
                {
                    Debug.Log("Null Port Origin Val");
                }
                else if (portData.PortType == PortType.Output)
                {
                }
                else if (portValType == typeof(bool))
                {
                    var t = new Toggle() { value = (bool)portData.OriginVal };
                    t.RegisterValueChangedCallback(x => { fieldInfo.SetValue(portData, x.newValue); });
                    element = t;
                }
                else if (portValType == typeof(int))
                {
                    var t = new IntegerField() { value = (int)portData.OriginVal };
                    t.RegisterValueChangedCallback(x => { fieldInfo.SetValue(portData, x.newValue); });
                    element = t;
                }
                else if (portValType == typeof(uint))
                {
                    var t = new IntegerField() { value = (int)portData.OriginVal };
                    t.RegisterValueChangedCallback(x => { fieldInfo.SetValue(portData, x.newValue); });
                    element = t;
                }
                else if (portValType == typeof(float))
                {
                    var t = new FloatField() { value = (float)portData.OriginVal };
                    t.RegisterValueChangedCallback(x => { fieldInfo.SetValue(portData, x.newValue); });
                    element = t;
                }
                else if (portValType == typeof(Color))
                {
                    var t = new ColorField() { value = (Color)portData.OriginVal };
                    t.RegisterValueChangedCallback(x => { fieldInfo.SetValue(portData, x.newValue); });
                    element = t;
                }
                else if (portValType == typeof(Vector4))
                {
                    var t = new Vector4Field() { value = (Vector4)portData.OriginVal };
                    t.RegisterValueChangedCallback(x => { fieldInfo.SetValue(portData, x.newValue); });
                    element = t;
                }
                else if (portValType == typeof(Vector3))
                {
                    var t = new Vector3Field() { value = (Vector3)portData.OriginVal };
                    t.RegisterValueChangedCallback(x => { fieldInfo.SetValue(portData, x.newValue); });
                    element = t;
                }
                else if (portValType == typeof(Vector2))
                {
                    var t = new Vector2Field() { value = (Vector2)portData.OriginVal };
                    t.RegisterValueChangedCallback(x => { fieldInfo.SetValue(portData, x.newValue); });
                    element = t;
                }
                else if (portValType == typeof(FlowData))
                {
                }
                else
                {
                    Debug.LogError($"Parse Error Type:{portData.OriginVal.GetType()}");
                }

                newPort.InputElement = element;
                if (element != null)
                {
                    element.style.maxWidth = 100;
                    newPort.Add(element);
                }
            }

            loadedNode.Refresh();


            return loadedNode;
        }

        /// <summary>
        /// Generate Edge By Edge Data
        /// Data => View
        /// </summary>
        /// <param name="allNodes"></param>
        /// <param name="p_UltimateEdgeData"></param>
        /// <returns></returns>
        public static UltimateEdgeView GenerateEdge(List<UltimateNodeView> allNodes,
            UltimateEdgeData p_UltimateEdgeData)
        {
            UltimateNodeView inputNode =
                allNodes.FirstOrDefault(x => x.NodeData.GUID == p_UltimateEdgeData.InputNodeGUID);
            UltimateNodeView outputNode =
                allNodes.FirstOrDefault(x => x.NodeData.GUID == p_UltimateEdgeData.OutputNodeGUID);

            if (inputNode != null && outputNode != null)
            {
                var inputPort = inputNode.GetInputPort(p_UltimateEdgeData.InputPortName);
                var outputPort = outputNode.GetOutputPort(p_UltimateEdgeData.OutputPortName);
                var edge = new UltimateEdgeView(p_UltimateEdgeData)
                {
                    input = inputPort,
                    output = outputPort,
                };
                inputPort.Connect(edge);
                outputPort.Connect(edge);

                return edge;
            }
            else
            {
                if (inputNode == null)
                {
                    Debug.LogError($"Not Found Edge Connect Input Target:{p_UltimateEdgeData.InputNodeGUID}");
                }

                if (outputNode == null)
                {
                    Debug.LogError($"Not Found Edge Connect Output Target:{p_UltimateEdgeData.OutputPortName}");
                }
            }

            return null;
        }
    }
}