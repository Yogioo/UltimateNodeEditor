using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace UltimateNode
{
    public class UltimateGraphViewBase : GraphView
    {
        public UltimateGraphViewBase()
        {
            // Init  Grid Line BG
            StyleSheet s = Resources.Load<StyleSheet>("GridLine");
            this.styleSheets.Add(s);
            GridBackground gridBackground = new GridBackground()
            {
                name = "GridBackGround",
            };
            this.Insert(0, gridBackground);

            // Graph Zoom in/out
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            
            // Drag content
            this.AddManipulator(new ContentDragger());
            // Selection
            this.AddManipulator(new SelectionDragger());
            // Rectangle Selection(Drag drop selection multiple nodes)
            this.AddManipulator(new RectangleSelector());

            GenerateEdgeTest();
        }

        /// <summary>
        /// Override this can make edge enable to connect
        /// </summary>
        /// <param name="startPort"></param>
        /// <param name="nodeAdapter"></param>
        /// <returns></returns>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            // return base.GetCompatiblePorts(startPort, nodeAdapter);
            return this.ports.ToList().Where(pot =>
                pot.direction != startPort.direction &&
                pot.node != startPort.node).ToList();
        }

        /// <summary>
        /// Example
        /// </summary>
        /// <param name="inputPort"></param>
        /// <param name="outputPort"></param>
        /// <returns></returns>
        public UltimateNodeBase GenerateTestNode(out Port inputPort, out Port outputPort)
        {
            // Add Entry Node
            UltimateNodeBase generateEntryPointNode = new UltimateNodeBase
            {
                title = "Start",
                GUID = Guid.NewGuid().ToString(),
            };
            generateEntryPointNode.SetPosition(new Rect(100, 200, 100, 150));
            // Add Port
            var addOutput =
                generateEntryPointNode.AddOutput("Next", Orientation.Horizontal, Port.Capacity.Multi, typeof(float));
            var addInput = generateEntryPointNode.AddInput("Input", Orientation.Horizontal, Port.Capacity.Multi,
                typeof(Vector3));
            generateEntryPointNode.AddInput("Input2", Orientation.Horizontal, Port.Capacity.Multi, typeof(float));
            // Refresh port,but it's not necessary
            generateEntryPointNode.RefreshExpandedState();
            generateEntryPointNode.RefreshPorts();

            inputPort = addInput;
            outputPort = addOutput;

            return generateEntryPointNode;
        }

        /// <summary>
        /// test
        /// </summary>
        public void GenerateEdgeTest()
        {
            var a = GenerateTestNode(out var ai, out var ao);
            var b = GenerateTestNode(out var bi, out var bo);

            AddElement(a);
            AddElement(b);

            ConnectPort(a, b, "Input2", "Next");
        }


        /// <summary>
        /// Connect Two Node Port By Port Name
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="inputPortName"></param>
        /// <param name="outputPortName"></param>
        private void ConnectPort(UltimateNodeBase a, UltimateNodeBase b, string inputPortName, string outputPortName)
        {
            this.AddElement(a.ConnectOutput(b, inputPortName, outputPortName));
        }
    }
}