using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FullSerializer;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using YogiTools;

namespace UltimateNode.Editor
{
    public class UltimateGraphView : GraphView
    {
        private readonly UltimateWindowBase m_CoreWindow;
        private UltimateSearchWindow m_SearchWindow;
        public UltimateGraphData GraphData { get; private set; }

        private List<UltimateNodeView> m_AllNodes;
        private List<UltimateEdgeView> m_AllEdges;

        public UltimateGraphView(UltimateWindowBase m_Window)
        {
            m_AllNodes = new List<UltimateNodeView>();
            m_AllEdges = new List<UltimateEdgeView>();
            this.m_CoreWindow = m_Window;
            // Init  Grid Line BG
            AddBackground();
            // Add View Control
            AddManipulators();

            AddSearchWindow();

            //Copy And Paste Node
            serializeGraphElements += CutCopy;
            canPasteSerializedData += AllowPaste;
            unserializeAndPaste += OnPaste;
            deleteSelection += OnDeleteCallback;

            EventManager.Instance.AddEventListener(UltimateGraphEventConst.OnDropOutsidePort,
                ((UltimateEdgeView edge, UnityEngine.Vector2 position ) args) =>
                {
                    OnDropOutsidePort(args.edge, args.position);
                });
        }

        public void Init(UltimateGraphData p_GraphData)
        {
            this.GraphData = p_GraphData;
            UltimateNodeFactory.LoadGraph(this, p_GraphData);
        }

        public void AddNodeData(UltimateNodeData nodeData)
        {
            GraphData.Nodes.Add(nodeData);
        }

        /// <summary>
        /// Add Node To Data And View
        /// </summary>
        /// <param name="nodeData"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public UltimateNodeView AddNodeView(UltimateNodeData nodeData, Vector2 position)
        {
            UltimateNodeView ultimateNodeView = AddNodeView(nodeData);
            ultimateNodeView.SetPosition(position);
            return ultimateNodeView;
        }

        /// <summary>
        /// Add Node To Data And View
        /// </summary>
        /// <param name="nodeData"></param>
        /// <returns></returns>
        public UltimateNodeView AddNodeView(UltimateNodeData nodeData)
        {
            UltimateNodeView ultimateNodeView = UltimateNodeFactory.GenerateBaseNode(nodeData);
            this.m_AllNodes.Add(ultimateNodeView);
            this.AddElement(ultimateNodeView);
            ultimateNodeView.OnDisconnectAllPorts += this.DisconnectAll;
            ultimateNodeView.OnDisconnectInputPorts += this.DisconnectAllInputPorts;
            ultimateNodeView.OnDisconnectOutputPorts += this.DisconnectAllOutputPorts;
            return ultimateNodeView;
        }

        public void AddEdgeData(UltimateEdgeData p_UltimateEdgeData)
        {
            GraphData.Edges.Add(p_UltimateEdgeData);
        }

        /// <summary>
        /// Add Edge to  Graph View 
        /// </summary>
        /// <param name="p_UltimateEdgeData"></param>
        /// <returns></returns>
        public UltimateEdgeView AddEdgeView(UltimateEdgeData p_UltimateEdgeData)
        {
            var generateEdge = UltimateNodeFactory.GenerateEdge(m_AllNodes, p_UltimateEdgeData);
            AddEdgeView(generateEdge);

            return generateEdge;
        }

        public void AddEdgeView(UltimateEdgeView p_EdgeView)
        {
            this.m_AllEdges.Add(p_EdgeView);
            this.AddElement(p_EdgeView);
        }

        private void RemoveNodeDataAndViews(List<UltimateNodeView> nodeViews)
        {
            foreach (UltimateNodeView ultimateNodeView in nodeViews)
            {
                // Delete All Connection Edge
                DisconnectAll(ultimateNodeView);

                this.m_AllNodes.Remove(ultimateNodeView);
                this.GraphData.Nodes.Remove(ultimateNodeView.NodeData);
            }

            this.DeleteElements(nodeViews);
        }

        public void DisconnectAll(UltimateNodeView p_NodeView)
        {
            var nodeDataGuid = p_NodeView.NodeData.GUID;
            var connectionEdge = this.m_AllEdges.Where(x =>
                x.EdgeData.InputNodeGUID == nodeDataGuid || x.EdgeData.OutputNodeGUID == nodeDataGuid).ToList();
            RemoveEdgeDataAndViews(connectionEdge);
        }

        public void DisconnectAllInputPorts(UltimateNodeView p_NodeView)
        {
            var nodeDataGuid = p_NodeView.NodeData.GUID;
            var connectionEdge = this.m_AllEdges.Where(x =>
                x.EdgeData.InputNodeGUID == nodeDataGuid).ToList();
            RemoveEdgeDataAndViews(connectionEdge);
        }

        public void DisconnectAllOutputPorts(UltimateNodeView p_NodeView)
        {
            var nodeDataGuid = p_NodeView.NodeData.GUID;
            var connectionEdge = this.m_AllEdges.Where(x =>
                x.EdgeData.OutputNodeGUID == nodeDataGuid).ToList();
            RemoveEdgeDataAndViews(connectionEdge);
        }

        private void RemoveEdgeDataAndViews(List<UltimateEdgeView> edgeViews)
        {
            foreach (var ultimateEdgeView in edgeViews)
            {
                this.m_AllEdges.Remove(ultimateEdgeView);
                this.GraphData.Edges.Remove(ultimateEdgeView.EdgeData);
            }

            this.DeleteElements(edgeViews);
        }

        public void RemoveAllDataAndView()
        {
            RemoveEdgeDataAndViews(new List<UltimateEdgeView>(this.m_AllEdges));
            RemoveNodeDataAndViews(new List<UltimateNodeView>(this.m_AllNodes));
        }

        public void ClearAllNodeAndEdgeView()
        {
            m_AllNodes.ForEach(this.RemoveElement);
            m_AllEdges.ForEach(this.RemoveElement);
            DeleteElements(m_AllEdges);
            DeleteElements(m_AllNodes);
            m_AllNodes.Clear();
            m_AllEdges.Clear();
        }

        public void OnDropOutsidePort(UltimateEdgeView edge, UnityEngine.Vector2 position)
        {
            // This is wrong position, but I don't now how to calc this.... help~
            OnOpenSearchWindow(GetLocalMousePosition(position));
        }

        #region Callback

        private void OnDeleteCallback(string p_Operationname, AskUser p_Askuser)
        {
            List<UltimateNodeView> needDeleteNodeViews = new List<UltimateNodeView>();
            List<UltimateEdgeView> needDeleteEdgeViews = new List<UltimateEdgeView>();
            for (var i = 0; i < selection.Count; i++)
            {
                var graphElement = selection[i];
                if (graphElement is UltimateNodeView n)
                {
                    needDeleteNodeViews.Add(n);
                }
                else if (graphElement is UltimateEdgeView e)
                {
                    needDeleteEdgeViews.Add(e);
                }
            }

            RemoveNodeDataAndViews(needDeleteNodeViews);
            RemoveEdgeDataAndViews(needDeleteEdgeViews);
        }

        /// <summary>
        /// Check Node Can be paste
        /// </summary>
        /// <param name="p_Data"></param>
        /// <returns></returns>
        private bool AllowPaste(string p_Data)
        {
            return true;
        }

        private const char SPLIT_CHAR = '|';

        /// <summary>
        /// Cut or Copy Function, to Serialized Nodes
        /// </summary>
        /// <param name="p_Elements"></param>
        /// <returns></returns>
        private string CutCopy(IEnumerable<GraphElement> p_Elements)
        {
            StringBuilder sb = new StringBuilder();

            var loopElements = p_Elements.Where(x => x is UltimateNodeView).ToList();
            for (var i = 0; i < loopElements.Count; i++)
            {
                var graphElement = loopElements[i];
                if (graphElement is UltimateNodeView n)
                {
                    //TODO: Json Serialized
                    string jsonData = JsonHelper.Serialize(n.NodeData);
                    sb.Append(jsonData);
                }

                if (i != loopElements.Count - 1)
                {
                    sb.Append(SPLIT_CHAR);
                }
            }

            Debug.Log($"Serialized Json Data:{sb}");
            return sb.ToString();
        }

        private void OnPaste(string p_Operationname, string p_Data)
        {
            if (string.IsNullOrEmpty(p_Data))
            {
                return;
            }

            var nodesJsonData = p_Data.Split(SPLIT_CHAR);
            for (var i = 0; i < nodesJsonData.Length; i++)
            {
                UltimateNodeData newNodeData = JsonHelper.Deserialize<UltimateNodeData>(nodesJsonData[i]);
                newNodeData.GUID = Guid.NewGuid().ToString();
                this.AddNodeData(newNodeData);
                var newPos = newNodeData.Position;
                newPos.position += Vector2.one * 50;
                newNodeData.Position = newPos;
                this.AddNodeView(newNodeData);
            }
        }

        #endregion

        #region Manipulators

        private void AddSearchWindow()
        {
            if (m_SearchWindow == null)
            {
                m_SearchWindow = ScriptableObject.CreateInstance<UltimateSearchWindow>();
            }

            m_SearchWindow.Init(this);

            nodeCreationRequest = content =>
                OnOpenSearchWindow(content.screenMousePosition);
        }

        private void OnOpenSearchWindow(Vector2 screenMousePosition)
        {
            SearchWindow.Open(new SearchWindowContext(screenMousePosition), m_SearchWindow);
        }

        private void AddBackground()
        {
            StyleSheet s = Resources.Load<StyleSheet>("GridLine");

            this.styleSheets.Add(s);
            GridBackground gridBackground = new GridBackground()
            {
                name = "GridBackGround",
            };
            gridBackground.StretchToParentSize();
            this.Insert(0, gridBackground);
        }

        private void AddManipulators()
        {
            // Graph Zoom in/out
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            // Right click to open Contextual menu
            this.AddManipulator(AddNodeContextualMenu());
            // Drag content
            this.AddManipulator(new ContentDragger());
            // Selection
            this.AddManipulator(new SelectionDragger());
            // Rectangle Selection(Drag drop selection multiple nodes)
            this.AddManipulator(new RectangleSelector());
        }

        // 1. add contextual by namespace
        // 2. add contextual by attribute
        // 0. add contextual by function
        ContextualMenuManipulator AddNodeContextualMenu()
        {
            ContextualMenuManipulator manipulator = new ContextualMenuManipulator(
                menuEvent =>
                {
                    // menuEvent.menu.AppendAction("Add Group",
                    //     (a) =>
                    //     {
                    //         this.AddElement(GenerateGroup(
                    //             new Rect(GetLocalMousePosition(a.eventInfo.localMousePosition), Vector2.zero)));
                    //     });
                }
            );
            return manipulator;
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
                pot.node != startPort.node &&
                (
                    pot.portType == startPort.portType ||
                    (startPort.direction == Direction.Input && pot.portType.IsSubclassOf(startPort.portType)) ||
                    (pot.direction == Direction.Input && startPort.portType.IsSubclassOf(pot.portType))
                )
            ).ToList();
        }

        #endregion

        #region Func

        public Vector2 GetLocalMousePositionWhenSearchWindow(Vector2 worldMousePos)
        {
            var coreWindowPosition = worldMousePos - m_CoreWindow.position.position;
            return GetLocalMousePosition(coreWindowPosition);
        }

        public Vector2 GetLocalMousePosition(Vector2 mousePos)
        {
            Vector2 worldMousePos = mousePos;
            Vector2 localMousePos = contentViewContainer.WorldToLocal(worldMousePos);
            return localMousePos;
        }

        #endregion
    }
}