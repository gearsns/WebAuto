using Microsoft.Web.WebView2.Core;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace WebAuto
{
    public class CookieSessionStore()
	{
#if DEBUG
        private readonly string _filename = Path.Combine(Application.StartupPath, "cookie.json");
#else
        private readonly string _filename = Path.Combine(Application.UserAppDataPath, "..\\store\\cookie.json");
#endif
        private static readonly CookieSessionStore _cookieSessionStore = new();
        public static CookieSessionStore GetInstance()
        {
            return _cookieSessionStore;
        }
        private class ItemInfo
        {
            public string Name { get; set; } = "";
            public string Value { get; set; } = "";
            public string Domain { get; set; } = "";
            public string Path { get; set; } = "";
        }
        private readonly List<ItemInfo> _itemList = [];

        public bool Load()
        {
            try
            {
                if (File.Exists(_filename))
                {
                    // 8時間以上前のcookieは読み込まない
                    if (File.GetLastWriteTime(_filename) > DateTime.Now.AddHours(-8))
                    {
                        _itemList.Clear();
                        return true;
                    }
                    FileStream fs = new(_filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    List<ItemInfo>? tmpList = JsonSerializer.Deserialize<List<ItemInfo>>(fs, new JsonSerializerOptions()
                    {
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                    });
                    fs.Close();
                    fs.Dispose();
                    _itemList.Clear();
                    if (null != tmpList)
                    {
                        _itemList.AddRange(tmpList);
                    }
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
            try
            {
                FileStream fs = new(_filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                JsonSerializer.Serialize(fs, _itemList, new JsonSerializerOptions()
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                });
                fs.Close();
                fs.Dispose();
            }
            catch
            {
                return false;
            }
            return true;
        }
        public void AddOrUpdateCookie(CoreWebView2CookieManager coreWebView2CookieManager)
		{
            foreach(ItemInfo item in _itemList)
            {
                CoreWebView2Cookie cookie = coreWebView2CookieManager.CreateCookie(item.Name, item.Value, item.Domain, item.Path);
                coreWebView2CookieManager.AddOrUpdateCookie(cookie);
            }
        }
        public async Task<bool> StoreCookies(CoreWebView2CookieManager coreWebView2CookieManager)
        {
            foreach (CoreWebView2Cookie? cookie in await coreWebView2CookieManager.GetCookiesAsync(null))
            {
                if (null != cookie)
                {
                    ItemInfo? item = _itemList.Find(x =>
                        x.Name == cookie.Name
                        && x.Domain == cookie.Domain
                        && x.Path == cookie.Path
                    );
                    if (null == item)
                    {
                        _itemList.Add(new ItemInfo { Name = cookie.Name, Value = cookie.Value, Domain = cookie.Domain, Path = cookie.Path });
                    }
                    else
                    {
                        item.Value = cookie.Value;
                    }
                }
            }
            return true;
        }
    }
}
