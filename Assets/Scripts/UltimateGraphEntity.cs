using FullSerializer;
using UnityEngine;

namespace UltimateNode
{
    [StaticNodeGroup]
    public abstract class UltimateGraphEntity : MonoBehaviour
    {
        public string JsonData;
        private UltimateGraphData m_GraphData = null;

        protected virtual void Awake()
        {
            LoadFromJson();
        }

        [ContextMenu("LoadFromJson")]
        public UltimateGraphData LoadFromJson()
        {
            if (string.IsNullOrWhiteSpace(JsonData))
            {
                m_GraphData = new UltimateGraphData();
                SaveToJson();
            }
            this.m_GraphData = JsonHelper.Deserialize<UltimateGraphData>(this.JsonData);
            Debug.Log("DeSerialized Success");
            return this.m_GraphData;
        }

        [ContextMenu("SaveToJson")]
        public void SaveToJson()
        {
            SaveToJson(this.m_GraphData);
        }

        public void SaveToJson(UltimateGraphData p_Data)
        {
            JsonData = JsonHelper.Serialize(p_Data);
            Debug.Log("Serialized Success");
        }

        [ContextMenu("Play")]
        public void Play()
        {
            if (!Application.isPlaying)
            {
                LoadFromJson();
            }

            UltimateFlowProcess process = new UltimateFlowProcess(this.m_GraphData);
            process.Play(this);
        }
    }
}