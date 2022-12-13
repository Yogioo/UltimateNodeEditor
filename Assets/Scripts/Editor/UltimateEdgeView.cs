using UnityEditor.Experimental.GraphView;

namespace UltimateNode.Editor
{
    public class UltimateEdgeView : Edge
    {
        public UltimateEdgeView()
        {
        }

        public UltimateEdgeData EdgeData;

        public UltimateEdgeView(UltimateEdgeData p_data)
        {
            Init(p_data);
        }

        public void Init(UltimateEdgeData p_data)
        {
            this.EdgeData = p_data;
        }
    }
}