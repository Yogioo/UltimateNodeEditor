using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace UltimateNode.Editor
{
    public class UltimateNodeView : Node
    {
        public UltimateNodeData NodeData;
        public event Action<UltimatePortView, Edge> OnPortConnectionChange;
        public event Action<UltimateNodeView> OnDisconnectAllPorts,OnDisconnectInputPorts,OnDisconnectOutputPorts;

        public UltimateNodeView(UltimateNodeData nodeData) : base()
        {
            this.NodeData = nodeData;

            Button btn = new Button(() =>
            {
                AddOutput("Test", Orientation.Horizontal, Port.Capacity.Single, typeof(Rect));
                this.RefreshExpandedState();
                this.RefreshPorts();
            }) { text = "+" };
            this.titleContainer.Add(btn);
        }


        public override void UpdatePresenterPosition()
        {
            this.NodeData.Position = this.GetPosition();
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            this.NodeData.Position = newPos;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            // Customize Disconnect all 
            // base.BuildContextualMenu(evt);
            evt.menu.AppendAction("Disconnect All Ports", p_Action => { OnDisconnectAllPorts?.Invoke(this); });
            evt.menu.AppendAction("Disconnect Input Ports", p_Action => { OnDisconnectInputPorts?.Invoke(this); });
            evt.menu.AppendAction("Disconnect Output Ports", p_Action => { OnDisconnectOutputPorts?.Invoke(this); });
        }

        public UltimatePortView AddOutput(
            string portName,
            Orientation orientation,
            Port.Capacity capacity,
            System.Type type
        )
        {
            UltimatePortView port =
                UltimatePortView.Create<UltimateEdgeView>(orientation, Direction.Output, capacity, type);
            port.source = this;
            port.name = portName;
            port.portName = portName;
            outputContainer.Add(port);
            RegisterPortCallback(port);
            return port;
        }

        public UltimatePortView AddInput(
            string portName,
            Orientation orientation,
            Port.Capacity capacity,
            System.Type type)
        {
            UltimatePortView port =
                UltimatePortView.Create<UltimateEdgeView>(orientation, Direction.Input, capacity, type);
            port.source = this;
            port.name = portName;
            port.portName = portName;
            inputContainer.Add(port);
            RegisterPortCallback(port);
            return port;
        }

        private void RegisterPortCallback(UltimatePortView p_Port)
        {
            p_Port.OnConnected += OnPortConnectionChange;
            p_Port.OnDisconnected += OnPortConnectionChange;
        }

        public void SetPosition(Vector2 position)
        {
            this.SetPosition(new Rect(position, Vector2.zero));
        }

        public UltimatePortView GetInputPort(string portName)
        {
            var port = this.inputContainer.Q<UltimatePortView>(portName);
            if (port == null)
            {
                Debug.LogError($"this Node :{this.name} Get Input Port Name Error:{portName}, No such Port");
            }

            return port;
        }

        public UltimatePortView GetOutputPort(string portName)
        {
            var port = this.outputContainer.Q<UltimatePortView>(portName);
            if (port == null)
            {
                Debug.LogError($"this Node :{this.name} Get Output Port Name Error:{portName}, No such Port");
            }

            return port;
        }

        public void ChangeInputPortColor(string portName, Color portColor)
        {
            GetInputPort(portName).portColor = portColor;
        }

        public void ChangeOutputPortColor(string portName, Color portColor)
        {
            GetOutputPort(portName).portColor = portColor;
        }

        /// <summary>
        /// Refresh Ports And Expand Style
        /// </summary>
        public void Refresh()
        {
            this.RefreshExpandedState();
            this.RefreshPorts();
        }
    }
}