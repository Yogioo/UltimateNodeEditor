using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace UltimateNode.Editor
{
    public class MiniMapView : MiniMap
    {
        public MiniMapView(GraphView baseGraphView)
        {
            this.graphView = baseGraphView;

            // not able to move mini map 
            anchored = true;

            var bgColor = new StyleColor(new Color32(29, 29, 30, 145));
            var bordColor = new StyleColor(new Color32(51, 51, 51, 255));
            this.style.backgroundColor = bgColor;
            this.style.borderTopColor = bordColor;
            this.style.borderBottomColor = bordColor;
            this.style.borderRightColor = bordColor;
            this.style.borderLeftColor = bordColor;
        }
        public void InitSize()
        {
            this.SetPosition(new Rect(15, 50, 200, 180));
        }
        public void ToggleActive()
        {
            this.visible = !this.visible;
        }
    }
}