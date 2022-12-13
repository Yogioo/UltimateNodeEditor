using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Object = System.Object;

namespace UltimateNode.Editor
{
    public class UltimateSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        public UltimateGraphView m_GraphView;

        private readonly List<Type> ClassTypes = new List<Type>()
            { typeof(Mathf), typeof(Debug), typeof(GameObject), typeof(Object) };

        private Texture2D m_Icon;

        public void Init(UltimateGraphView p_GraphView)
        {
            this.m_GraphView = p_GraphView;
            m_Icon = new Texture2D(1, 1);
            m_Icon.SetPixel(0, 0, Color.clear);
            m_Icon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>();
            searchTreeEntries.Add(new SearchTreeGroupEntry(new GUIContent("Ultimate Node"), 0));
            searchTreeEntries.Add(new SearchTreeGroupEntry(new GUIContent("Tool Node"), 1));
            searchTreeEntries.Add(new SearchTreeEntry(new GUIContent("Group")){level = 2,userData =  ToolNodeType.Group});
            AddNodeGroupByAttributeType(GetTypesByAttribute(), searchTreeEntries, 1);
            return searchTreeEntries;
        }

        private List<Type> GetTypesByAttribute()
        {
            List<Type> result = new List<Type>();
            var executingAssembly = Assembly.GetAssembly(typeof(UltimateNodeData));
            var types = executingAssembly.GetTypes();
            foreach (var type in types)
            {
                var customAttributes = type.GetCustomAttributes(typeof(NodeGroupAttribute), true);
                if (customAttributes.Length > 0)
                {
                    result.Add(type);
                }
            }

            return result;
        }

        private void AddNodeGroupByAttributeType(List<Type> p_Types, List<SearchTreeEntry> searchTreeEntries,
            int levelCount)
        {
            foreach (var targetClass in p_Types)
            {
                searchTreeEntries.Add(new SearchTreeGroupEntry(new GUIContent(targetClass.Name), levelCount));

                var methodInfos = targetClass.GetMethods(BindingFlags.Static | BindingFlags.Public);
                foreach (var methodInfo in methodInfos)
                {
                    StringBuilder methodDisplay = new StringBuilder($"{methodInfo.Name}");
                    methodDisplay.Append('(');
                    var parameterInfos = methodInfo.GetParameters();
                    for (var index = 0; index < parameterInfos.Length; index++)
                    {
                        var parameterInfo = parameterInfos[index];
                        methodDisplay.Append($"{parameterInfo.ParameterType.Name} {parameterInfo.Name}");
                        if (index != parameterInfos.Length - 1)
                        {
                            methodDisplay.Append(',');
                        }
                    }

                    methodDisplay.Append(')');
                    methodDisplay.Append($" : {methodInfo.ReturnType.Name}");
                    var searchTreeEntry = new SearchTreeEntry(new GUIContent(methodDisplay.ToString(), m_Icon))
                    {
                        level = levelCount + 1,
                        userData = methodInfo
                    };
                    searchTreeEntries.Add(searchTreeEntry);
                }
            }
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var localMousePosition = m_GraphView.GetLocalMousePositionWhenSearchWindow(context.screenMousePosition);

            switch (SearchTreeEntry.userData)
            {
                case MethodInfo m:
                    // Init Node Data By Method Info 
                    UltimateNodeData nodeData = new UltimateNodeData(m)
                    {
                        GUID = Guid.NewGuid().ToString(),
                    };
                    m_GraphView.AddNodeData(nodeData);
                    m_GraphView.AddNodeView(nodeData, localMousePosition);
                    return true;
                case ToolNodeType toolNodeType:
                   
                    return true;
                default:
                    break;
            }

            return false;
        }
        
        private enum ToolNodeType
        {
            Group,
        }
    }
}