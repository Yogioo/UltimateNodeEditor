namespace FullSerializer
{
    public class JsonHelper
    {
        public static string Serialize<T>(T data)
        {
            var fsSerializer = new fsSerializer();
            fsSerializer.TrySerialize(typeof(T), data, out var fsData)
                .AssertSuccessWithoutWarnings();
            return fsJsonPrinter.CompressedJson(fsData);
        }

        public static T Deserialize<T>(string jsonData) where T : new()
        {
            var result = new T();
            var fsSerializer = new fsSerializer();
            var fsData = fsJsonParser.Parse(jsonData);
            fsSerializer.TryDeserialize(fsData, ref result).AssertSuccessWithoutWarnings();
            return result;
        }
    }
}