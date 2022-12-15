namespace UltimateNode
{
    public class UltimateGraphEventConst
    {
        /// <summary>
        /// GrpahConrolEvent,On Drop Edge Outside of port
        /// args: UltimateEdgeView edge, UnityEngine.Vector2 position
        /// </summary>
        public const string OnDropOutsidePort = "OnDropOutsidePort";

        /// <summary>
        /// args: UltimateNodeData
        /// </summary>
        public static string OnEnterExecuteNode = "ExecuteNode";
        /// <summary>
        /// args: UltimateNodeData
        /// </summary>
        public static string OnExitExecuteNode = "OnExitExecuteNode";
    }
}