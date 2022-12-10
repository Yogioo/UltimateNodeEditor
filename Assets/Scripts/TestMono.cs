using System.Collections.Generic;
using FullSerializer;
using UnityEngine;

namespace UltimateNode
{
    public class TestMono : MonoBehaviour
    {
        public string JsonData;
        public UltimateGraphData graphData;

        [ContextMenu("Test")]
        public void OnTest()
        {
            var newGraphData = new UltimateGraphData();
            newGraphData.Nodes = new List<UltimateNodeData>();
            newGraphData.Nodes.Add(new UltimateNodeData()
            {
                PortData = new List<PortData>()
                {
                    new PortData()
                    {
                        PortName = "A",
                        OriginVal = Color.red,
                        PortType = PortType.Input,
                        PortValueType = typeof(Color)
                    },
                    new PortData()
                    {
                        PortName = "B",
                        OriginVal = false,
                        PortType = PortType.Input,
                        PortValueType = typeof(bool)
                    },
                    new PortData()
                    {
                        PortName = "C",
                        OriginVal = 2,
                        PortType = PortType.Output,
                        PortValueType = typeof(int)
                    },
                    new PortData()
                    {
                        PortName = "E",
                        OriginVal = 5.2f,
                        PortType = PortType.Output,
                        PortValueType = typeof(float)
                    },
                }
            });
            graphData = newGraphData;
            var fsSerializer = new fsSerializer();
            fsSerializer.TrySerialize(newGraphData.GetType(), newGraphData, out var data)
                .AssertSuccessWithoutWarnings();
            JsonData = fsJsonPrinter.CompressedJson(data);
            Debug.Log("Serialize Success");
        }
        
        [ContextMenu("LoadFromStr")]
        public void LoadFromStr()
        {
            var fsSerializer = new fsSerializer();
            var fsData = fsJsonParser.Parse(this.JsonData);
            fsSerializer.TryDeserialize(fsData, ref this.graphData).AssertSuccessWithoutWarnings();
            Debug.Log("DeSerialized Success");
        }
        
        [ContextMenu("SaveCurrentToStr")]
        public void SaveCurrentToStr()
        {
            var fsSerializer = new fsSerializer();
            fsSerializer.TrySerialize(this.graphData.GetType(), graphData, out var data)
                .AssertSuccessWithoutWarnings();
            JsonData = fsJsonPrinter.CompressedJson(data);
            Debug.Log("Serialized Success");
        }


        //
        // public Test TestData;
        //
        // [ContextMenu("TestD")]
        // public void TestD()
        // {
        //     var data = JSONHelper.Serialize(TestData);
        //     Debug.Log(data);
        //     TestData = new Test();
        //     TestData = JSONHelper.Deserialize<Test>(data);
        //     Debug.Log("Success");
        // }
        // [System.Serializable]
        // public class Test
        // {
        //     public Color C;
        //     public Vector4 V;
        //     public Vector3 V2;
        //     public Vector2 V3;
        // }
    }
}