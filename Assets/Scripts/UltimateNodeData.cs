using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FullSerializer.Internal;
using UnityEngine;
using UnityEngine.Windows.WebCam;

namespace UltimateNode
{
    [System.Serializable]
    public class UltimateGroupData
    {
        public string Name;
        public string GUID;
        public Rect Position;
    }

    /// <summary>
    /// Node Data
    /// </summary>
    [System.Serializable]
    public class UltimateNodeData
    {
        public string Name;
        public string GUID;
        public Rect Position;
        public List<PortData> PortData;

        public string ClassFullName;
        public string MethodFullName;

        private MethodInfo m_MethodInfo
        {
            get
            {
                if (_MethodInfo == null)
                {
                    
                    var targetClass = UltimateSetting.ParseAssembly.GetType(ClassFullName);
                    _MethodInfo = targetClass.GetMethod(this.MethodFullName, UltimateSetting.ParseNodeBindingFlags);
                }

                return _MethodInfo;
            }
        }

        private MethodInfo _MethodInfo;


        public UltimateNodeData()
        {
            PortData = new List<PortData>();
        }

        public UltimateNodeData(MethodInfo methodInfo) : this()
        {
            Name = methodInfo.Name;

            this.ClassFullName = methodInfo.DeclaringType.FullName;
            this.MethodFullName = methodInfo.Name;

            // Input/Output Port
            var parameterInfos = methodInfo.GetParameters();
            for (var i = 0; i < parameterInfos.Length; i++)
            {
                ParameterInfo parameterInfo = parameterInfos[i];
                var customAttributes = parameterInfo.GetCustomAttributes(typeof(MultiPortAttribute));
                int capacity = customAttributes.Any() ? 1 : 0;

                var hideAttributes = parameterInfo.GetCustomAttributes(typeof(HidePortAttribute));
                bool isHidePort = hideAttributes.Any();

                PortType portType = PortType.Input;

                var realParameterType = parameterInfo.ParameterType;
                if (realParameterType.IsByRef)
                {
                    realParameterType = realParameterType.GetElementType();
                    portType = PortType.Output;
                }

                this.PortData.Add(new PortData()
                {
                    Capacity = capacity,
                    Orientation = 0,
                    PortName = parameterInfo.Name,
                    PortType = portType,
                    PortValueType = realParameterType,
                    IsHide = isHidePort,
                    OriginVal = parameterInfo.ParameterType.IsValueType
                        ? Activator.CreateInstance(parameterInfo.ParameterType)
                        : null,
                });
            }

            // Return Port
            if ((methodInfo.ReturnParameter.ParameterType != typeof(void)))
            {
                var returnType = methodInfo.ReturnParameter.ParameterType;
                this.PortData.Add(new PortData()
                {
                    Capacity = 0,
                    Orientation = 0,
                    PortName = methodInfo.ReturnParameter.Name,
                    PortType = PortType.Output,
                    PortValueType = returnType,
                    OriginVal = methodInfo.ReturnType.IsValueType
                        ? Activator.CreateInstance(methodInfo.ReturnType)
                        : null,
                });
            }
        }

        public async Task Execute(object owner)
        {
            Debug.Log($"Execute:{Name}");

            object[] inputData = new object[this.PortData.Count];
            for (var i = 0; i < this.PortData.Count; i++)
            {
                inputData[i] = this.PortData[i].OriginVal;
            }

            // ---------------------Execute Async Function--------------------- 
            var _methodInfo = m_MethodInfo;
            // Await Reflection Async Function
            var isAwaitable = _methodInfo.ReturnType.GetMethod(nameof(Task.GetAwaiter)) != null;
            if (isAwaitable)
            {
                await (Task)_methodInfo.Invoke(owner, inputData);
            }
            else
            {
                _methodInfo.Invoke(owner, inputData);
            }
            // ---------------------Execute Complete---------------------

            for (var i = 0; i < this.PortData.Count; i++)
            {
                this.PortData[i].OriginVal = inputData[i];
            }
        }

        private async void ExecuteAsyncFunctionWithoutNoReturn(object owner, object[] inputData)
        {
            var _methodInfo = m_MethodInfo;
            // Await Reflection Async Function
            var isAwaitable = _methodInfo.ReturnType.GetMethod(nameof(Task.GetAwaiter)) != null;
            if (isAwaitable)
            {
                await (Task)_methodInfo.Invoke(owner, inputData);
            }
            else
            {
                _methodInfo.Invoke(owner, inputData);
            }
        }
        // private async Task<object> ExecuteAsyncFunction(object owner, object[] inputData)
        // {
        //     var _methodInfo = m_MethodInfo;
        //     // Await Reflection Async Function
        //     var isAwaitable = _methodInfo.ReturnType.GetMethod(nameof(Task.GetAwaiter)) != null;
        //     if (isAwaitable)
        //     {
        //         if (_methodInfo.ReturnType.IsGenericType)
        //         {
        //             return (object)await (dynamic)_methodInfo.Invoke(owner, inputData);
        //         }
        //         else
        //         {
        //             await (Task)_methodInfo.Invoke(owner, inputData);
        //         }
        //     }
        //     else
        //     {
        //         if (_methodInfo.ReturnType == typeof(void))
        //         {
        //             _methodInfo.Invoke(owner, inputData);
        //         }
        //         else
        //         {
        //             return _methodInfo.Invoke(owner, inputData);
        //         }
        //     }
        //
        //     return null;
        // }


        public override string ToString()
        {
            return this.Name;
        }
    }

    /// <summary>
    /// Node Graph Data
    /// </summary>
    [System.Serializable]
    public class UltimateGraphData
    {
        public List<UltimateNodeData> Nodes;
        public List<UltimateEdgeData> Edges;

        public UltimateGraphData()
        {
            Nodes = new List<UltimateNodeData>();
            Edges = new List<UltimateEdgeData>();
        }

        public UltimateNodeData GetNodeByGUID(string guid)
        {
            return this.Nodes.FirstOrDefault(x => x.GUID == guid);
        }
    }


    [System.Serializable]
    public class PortData
    {
        public string PortName;
        public Type PortValueType;

        /// <summary>
        /// 0 is Input, 1 is Output
        /// </summary>
        public PortType PortType;

        /// <summary>
        /// Port Connect Direction
        /// </summary>
        public int Orientation;

        /// <summary>
        /// if equal 1 Port be able to connect multi edge
        /// </summary>
        public int Capacity;

        /// <summary>
        /// Hide Port
        /// </summary>
        public bool IsHide;

        /// <summary>
        /// Port Data
        /// </summary>
        public object OriginVal;
    }

    [System.Serializable]
    public class UltimateEdgeData
    {
        public string InputNodeGUID;
        public string OutputNodeGUID;
        public string InputPortName;
        public string OutputPortName;
    }

    public enum PortType
    {
        None,
        Input,
        Output,
    }
}