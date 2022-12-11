using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltimateNode
{
    public class UltimateFlowProcess
    {
        private UltimateGraphData m_Data;

        public UltimateFlowProcess(UltimateGraphData p_GraphData)
        {
            m_Data = p_GraphData;
        }

        public void Play()
        {
            UltimateNodeData startNode = m_Data.Nodes.First(
                x => x.Name == nameof(FlowControl.OnStart));
            InvokeByProcess(startNode);
        }

        private void InvokeByProcess(UltimateNodeData targetNode)
        {
            // targetNode.Execute();

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

                foreach (var outputNodeData in needExeNode)
                {
                    var inputPortData = outputNodeData.PortData.FirstOrDefault(x =>
                        x.PortValueType == typeof(FlowData) && x.PortType == PortType.Input);

                    if (inputPortData != null)
                    {
                        if (inputPortData.OriginVal is FlowData originVal)
                        {
                            outputNodeData.Execute();
                        }
                    }
                    else
                    {
                        outputNodeData.Execute();
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

                needExeNode = cache.OrderBy(x => x.Position.position.y).ToList();
            }
        }
    }
}