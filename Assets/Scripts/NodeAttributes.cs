using System;

namespace UltimateNode
{
    /// **********************************************
    /// Look At Example Class Down below
    /// [NodeGroupEntry]
    /// public class TestClass
    /// {
    ///     [NodeEntry]
    ///     [return: OutPort]
    ///     public float TestFunc([InputPort] int inputValue, [OutPort] float outputVal)
    ///     {
    ///         return outputVal + inputValue;
    ///     }
    /// }
    /// **********************************************
    
    /// <summary>
    /// When tag this attribute, this class will be Custom Node Group
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NodeGroupAttribute : Attribute
    {
    }

    // [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    // public class NodeEntryAttribute : Attribute
    // {
    // }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class MultiPortAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class HidePortAttribute : Attribute
    {
    }
    //
    // [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false,
    //     Inherited = false)]
    // public class OutPortAttribute : Attribute
    // {
    // }
}