using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UltimateNode.Editor
{
    public class UltimateWindowBase : EditorWindow
    {
        // [MenuItem("Graph/UltimateWindow")]
        public static UltimateWindowBase OpenWindow(UltimateGraphEntity p_GraphEntity)
        {
            var window = GetWindow(typeof(UltimateWindowBase));
            window.titleContent = new GUIContent("UltimateWindow");
            var ultimateWindowBase = window as UltimateWindowBase;
            ultimateWindowBase.Init(p_GraphEntity);
            return ultimateWindowBase;
        }

        private UltimateGraphView m_GraphView;
        private ToolbarView m_ToolbarView;

        private void OnEnable()
        {
            // Add a graph view
            m_GraphView = new UltimateGraphView(this)
            {
                name = "UltimateGraphView"
            };
            // Full the window with the graph view
            m_GraphView.StretchToParentSize();
            rootVisualElement.Add(m_GraphView);

            var miniMapView = new MiniMapView(m_GraphView);
            miniMapView.InitSize();
            m_GraphView.Add(miniMapView);

            // Add a button to the toolbar
            // after the graph view, so it's could covered by the graph view
            m_ToolbarView = new ToolbarView(this.m_GraphView);
            this.rootVisualElement.Add(m_ToolbarView);
        }

        public void Init(UltimateGraphEntity p_GraphEntity)
        {
            m_GraphView.ClearAllNodeAndEdgeView();
            
            m_ToolbarView.InitGraphEntity(p_GraphEntity);
            m_GraphView.Init(p_GraphEntity.LoadFromJson());
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(m_GraphView);
        }
    }
}