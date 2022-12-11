using UnityEditor.Experimental.GraphView;

namespace UltimateNode.Editor
{
    public class UltimateEdgeView : Edge
    {
        public UltimateEdgeView()
        {
        }

        private UltimateEdgeData m_Data;

        public UltimateEdgeView(UltimateEdgeData p_data)
        {
            Init(p_data);
        }

        public void Init(UltimateEdgeData p_data)
        {
            this.m_Data = p_data;
        }
    }
}