using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UltimateNode.Editor
{
    public class ToolbarView : Toolbar
    {
        // private readonly UltimateGraphView m_GraphView;

        private UltimateGraphEntity m_GraphEntity;
        private ObjectField objectField;

        public ToolbarView(UltimateGraphView graphView)
        {
            // m_GraphView = graphView;
            // this.Add(AddBtn("Add CommitGraphNode", () =>
            // {
            //     var group = new CommitGraphNode()
            //     {
            //     };
            //     m_GraphView.AddElement(group);
            // }));

            objectField = new ObjectField()
            {
                objectType = typeof(UltimateGraphEntity),
                value = m_GraphEntity
            };

            objectField.SetEnabled(false);
            this.Add(objectField);

            this.Add(AddBtn("Ping", () =>
            {
                if (m_GraphEntity != null)
                {
                    Selection.activeObject = m_GraphEntity;
                    EditorGUIUtility.PingObject(m_GraphEntity);
                }
            }));
            
            this.Add(AddBtn("Reload", () =>
            {
                if (m_GraphEntity != null)
                {
                    graphView.RemoveAllDataAndView();
                    var data = m_GraphEntity.LoadFromJson();
                    graphView.Init(data);
                }
            }));

            this.Add(AddBtn("Save", () =>
            {
                if (m_GraphEntity != null)
                {
                    m_GraphEntity.SaveToJson(graphView.GraphData);
                }
            }));

            this.Add(AddBtn("Play", ()=>m_GraphEntity.Play()));

            this.Add(AddBtn("Clear All", () =>
            {
                if (EditorUtility.DisplayDialog("Clear All",
                        "Are you sure to Clear Ultimate Graph Data And Graph View?",
                        "Yes", "No"))
                {
                    graphView.RemoveAllDataAndView();
                }
            }));
        }

        public void InitGraphEntity(UltimateGraphEntity p_GraphEntity)
        {
            m_GraphEntity = p_GraphEntity;
            objectField.value = m_GraphEntity;
        }
        Button AddBtn(string btnName, Action onclick)
        {
            return new Button(onclick)
            {
                text = btnName,
            };
        }
        Button AddBtn(string btnName, Action onclick, Color displayColor)
        {
            return new Button(onclick)
            {
                text = btnName,
                style =
                {
                    backgroundColor =  displayColor
                }
            };
        }
    }
}