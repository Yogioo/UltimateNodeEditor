/*
 * **********************************************
* Look At Example Class Down below
* [NodeGroupEntry]
* public class TestClass
* {
*     [NodeEntry]
*     [return: OutPort]
*     public float TestFunc([InputPort] int inputValue, [OutPort] float outputVal)
*     {
*         return outputVal + inputValue;
*     }
* }
* **********************************************
*/

using System;

namespace UltimateNode
{
    #region Graph Attriubte

    /// <summary>
    /// When tag this attribute, this class will be Custom Node Group
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class StaticNodeGroupAttribute : Attribute
    {
       
    }
    
    #endregion

    #region PortAttribute

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class MultiPortAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class HidePortAttribute : Attribute
    {
    }

    #endregion
}