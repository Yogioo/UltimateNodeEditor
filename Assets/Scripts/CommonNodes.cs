namespace UltimateNode
{
    [StaticNodeGroup]
    public static class FlowControl
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
    [StaticNodeGroup]
    public static class AI
    {
        
        #region AI Nodes

        public static void OnAIThink([MultiPort] ref AIFlowData o)
        {
            o = new AIFlowData();
        }

        public static void Select(AIFlowData i, [MultiPort] ref AIFlowData o)
        {
        }

        public static void Sequence(AIFlowData i, [MultiPort] ref AIFlowData o)
        {
        }
        #endregion
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