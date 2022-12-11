using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UltimateNode.Editor
{
    public class UltimateWindowBase : EditorWindow
    {
        [MenuItem("Graph/UltimateWindow")]
        public static void OpenWindow()
        {
            var window = GetWindow(typeof(UltimateWindowBase));
            window.titleContent = new GUIContent("UltimateWindow");
        }

        private UltimateGraphView m_GraphView;

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

            m_GraphView.Add(new MiniMapView(m_GraphView));
            
            // Add a button to the toolbar
            // after the graph view, so it's could covered by the graph view
            this.rootVisualElement.Add(new ToolbarView(this.m_GraphView));
            
        }
        private void OnDisable()
        {
            rootVisualElement.Remove(m_GraphView);
        }
    }
}