using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UltimateNode.Editor
{
    public static class UltimateNodeFactory
    {
        public static void LoadGraph(UltimateGraphData graphData,
            out List<UltimateNodeBase> nodes, out List<Edge> edges, out List<Group> groups)
        {
            nodes = new List<UltimateNodeBase>();
            edges = new List<Edge>();
            groups = new List<Group>();

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

            for (var i = 0; i < graphData.Groups.Count; i++)
            {
                var group = graphData.Groups[i];
                var loadGroup = LoadGroup(group);
                groups.Add(loadGroup);
            }
        }

        /// <summary>
        /// Generate Node By Method 
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static UltimateNodeBase GenerateBaseNode(MethodInfo methodInfo, Vector2 position)
        {
            // Init Node Data By Method Info 
            UltimateNodeData nodeData = new UltimateNodeData(methodInfo)
            {
                GUID = Guid.NewGuid().ToString(),
            };
            UltimateNodeBase ultimateNodeBase = LoadBaseNode(nodeData);
            ultimateNodeBase.SetPosition(position);
            return ultimateNodeBase;
        }


        /// <summary>
        /// Load Base Node By Node Data
        /// </summary>
        /// <param name="nodeData"></param>
        /// <returns></returns>
        public static UltimateNodeBase LoadBaseNode(UltimateNodeData nodeData)
        {
            // Add Entry Node
            UltimateNodeBase loadedNode = new UltimateNodeBase(nodeData)
            {
                title = nodeData.Name,
                GUID = nodeData.GUID
            };
            loadedNode.SetPosition(nodeData.Position);

            // loadedNode.portData = new Dictionary<string, MemberInfo>();
            for (var i = 0; i < nodeData.PortData.Count; i++)
            {
                var portData = nodeData.PortData[i];
                Port newPort = null;
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

                var fieldInfo = portData.GetType().GetField(nameof(portData.OriginVal));
                VisualElement element = null;

                if (portData.OriginVal == null)
                {
                    Debug.Log("Null Port Origin Val");
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
                else
                {
                    Debug.LogError($"Parse Error Type:{portData.OriginVal.GetType()}");
                }

                if (element != null)
                {
                    element.style.maxWidth = 100;
                    newPort.Add(element);
                }
            }

            loadedNode.Refresh();


            return loadedNode;
        }


        public static Group LoadGroup(UltimateGroupData groupData)
        {
            Group g = new Group()
            {
                name = groupData.Name,
                autoUpdateGeometry = true,
            };
            g.SetPosition(groupData.Position);
            return g;
        }
    }
}