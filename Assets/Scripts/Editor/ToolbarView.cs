using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UltimateNode.Editor
{
    public class ToolbarView : Toolbar
    {
        private readonly UltimateGraphView m_GraphView;

        public ToolbarView(UltimateGraphView graphView)
        {
            m_GraphView = graphView;

            this.Add(AddBtn("Add CommitGraphNode", () =>
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
            this.Add(objectField);

            this.Add(AddBtn("Add Node By Json", () =>
            {
                TestMono mono = objectField.value as TestMono;
                mono.LoadFromStr();
                var data = mono.graphData;
                m_GraphView.Init(data);
            }));
        }

        Button AddBtn(string btnName, Action onclick)
        {
            return new Button(onclick)
            {
                text = btnName
            };
        }
    }
}