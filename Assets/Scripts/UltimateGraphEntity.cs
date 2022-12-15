using System;
using FullSerializer;
using UnityEngine;

namespace UltimateNode
{
    [NodeGroup("Base Graph Entity")]
    public abstract class UltimateGraphEntity : MonoBehaviour
    {
        public string JsonData;
        private UltimateGraphData m_GraphData = null;

        private UltimateFlowProcess processor
        {
            get
            {
                if (_processor == null)
                {
                    _processor = new UltimateFlowProcess(m_GraphData);
                }
                return _processor;
            }
        }

        private UltimateFlowProcess _processor;


        protected virtual void Awake()
        {
            LoadFromJson();
        }

        public UltimateGraphData LoadFromJson()
        {
            if (string.IsNullOrWhiteSpace(JsonData))
            {
                m_GraphData = new UltimateGraphData();
                SaveToJson();
            }

            this.m_GraphData = JsonHelper.Deserialize<UltimateGraphData>(this.JsonData);
            return this.m_GraphData;
        }

        public void SaveToJson()
        {
            SaveToJson(this.m_GraphData);
        }

        public void SaveToJson(UltimateGraphData p_Data)
        {
            JsonData = JsonHelper.Serialize(p_Data);
        }

        private void CheckJson()
        {
            if (!Application.isPlaying)
            {
                LoadFromJson();
            }
        }

        public void Play()
        {
            CheckJson();
            processor.PlayFlow(this, nameof(FlowControl.OnStart));
            processor.PlayAIFlow(this);
        }

        public void PlayUpdate()
        {
            CheckJson();
            processor.PlayFlow(this, nameof(FlowControl.OnUpdate));
        }

        #region Unity Action

        protected virtual void Start()
        {
            Play();
        }

        protected virtual void Update()
        {
            PlayUpdate();
        }

        #endregion
    }
}