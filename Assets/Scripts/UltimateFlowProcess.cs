using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using YogiTools;

namespace UltimateNode
{
    public class UltimateFlowProcess
    {
        public bool IsDebug = true;
        private UltimateGraphData m_Data;

        public UltimateFlowProcess(UltimateGraphData p_GraphData)
        {
            m_Data = p_GraphData;
        }

        public async void PlayFlow(object owner, string startNodeName)
        {
            UltimateNodeData startNode = m_Data.Nodes.FirstOrDefault(x => x.Name == startNodeName);
            if (startNode != null)
            {
                await InvokeOnStartByProcess(owner, startNode);
            }
        }

        public async void PlayAIFlow(object owner)
        {
            var aiThinkNode = m_Data.Nodes.FirstOrDefault(x => x.Name == nameof(AI.OnAIThink));
            if (aiThinkNode != null)
            {
                await InvokeOnAIThinkByProcess(owner, aiThinkNode, new AIFlowData());
            }
        }

        public void Play(object owner)
        {
            PlayFlow(owner, nameof(FlowControl.OnStart));

            PlayAIFlow(owner);
        }

        private async Task InvokeOnStartByProcess(object p_Owner, UltimateNodeData targetNode)
        {
            List<UltimateNodeData> needExeNode = new List<UltimateNodeData>();
            needExeNode.Add(targetNode);

            // Get Start Node's pair input Connection Nodes
            int loopCount = 10000;
            while (needExeNode.Count > 0)
            {
                loopCount--;
                if (loopCount < 0)
                {
                    Debug.LogError("Infinity Loop!");
                    return;
                }

                List<UltimateNodeData> cache = new List<UltimateNodeData>();

                foreach (UltimateNodeData outputNodeData in needExeNode)
                {
                    PortData inputPortData = null;

                    inputPortData = outputNodeData.PortData.FirstOrDefault(x =>
                        IsTypeOrSubclass(x.PortValueType, typeof(FlowData)) && x.PortType == PortType.Input);

                    // Execute All Connection PreNode
                    async Task ExecutePreNode(UltimateNodeData t_CurrentNode, int limit = 1000)
                    {
                        limit--;
                        if (limit < 0)
                        {
                            Debug.LogError("Loop ExecutePreNode");
                            return;
                        }

                        var preNodeGUIDs = m_Data.Edges.Where(x =>
                                {
                                    if (x.InputNodeGUID == t_CurrentNode.GUID)
                                    {
                                        var connectInputPort =
                                            t_CurrentNode.PortData.FirstOrDefault(port =>
                                                port.PortName == x.InputPortName);
                                        if (IsTypeOrSubclass(connectInputPort.PortValueType, typeof(FlowData)))
                                        {
                                            return false;
                                        }
                                        else
                                        {
                                            return true;
                                        }
                                    }

                                    return false;
                                }
                                // && !IsTypeOrSubclass(t_CurrentNode.PortData.FirstOrDefault(port=>port.PortName==x.InputPortName).PortValueType,typeof(FlowData)) 
                            )
                            .Select(x => x.OutputNodeGUID).ToList();

                        foreach (var ultimateNodeData in m_Data.Nodes)
                        {
                            if (preNodeGUIDs.Contains(ultimateNodeData.GUID))
                            {
                                // preNode
                                var preNode = ultimateNodeData;
                                await ExecutePreNode(preNode);
                                DebugOnEnterNode(preNode);
                                await preNode.Execute(p_Owner);
                                DebugOnExitNode(preNode);

                                foreach (var edgeData in m_Data.Edges)
                                {
                                    // Match this node(OutputNode) connected inputNode
                                    if (edgeData.OutputNodeGUID == preNode.GUID)
                                    {
                                        var inputNode = m_Data.Nodes.First(x => x.GUID == edgeData.InputNodeGUID);

                                        PortData outputPort = null, inputPort = null;
                                        for (var i = 0; i < preNode.PortData.Count; i++)
                                        {
                                            var portData = preNode.PortData[i];
                                            if (edgeData.OutputPortName == portData.PortName)
                                            {
                                                outputPort = portData;
                                                break;
                                            }
                                        }

                                        for (var i = 0; i < inputNode.PortData.Count; i++)
                                        {
                                            var portData = inputNode.PortData[i];
                                            if (edgeData.InputPortName == portData.PortName)
                                            {
                                                inputPort = portData;
                                                break;
                                            }
                                        }

                                        if (inputPort != null && outputPort != null)
                                        {
                                            inputPort.OriginVal = outputPort.OriginVal;
                                        }
                                    }
                                }
                            }
                        }
                    }


                    //Execute FlowData Connection  PreNode 
                    if (inputPortData != null)
                    {
                        if (inputPortData.OriginVal is FlowData originVal)
                        {
                            await ExecutePreNode(outputNodeData);

                            DebugOnEnterNode(outputNodeData);
                            await outputNodeData.Execute(p_Owner);
                            DebugOnExitNode(outputNodeData);
                        }
                    }
                    else
                    {
                        await ExecutePreNode(outputNodeData);
                        DebugOnEnterNode(outputNodeData);
                        await outputNodeData.Execute(p_Owner);
                        DebugOnExitNode(outputNodeData);
                    }

                    foreach (var edgeData in m_Data.Edges)
                    {
                        // Match this node(OutputNode) connected inputNode
                        if (edgeData.OutputNodeGUID == outputNodeData.GUID)
                        {
                            var inputNode = m_Data.Nodes.First(x => x.GUID == edgeData.InputNodeGUID);

                            PortData outputPort = null, inputPort = null;
                            for (var i = 0; i < outputNodeData.PortData.Count; i++)
                            {
                                var portData = outputNodeData.PortData[i];
                                if (edgeData.OutputPortName == portData.PortName)
                                {
                                    outputPort = portData;
                                    break;
                                }
                            }

                            for (var i = 0; i < inputNode.PortData.Count; i++)
                            {
                                var portData = inputNode.PortData[i];
                                if (edgeData.InputPortName == portData.PortName)
                                {
                                    inputPort = portData;
                                    break;
                                }
                            }

                            if (inputPort != null && outputPort != null)
                            {
                                inputPort.OriginVal = outputPort.OriginVal;
                            }


                            if (!cache.Contains(inputNode))
                            {
                                cache.Add(inputNode);
                            }
                        }
                    }
                }

                needExeNode = OrderNodes(cache);
            }
        }

        private async Task<bool> InvokeOnAIThinkByProcess(object p_Owner, UltimateNodeData currentNode,
            AIFlowData p_FlowData)
        {
            var outputNodeData = currentNode;
            //1. Check Child Node Return Value, If it's True, return

            // 1. Get Output Port
            var outputPort = outputNodeData.PortData.FirstOrDefault(x =>
                IsTypeOrSubclass(x.PortValueType, typeof(AIFlowData)) && x.PortType == PortType.Output);
            Debug.Log(outputPort.PortName);

            // 2. Get Connection Input Nodes
            var connectionNodes = new List<UltimateNodeData>();
            foreach (var edgeData in m_Data.Edges)
            {
                if (edgeData.OutputNodeGUID == outputNodeData.GUID)
                {
                    var inputNode = m_Data.GetNodeByGUID(edgeData.InputNodeGUID);
                    connectionNodes.Add(inputNode);
                }
            }

            connectionNodes = OrderNodes(connectionNodes);

            for (var i = 0; i < connectionNodes.Count; i++)
            {
                var node = connectionNodes[i];

                // Modify Node Not Execute
                if (node.Name == nameof(AI.Sequence) || node.Name == nameof(AI.Select))
                {
                    var returnVal = await InvokeOnAIThinkByProcess(p_Owner, node, p_FlowData);
                    if (currentNode.Name == nameof(AI.Select))
                    {
                        if (returnVal == false)
                        {
                            continue;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if(currentNode.Name == nameof(AI.Sequence))
                    {
                        if (returnVal == false)
                        {
                            return false;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    var actionOrConditionOutputPort = node.PortData.FirstOrDefault(x =>
                        IsTypeOrSubclass(x.PortValueType, typeof(AIFlowData)) && x.PortType == PortType.Output);
                    actionOrConditionOutputPort.OriginVal = p_FlowData;
                    DebugOnEnterNode(node);
                    await node.Execute(p_Owner);
                    DebugOnExitNode(node);

                    if (currentNode.Name == nameof(AI.Sequence))
                    {
                        // Sequence
                        if (p_FlowData.ReturnValue)
                        {
                            //Continue
                        }
                        else
                        {
                            //Break
                            return false;
                        }
                    }
                    else if (currentNode.Name == nameof(AI.Select))
                    {
                        // Select Break
                        if (p_FlowData.ReturnValue)
                        {
                            //Break
                            return false;
                        }
                        else
                        {
                            //Continue
                        }
                    }
                    else
                    {
                        return p_FlowData.ReturnValue;
                    }
                }
            }

            if (currentNode.Name == nameof(AI.Sequence))
            {
                return true;
            }
            else if (currentNode.Name == nameof(AI.Select))
            {
                return false;
            }

            return false;

            var currentOutputPort = outputNodeData.PortData.FirstOrDefault(x =>
                IsTypeOrSubclass(x.PortValueType, typeof(AIFlowData)) && x.PortType == PortType.Output);
            currentOutputPort.OriginVal = p_FlowData;
            DebugOnEnterNode(outputNodeData);
            await outputNodeData.Execute(p_Owner);
            DebugOnExitNode(outputNodeData);
            return p_FlowData.ReturnValue;
        }

        private static List<UltimateNodeData> OrderNodes(List<UltimateNodeData> connectionNodes)
        {
            return connectionNodes.OrderBy(x => x.Position.position.y).ToList();
        }

        private static bool IsTypeOrSubclass(Type a, Type b)
        {
            if (a == b || a.IsSubclassOf(b))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void DebugOnEnterNode(UltimateNodeData p_NodeData)
        {
            if (IsDebug)
            {
                EventManager.Instance.EventTrigger(UltimateGraphEventConst.OnEnterExecuteNode, p_NodeData);
            }
        }

        private void DebugOnExitNode(UltimateNodeData p_NodeData)
        {
            if (IsDebug)
            {
                EventManager.Instance.EventTrigger(UltimateGraphEventConst.OnExitExecuteNode, p_NodeData);
            }
        }
    }
}