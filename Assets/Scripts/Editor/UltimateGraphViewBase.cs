using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace UltimateNode.Editor
{
    public class UltimateGraphViewBase : GraphView
    {
        private UltimateSearchWindow m_SearchWindow;
        private UltimateWindowBase m_CoreWindow;

        public UltimateGraphViewBase(UltimateWindowBase m_Window)
        {
            this.m_CoreWindow = m_Window;
            // Init  Grid Line BG
            AddBackground();
            // Add View Control
            AddManipulators();

            AddSearchWindow();

            //TODO: Load Graph From Data
        }

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
                    menuEvent.menu.AppendAction("Add Group",
                        (a) =>
                        {
                            this.AddElement(GenerateGroup(new Rect(GetLocalMousePosition(a.eventInfo.localMousePosition), Vector2.zero)));
                        });
                    // AppendActionByUnityFunction(menuEvent.menu);
                }
            );
            return manipulator;
        }
        //
        // public void AppendActionByUnityFunction(DropdownMenu p_MenuEventMenu)
        // {
        //     var targetClass = typeof(Mathf);
        //     var methodsInfo = targetClass.GetMethods(BindingFlags.Static | BindingFlags.Public);
        //     for (var i = 0; i < methodsInfo.Length; i++)
        //     {
        //         var methodInfo = methodsInfo[i];
        //         StringBuilder methodDisplay = new StringBuilder($"{targetClass.Name}/{methodInfo.Name}");
        //         methodDisplay.Append('(');
        //         var parameterInfos = methodInfo.GetParameters();
        //         for (var index = 0; index < parameterInfos.Length; index++)
        //         {
        //             var parameterInfo = parameterInfos[index];
        //             methodDisplay.Append($"{parameterInfo.ParameterType.Name} {parameterInfo.Name}");
        //             if (index != parameterInfos.Length - 1)
        //             {
        //                 methodDisplay.Append(',');
        //             }
        //         }
        //
        //         methodDisplay.Append(')');
        //         methodDisplay.Append($" : {methodInfo.ReturnType.Name}");
        //         p_MenuEventMenu.AppendAction(methodDisplay.ToString(),
        //             actionEvent =>
        //             {
        //                 var node = GenerateNode(methodInfo,
        //                     new Rect(actionEvent.eventInfo.mousePosition, Vector2.zero));
        //                 this.AddElement(node);
        //             });
        //     }
        // }

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

        public Group GenerateGroup(Rect position)
        {
            var group = new Group()
            {
                name = "Group",
            };
            group.SetPosition(position);
            return group;
        }

        /// <summary>
        /// Connect Two Node Port By Port Name
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="inputPortName"></param>
        /// <param name="outputPortName"></param>
        private void ConnectPort(UltimateNodeBase a, UltimateNodeBase b, string inputPortName, string outputPortName)
        {
            this.AddElement(a.ConnectOutput(b, inputPortName, outputPortName));
        }

        
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
    }
}