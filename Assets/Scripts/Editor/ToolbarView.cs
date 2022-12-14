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
                objectType = typeof(UltimateGraphEntity)
            };
            this.Add(objectField);

            this.Add(AddBtn("Load", () =>
            {
                UltimateGraphEntity mono = objectField.value as UltimateGraphEntity;
                var data = mono.LoadFromJson();
                m_GraphView.Init(data);
            }));
            
            this.Add(AddBtn("Save", () =>
            {
                UltimateGraphEntity mono = objectField.value as UltimateGraphEntity;
                if (mono != null)
                {
                    mono.SaveToJson(m_GraphView.GraphData);
                }
            }));

            this.Add(AddBtn("Clear All", () => { m_GraphView.RemoveAllDataAndView(); }));

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