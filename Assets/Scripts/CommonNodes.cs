namespace UltimateNode
{
    [NodeGroup]
    public class FlowControl
    {
        public static void OnStart(out FlowData Start)
        {
            Start = new FlowData();
        }

        public static void OnAwake(out FlowData Awake)
        {
            Awake = new FlowData();
        }

        public static void OnUpdate(out FlowData Update)
        {
            Update = new FlowData();
        }

        public static void OnStop(out FlowData Stop)
        {
            Stop = new FlowData();
        }

        public static void OnAIUpdate(out FlowData AIUpdate)
        {
            AIUpdate = new FlowData();
        }
    }

    public class FlowData
    {
    
    }

    public class AIFlowData
    {
        /// <summary>
        /// Return Value of Action Or Condition Node
        /// </summary>
        public bool ReturnValue = false;
    }
}