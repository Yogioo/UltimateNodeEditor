using System.Collections.Generic;
using System.Linq;
using System.Text;
using FullSerializer;
using NUnit.Framework;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace UltimateNode.Editor
{
    public class UltimateGraphView : GraphView
    {
        private readonly UltimateWindowBase m_CoreWindow;
        private UltimateSearchWindow m_SearchWindow;
        private UltimateGraphData m_GraphData;

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

            Init(new UltimateGraphData());
        }


        public void ClearAllNodeAndEdge()
        {
            m_AllNodes.ForEach(this.RemoveElement);
            m_AllEdges.ForEach(this.RemoveElement);
        }

        public void Init(UltimateGraphData p_GraphData)
        {
            ClearAllNodeAndEdge();
            this.m_GraphData = p_GraphData;

            UltimateNodeFactory.LoadGraph(m_GraphData, out m_AllNodes,
                out m_AllEdges);
            m_AllNodes.ForEach(this.AddElement);
            m_AllEdges.ForEach(this.AddElement);
        }

        /// <summary>
        /// Add Node To Data And View
        /// </summary>
        /// <param name="nodeData"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public UltimateNodeView AddNode(UltimateNodeData nodeData, Vector2 position)
        {
            var ultimateNodeBase = this.AddNode(nodeData);
            ultimateNodeBase.SetPosition(position);
            return ultimateNodeBase;
        }

        /// <summary>
        /// Add Node To Data And View
        /// </summary>
        /// <param name="nodeData"></param>
        /// <returns></returns>
        public UltimateNodeView AddNode(UltimateNodeData nodeData)
        {
            m_GraphData.Nodes.Add(nodeData);
            UltimateNodeView ultimateNodeView = UltimateNodeFactory.GenerateBaseNode(nodeData);
            this.m_AllNodes.Add(ultimateNodeView);
            this.AddElement(ultimateNodeView);
            return ultimateNodeView;
        }

        /// <summary>
        /// Add Edge to Data And View 
        /// </summary>
        /// <param name="p_UltimateEdgeData"></param>
        /// <returns></returns>
        public UltimateEdgeView AddEdge(UltimateEdgeData p_UltimateEdgeData)
        {
            AddEdgeData(p_UltimateEdgeData);
            var generateEdge = UltimateNodeFactory.GenerateEdge(m_AllNodes, p_UltimateEdgeData);
            AddEdgeView(generateEdge);
            return generateEdge;
        }

        public void AddEdgeData(UltimateEdgeData p_UltimateEdgeData)
        {
            m_GraphData.Edges.Add(p_UltimateEdgeData);
        }

        public void AddEdgeView(UltimateEdgeView p_EdgeView)
        {
            this.m_AllEdges.Add(p_EdgeView);
            this.AddElement(p_EdgeView);
        }

        private void RemoveNodeViews(List<UltimateNodeView> nodeViews)
        {
            foreach (var ultimateNodeView in nodeViews)
            {
                this.m_AllNodes.Remove(ultimateNodeView);
                this.m_GraphData.Nodes.Remove(ultimateNodeView.NodeData);
            }
            this.DeleteElements(nodeViews);
        }

        private void RemoveEdgeViews(List<UltimateEdgeView> edgeViews)
        {
            foreach (var ultimateEdgeView in edgeViews)
            {
                this.m_AllEdges.Remove(ultimateEdgeView);
                this.m_GraphData.Edges.Remove(ultimateEdgeView.EdgeData);
            }
            this.DeleteElements(edgeViews);
        }

        public void RemoveAll()
        {
            RemoveEdgeViews(new List<UltimateEdgeView>(this.m_AllEdges));
            RemoveNodeViews(new List<UltimateNodeView>(this.m_AllNodes));
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

            RemoveNodeViews(needDeleteNodeViews);
            RemoveEdgeViews(needDeleteEdgeViews);
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
                var ultimateNodeBase = UltimateNodeFactory.GenerateBaseNode(newNodeData);
                var newPos = newNodeData.Position;
                newPos.position += Vector2.one * 50;
                ultimateNodeBase.SetPosition(newPos);
                this.AddElement(ultimateNodeBase);
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
                SearchWindow.Open(new SearchWindowContext(content.screenMousePosition), m_SearchWindow);
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
                pot.portType == startPort.portType).ToList();
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