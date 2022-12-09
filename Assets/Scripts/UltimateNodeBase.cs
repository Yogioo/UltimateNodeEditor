using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace UltimateNode
{
    public class UltimateNodeBase : Node
    {
        public string GUID;
        public Dictionary<string, object> portData;

        public UltimateNodeBase() : base()
        {
            Button btn = new Button(() =>
            {
                AddOutput("Test", Orientation.Horizontal, Port.Capacity.Single, typeof(Rect));
                this.RefreshExpandedState();
                this.RefreshPorts();
            }) { text = "+" };
            this.titleContainer.Add(btn);
        }

        public Port AddOutput(
            string portName,
            Orientation orientation,
            Port.Capacity capacity,
            System.Type type
        )
        {
            Port port = InstantiatePort(orientation, Direction.Output, capacity, type);
            port.source = this;
            port.name = portName;
            port.portName = portName;
            outputContainer.Add(port);
            return port;
        }

        public Port AddInput(
            string portName,
            Orientation orientation,
            Port.Capacity capacity,
            System.Type type)
        {
            Port port = InstantiatePort(orientation, Direction.Input, capacity, type);
            port.source = this;
            port.name = portName;
            port.portName = portName;
            inputContainer.Add(port);
            return port;
        }

        public Port GetInputPort(string portName)
        {
            var port = this.inputContainer.Q<Port>(portName);
            if (port == null)
            {
                Debug.LogError($"this Node :{this.name} Get Input Port Name Error:{portName}, No such Port");
            }

            return port;
        }

        public Port GetOutputPort(string portName)
        {
            var port = this.outputContainer.Q<Port>(portName);
            if (port == null)
            {
                Debug.LogError($"this Node :{this.name} Get Output Port Name Error:{portName}, No such Port");
            }

            return port;
        }

        /// <summary>
        /// this.Input connect to targetNode's output
        /// </summary>
        /// <param name="targetNode"></param>
        /// <param name="inputPortName"></param>
        /// <param name="outputPortName"></param>
        /// <returns></returns>
        public Edge ConnectInput(UltimateNodeBase targetNode, string inputPortName, string outputPortName)
        {
            var inputPort = this.GetInputPort(inputPortName);
            var outputPort = targetNode.GetOutputPort(outputPortName);
            var edge = new Edge()
            {
                input = inputPort,
                output = outputPort,
            };
            return edge;
        }

        /// <summary>
        /// this.output connect to targetNode's Input
        /// </summary>
        /// <param name="targetNode"></param>
        /// <param name="inputPortName"></param>
        /// <param name="outputPortName"></param>
        /// <returns>Need be added to GraphView</returns>
        public Edge ConnectOutput(UltimateNodeBase targetNode, string inputPortName, string outputPortName)
        {
            var inputPort = targetNode.GetInputPort(inputPortName);
            var outputPort = this.GetOutputPort(outputPortName);
            var edge = new Edge()
            {
                input = inputPort,
                output = outputPort,
            };
            return edge;
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