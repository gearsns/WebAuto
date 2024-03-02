using System.Text.Json.Nodes;

namespace WebAuto
{
    public static class JsonNodeExtensions
    {
        public static string ToString(this JsonNode jsonNode, string? key)
        {
            return null == key || null == jsonNode[key] ? "" : jsonNode[key]?.ToString() ?? "";
        }
        public static string ToString(this JsonNode jsonNode, int key)
        {
            return jsonNode is not JsonArray jsonArray || 0 > key || key >= jsonArray.Count ? "" : jsonArray[key]?.ToString() ?? "";
        }
    }
}
