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

            m_GraphView.Add(new MiniMapView(m_GraphView));

            // Add a button to the toolbar
            // after the graph view, so it's could covered by the graph view
            AddToolbar();
        }

        void AddToolbar()
        {
            var toolbar = new Toolbar();
            rootVisualElement.Add(toolbar);


            toolbar.Add(AddBtn("Add CommitGraphNode", () =>
            {
                var group = new CommitGraphNode()
                {
                };
                m_GraphView.AddElement(group);
            }));

            var objectField = new ObjectField()
            {
                objectType = typeof(TestMono)
            };
            toolbar.Add(objectField);

            toolbar.Add(AddBtn("Add CommitGraphNode", () =>
            {
                var data = (objectField.value as TestMono).graphData;
                // Test Load Graph 
                UltimateNodeFactory.LoadGraph(data, out var nodes, out var edges);
                nodes.ForEach(x => { m_GraphView.AddElement(x); });
                edges.ForEach(x => { m_GraphView.AddElement(x); });

            }));
        }

        Button AddBtn(string btnName, Action onclick)
        {
            return new Button(onclick)
            {
                text = btnName
            };
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(m_GraphView);
        }
    }
}