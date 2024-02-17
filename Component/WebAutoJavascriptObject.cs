using AngleSharp.Io;
using AngleSharp;
using System.Net;
using System.Runtime.InteropServices;

namespace WebAuto
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class WebAutoJavascriptObject(BrowserTabUserControl btc)
    {
        private readonly BrowserTabUserControl _browserTabUserControl = btc;


        public string GetFolderName()
        {
            FolderBrowserDialog fd = new()
            {
                Description = "フォルダを選択してください"
            };
            DialogResult ret = fd.ShowDialog();
            return ret == DialogResult.OK ? fd.SelectedPath : "";
        }

        public void SetStoreValue(string key, string value, bool isCypt)
        {
            Store.GetInstance().SetValue(key, value, isCypt);
        }
        public string GetStoreValue(string key)
        {
            return Store.GetInstance().GetValue(key);
        }
        public void CompletedScript()
        {
            _browserTabUserControl.CompletedScript();
        }

        public async void DownloadFileAsync(string filename, string url)
        {
            string outputPath = Store.GetInstance().GetValue("WebAutoOutputFolder");
            if (string.IsNullOrEmpty(outputPath))
            {
                return;
            }
            IConfiguration config = Configuration.Default
            .WithDefaultCookies()
            .WithDefaultLoader(new LoaderOptions
            {
                IsResourceLoadingEnabled = true,
            });
            Dictionary<string, string> cookies_map = [];
            using IBrowsingContext context = BrowsingContext.New(config);
            await _browserTabUserControl.Invoke(async () =>
            {
                foreach (Microsoft.Web.WebView2.Core.CoreWebView2Cookie? cookie
                    in await _browserTabUserControl.WebView2.CoreWebView2.CookieManager.GetCookiesAsync(""))
                {
                    if(null != cookie)
                    {
                        if (cookies_map.ContainsKey(cookie.Domain))
                        {
                            cookies_map[cookie.Domain] += $@"{cookie.Name}={cookie.Value},";
                        }
                        else
                        {
                            cookies_map.Add(cookie.Domain, $@"{cookie.Name}={cookie.Value},");
                        }
                    }
                }
            });
            foreach (KeyValuePair<string, string> item in cookies_map)
            {
                try
                {
                    context.SetCookie(new AngleSharp.Dom.Url($"https://{item.Key}"), item.Value);
                }
                catch { }
            }
            try
            {
                DocumentRequest req = DocumentRequest.Get(new AngleSharp.Dom.Url(url));
                IDocumentLoader? loader = context.GetService<IDocumentLoader>();
                IDownload download = loader!.FetchAsync(req);
                using AngleSharp.Io.IResponse response = await download.Task;
                // Headersで判定
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string fullpath = Path.Combine(outputPath, filename);
                    if (Directory.GetParent(fullpath) is DirectoryInfo diParent
                        && !diParent.Exists)
                    {
                        diParent.Create();
                    }
                    FileStream fs = new(fullpath, FileMode.Create);
                    BinaryWriter sw = new(fs);
                    response.Content.CopyTo(fs);
                    sw.Close();
                    fs.Close();
                    _browserTabUserControl.Invoke(() =>
                    {
                        Program.ShowBalloonTip("WebAuto", $"${filename}の\nダウンロードが完了しました。");
                    });
                }
                else
                {
                    _browserTabUserControl.Invoke(() =>
                    {
                        Program.ShowBalloonTip("WebAuto", $"${filename}の\nダウンロードに失敗しました。", ToolTipIcon.Error);
                    });
                }
            }
            catch { }
        }
    }
}
