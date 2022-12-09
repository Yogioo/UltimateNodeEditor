using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace UltimateNode
{
    public class MiniMapView : MiniMap
    {
        public MiniMapView(GraphView baseGraphView)
        {
            this.graphView = baseGraphView;
            SetPosition(new Rect(0, 0, 100, 100));
        }
    }
}