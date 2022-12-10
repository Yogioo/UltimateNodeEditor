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
        public UltimateGraphViewBase m_GraphView;

        private List<Type> UnityClassTypes = new List<Type>()
            { typeof(Mathf), typeof(Debug), typeof(GameObject), typeof(Object) };

        private Texture2D m_Icon;
        public void Init(UltimateGraphViewBase p_GraphView)
        {
            this.m_GraphView = p_GraphView;

            m_Icon = new Texture2D(1, 1);
            m_Icon.SetPixel(0,0,Color.clear);
            m_Icon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>();
            int levelCount = 0;
            searchTreeEntries.Add(new SearchTreeGroupEntry(new GUIContent("Unity Func"), levelCount++));
            searchTreeEntries.Add(new SearchTreeGroupEntry(new GUIContent("Func"), levelCount++));
            foreach (var targetClass in UnityClassTypes)
            {
                searchTreeEntries.Add(new SearchTreeGroupEntry(new GUIContent(targetClass.Name), levelCount));

                var methodsInfo = targetClass.GetMethods(BindingFlags.Static | BindingFlags.Public);
                for (var i = 0; i < methodsInfo.Length; i++)
                {
                    var methodInfo = methodsInfo[i];
                    StringBuilder methodDisplay = new StringBuilder($"{targetClass.Name}/{methodInfo.Name}");
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
                    var searchTreeEntry = new SearchTreeEntry(new GUIContent(methodDisplay.ToString(),m_Icon))
                    {
                        level = levelCount + 1,
                        userData = methodInfo
                    };
                    searchTreeEntries.Add(searchTreeEntry);
                }
            }

            return searchTreeEntries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var localMousePosition = m_GraphView.GetLocalMousePositionWhenSearchWindow(context.screenMousePosition);

            switch (SearchTreeEntry.userData)
            {
                case MethodInfo m:
                    m_GraphView.AddElement(UltimateNodeFactory.GenerateBaseNode(m, localMousePosition));
                    return true;
                
                case "Test":
                    Debug.Log("Test Success");
                    return true;
                default:
                    break;
            }

            return false;
        }
    }
}