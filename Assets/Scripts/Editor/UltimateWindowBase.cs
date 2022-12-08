using System;
using UnityEditor;
using UnityEditor.UIElements;
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
        
        private UltimateGraphViewBase m_GraphView;

        private void OnEnable()
        {
            // Add a graph view
            m_GraphView = new UltimateGraphViewBase()
            {
                name = "UltimateGraphView"
            };
            // Full the window with the graph view
            m_GraphView.StretchToParentSize();
            rootVisualElement.Add(m_GraphView);
            
            
            // Add a button to the toolbar
            // after the graph view, so it's could covered by the graph view
            var toolbar = new Toolbar();
            rootVisualElement.Add(toolbar);
            var button = new Button(() =>
            {
                m_GraphView.AddElement(m_GraphView.GenerateTestNode());
            })
            {
                text = "AddNode"
            };
            toolbar.Add(button);
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(m_GraphView);
        }
    }
}