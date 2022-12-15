using System;
using System.Collections.Generic;
using System.Reflection;

namespace UltimateNode
{
    public static class UltimateSetting
    {
        public static readonly List<Type> IgnoreDisplayTypeArr = new List<Type>()
        {
            typeof(object),
            typeof(FlowData),
        };

        public static readonly BindingFlags ParseNodeBindingFlags =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

        public static readonly Assembly
            ParseAssembly = Assembly.GetExecutingAssembly(); //Assembly.GetAssembly(typeof(UltimateNodeData));
    }
}