using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Vector2 = System.Numerics.Vector2;

namespace UltimateNode.Editor
{
    public class UltimatePortView : Port
    {
        public event Action<UltimatePortView, Edge> OnConnected;
        public event Action<UltimatePortView, Edge> OnDisconnected;

        public VisualElement InputElement;

        protected UltimatePortView(Orientation portOrientation, Direction portDirection, Capacity portCapacity,
            Type type) : base(portOrientation, portDirection, portCapacity, type)
        {
        }

        public void SetDisplayInputElement(bool isActive)
        {
            if (InputElement != null)
            {
                InputElement.style.display = isActive ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        public override void Connect(Edge edge)
        {
            base.Connect(edge);
            this.OnConnected?.Invoke(this, edge);
            Debug.Log("OnConnected");
            SetDisplayInputElement(false);
        }

        public override void Disconnect(Edge edge)
        {
            base.Disconnect(edge);
            this.OnDisconnected?.Invoke(this, edge);
            Debug.Log("OnDisconnected");
            SetDisplayInputElement(true);
        }

        public new static UltimatePortView Create<TEdge>(
            Orientation orientation,
            Direction direction,
            Port.Capacity capacity,
            System.Type type)
            where TEdge : Edge, new()
        {
            CustomEdgeConnectorListener listener = new CustomEdgeConnectorListener();
            UltimatePortView ele = new UltimatePortView(orientation, direction, capacity, type)
            {
                m_EdgeConnector = (EdgeConnector)new EdgeConnector<TEdge>((IEdgeConnectorListener)listener)
            };
            ele.AddManipulator((IManipulator)ele.m_EdgeConnector);
            return ele;
        }


        public class CustomEdgeConnectorListener : IEdgeConnectorListener
        {
            private GraphViewChange m_GraphViewChange;
            private List<Edge> m_EdgesToCreate;
            private List<GraphElement> m_EdgesToDelete;

            public CustomEdgeConnectorListener()
            {
                this.m_EdgesToCreate = new List<Edge>();
                this.m_EdgesToDelete = new List<GraphElement>();
                this.m_GraphViewChange.edgesToCreate = this.m_EdgesToCreate;
            }

            public void OnDropOutsidePort(Edge edge, Vector2 position)
            {
            }

            public void OnDropOutsidePort(Edge edge, UnityEngine.Vector2 position)
            {
            }

            public void OnDrop(UnityEditor.Experimental.GraphView.GraphView graphView, Edge edge)
            {
                this.m_EdgesToCreate.Clear();
                this.m_EdgesToCreate.Add(edge);
                this.m_EdgesToDelete.Clear();
                if (edge.input.capacity == Port.Capacity.Single)
                {
                    foreach (Edge connection in edge.input.connections)
                    {
                        if (connection != edge)
                            this.m_EdgesToDelete.Add((GraphElement)connection);
                    }
                }

                if (edge.output.capacity == Port.Capacity.Single)
                {
                    foreach (Edge connection in edge.output.connections)
                    {
                        if (connection != edge)
                            this.m_EdgesToDelete.Add((GraphElement)connection);
                    }
                }

                if (this.m_EdgesToDelete.Count > 0)
                {
                    graphView.DeleteElements((IEnumerable<GraphElement>)this.m_EdgesToDelete);
                }

                List<Edge> edgesToCreate = this.m_EdgesToCreate;
                if (graphView.graphViewChanged != null)
                    edgesToCreate = graphView.graphViewChanged(this.m_GraphViewChange).edgesToCreate;
                foreach (Edge edge1 in edgesToCreate)
                {
                    if (graphView is UltimateGraphView u)
                    {
                        var inputPort = edge.input as UltimatePortView;
                        var outputPort = edge.output as UltimatePortView;
                        var inputPortNode = inputPort.node as UltimateNodeView;
                        var outputPortNode = outputPort.node as UltimateNodeView;
                        var ultimateEdgeData = new UltimateEdgeData()
                        {
                            InputNodeGUID = inputPortNode.NodeData.GUID,
                            OutputNodeGUID = outputPortNode.NodeData.GUID,
                            InputPortName = inputPort.portName,
                            OutputPortName = outputPort.portName,
                        };
                        u.AddEdgeData(ultimateEdgeData);
                        var ultimateEdgeView = edge as UltimateEdgeView;
                        ultimateEdgeView.Init(ultimateEdgeData);

                        graphView.AddElement((GraphElement)edge1);
                        inputPort.Connect(edge1);
                        outputPort.Connect(edge1);
                    }
                }
            }
        }
    }
}