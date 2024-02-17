﻿
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace WebAuto
{
    public class ItemValue
    {
        public string Name { get; set; } = "";
        public string Key { get; set; } = "";
        public string Value { get; set; } = "";
    }
    public class ItemPattern
    {
        public string Name { get; set; } = "";
        public List<ItemValue> Items { get; set; } = new List<ItemValue>();
        public override string ToString()
        {
            return Name;
        }
    }
    public static class ItemPatternInfo
    {
        public static List<ItemPattern> ItemPatterns { get; set; } = new();

        public static event Action<ItemPattern>? AddEvent;

        public static void Add(ItemPattern itemPattern)
        {
            ItemPatterns.Add(itemPattern);
            AddEvent?.Invoke(itemPattern);
        }

        public static async Task Load()
        {
            Uri store_uri = new($"{Application.UserAppDataPath}\\..");
            string store_file = $"{store_uri.AbsolutePath}\\store\\patterns.json";
            try
            {
                using FileStream stream = new(store_file, FileMode.Open, FileAccess.Read);
                List<ItemPattern>? tmpItemPatterns = await JsonSerializer.DeserializeAsync<List<ItemPattern>>(stream, new JsonSerializerOptions()
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true
                });
                ItemPatterns.Clear();
                if (tmpItemPatterns != null)
                {
                    ItemPatterns = tmpItemPatterns;
                }
            }
            catch { }
        }
        public static async Task Save()
        {
            Uri store_uri = new($"{Application.UserAppDataPath}\\..");
            string store_path = $"{store_uri.AbsolutePath}\\store";
            if (!Directory.Exists(store_path))
            {
                _ = Directory.CreateDirectory(store_path);
            }
            string store_file = $"{store_path}\\patterns.json";
            try
            {
                using FileStream stream = new(store_file, FileMode.Create, FileAccess.Write);
                await JsonSerializer.SerializeAsync(stream, ItemPatterns, new JsonSerializerOptions()
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = true
                });
            }
            catch { }
        }
    }
}