using System.Text.Json;
using System.Text.Unicode;
using System.Text.Encodings.Web;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace WebAuto
{
    public class Store
	{
#if DEBUG
        private readonly string _filename = Path.Combine(Application.StartupPath, "Setting.json");
        private readonly string _outputPath = Path.Combine(Application.StartupPath, "Output");
#else
        private readonly string _filename = Path.Combine(Application.UserAppDataPath, "..\\store\\Setting.json");
        private readonly string _outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "WebAutoOutput");
#endif
        public static Store Instance { get; } = new();

        private class ItemInfo
        {
            public string Value { get; set; } = "";
            public bool IsCypt { get; set; }
        }
        private readonly Dictionary<string, ItemInfo> _itemList = [];

        private Store() { InitValue(); }

        private void InitValue()
        {
            _itemList["WebAutoOutputFolder"] = new ItemInfo { Value = _outputPath };
        }
        public string GetValue(string key)
        {
            return _itemList.ContainsKey(key) ? _itemList[key].Value ?? "" : "";
        }

        public void SetValue(string key, string value, bool isCypt = false)
        {
            _itemList[key] = new ItemInfo { Value = value, IsCypt = isCypt };
        }
        //
        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException(nameof(plainText));
            }
            if (Key == null || Key.Length <= 0)
            {
                throw new ArgumentNullException(nameof(Key));
            }
            if (IV == null || IV.Length <= 0)
            {
                throw new ArgumentNullException(nameof(IV));
            }
            // Create an Aes object
            // with the specified key and IV.
            using Aes aesAlg = Aes.Create();
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using MemoryStream msEncrypt = new();
                using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
                using (StreamWriter swEncrypt = new(csEncrypt))
                {
                    //Write all data to the stream.
                    swEncrypt.Write(plainText);
                }
            return msEncrypt.ToArray();
        }
        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException(nameof(cipherText));
            }
            if (Key == null || Key.Length <= 0)
            {
                throw new ArgumentNullException(nameof(Key));
            }
            if (IV == null || IV.Length <= 0)
            {
                throw new ArgumentNullException(nameof(IV));
            }
            // Create an Aes object
            // with the specified key and IV.
            using Aes aesAlg = Aes.Create();
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using MemoryStream msDecrypt = new(cipherText);
                using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
                using StreamReader srDecrypt = new(csDecrypt);

                // Read the decrypted bytes from the decrypting stream
                // and place them in a string.
            return srDecrypt.ReadToEnd();
        }
        //
        private class Item
        {
            public string Name { get; set; } = "";
            public string Value { get; set; } = "";
            [JsonPropertyName("vk")]
            public string? AesKey { get; set; }
            [JsonPropertyName("vz")]
            public string? AesIV { get; set; }
        }

        private static readonly JsonSerializerOptions options = new()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        public bool Load()
        {
            try
            {
                FileStream fs = new(_filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                List<Item> tmpList = JsonSerializer.Deserialize<List<Item>>(fs, options) ?? [];
                fs.Close();
                fs.Dispose();
                _itemList.Clear();
                InitValue();
                    foreach (Item item in tmpList)
                    {
                        ItemInfo tmpItem = new()
                        {
                            Value = item.Value,
                        };
                        if (!string.IsNullOrEmpty(item.AesKey) && !string.IsNullOrEmpty(item.AesIV))
                        {
                            tmpItem.Value = DecryptStringFromBytes_Aes(Convert.FromBase64String(item.Value)
                                , Convert.FromBase64String(item.AesKey), Convert.FromBase64String(item.AesIV));
                            tmpItem.IsCypt = true;
                        }
                        _itemList[item.Name] = tmpItem;
                    }
                }
            catch
            {
                return false;
            }
            return true;
        }

        public bool Save()
        {
            List<Item> tmpList = [];
            using Aes myAes = Aes.Create();
            foreach (KeyValuePair<string, ItemInfo> item in _itemList)
            {
                    if (item.Value is ItemInfo itemInfo)
                    {
                        Item tmpItem = new()
                        {
                            Name = item.Key,
                            Value = itemInfo.Value
                        };
                        if (itemInfo.IsCypt)
                        {
                            byte[] encrypted = EncryptStringToBytes_Aes(itemInfo.Value, myAes.Key, myAes.IV);
                            tmpItem.Value = Convert.ToBase64String(encrypted);
                            tmpItem.AesKey = Convert.ToBase64String(myAes.Key);
                            tmpItem.AesIV = Convert.ToBase64String(myAes.IV);
                        }
                        tmpList.Add(tmpItem);
                    }
                }
            FileStream fs = new(_filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            JsonSerializer.Serialize(fs, tmpList, options);
            fs.Close();
            fs.Dispose();
            return true;
        }
    }
}
