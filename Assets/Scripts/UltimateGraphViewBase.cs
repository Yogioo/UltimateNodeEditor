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

            AddElement(GenerateTestNode());
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            // return base.GetCompatiblePorts(startPort, nodeAdapter);
            return this.ports.ToList().Where(pot =>
                pot.direction != startPort.direction &&
                pot.node != startPort.node).ToList();
        }

        public UltimateNodeBase GenerateTestNode()
        {
            // Add Entry Node
            UltimateNodeBase generateEntryPointNode = new UltimateNodeBase
            {
                title = "Start",
                GUID = Guid.NewGuid().ToString(),
            };
            generateEntryPointNode.SetPosition(new Rect(100, 200, 100, 150));
            // Add Port
            generateEntryPointNode.AddOutput("Next", Orientation.Horizontal, Port.Capacity.Multi, typeof(float));
            generateEntryPointNode.AddInput("Input", Orientation.Horizontal, Port.Capacity.Multi, typeof(Vector3));
            generateEntryPointNode.AddInput("Input2", Orientation.Horizontal, Port.Capacity.Multi, typeof(float));
            // Refresh port,but it's not necessary
            generateEntryPointNode.RefreshExpandedState();
            generateEntryPointNode.RefreshPorts();
            return generateEntryPointNode;
        }
    }
}