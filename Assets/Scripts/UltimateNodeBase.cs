using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace UltimateNode
{
    public class UltimateNodeBase : Node
    {
        public string GUID;
        public Rect Position;

        public UltimateNodeBase():base()
        {
            Button btn = new Button(() =>
            {
                AddOutput("Test", Orientation.Horizontal, Port.Capacity.Single, typeof(Rect));
                this.RefreshExpandedState();
                this.RefreshPorts();
            }){text = "+"};
            this.titleContainer.Add(btn);
        }

        public override void SetPosition(Rect newPos)
        {
            Position = newPos;
            base.SetPosition(Position);
        }

        public void AddOutput(
            string portName,
            Orientation orientation,
            Port.Capacity capacity,
            System.Type type
        )
        {
            Port port = InstantiatePort(orientation, Direction.Output, capacity, type);
            port.source = this;
            port.name = portName;
            outputContainer.Add(port);
        }

        public void AddInput(
            string portName,
            Orientation orientation,
            Port.Capacity capacity,
            System.Type type)
        {
            Port port = InstantiatePort(orientation, Direction.Input, capacity, type);
            port.source = this;
            port.name = portName;
            inputContainer.Add(port);
        }
    }
}