using System;
using UnityEditor;
using UnityEngine;

namespace UltimateNode.Editor
{
    [CustomEditor(typeof(UltimateGraphEntity), true)]
    public class UltimateGraphEntityEditor : UnityEditor.Editor
    {
        private UltimateGraphEntity ultimateGraphEntity;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ultimateGraphEntity = this.target as UltimateGraphEntity;
            
            GUIBtn("Open Ultimate Graph Window", OnOpenGraphWindow);
            // GUIBtn("Load From Json");
            GUIBtn("Play", OnPlay);
        }

        private void OnPlay()
        {
            ultimateGraphEntity.Play();
        }

        private void OnOpenGraphWindow()
        {
            UltimateWindowBase.OpenWindow(ultimateGraphEntity);
        }

        public void GUIBtn(string btnName, Action btnAction)
        {
            if (GUILayout.Button(btnName))
            {
                btnAction?.Invoke();
            }
        }
    }
}