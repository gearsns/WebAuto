using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Text;

namespace WebAuto
{
    public partial class BrowserTabUserControl : UserControl
    {
        public WebView2 WebView2 => webView2;
        public event EventHandler<EventArgs>? WebViewFirstInitialized;
        private readonly string InitUrl = "";

        public BrowserTabUserControl()
        {
            InitializeComponent();
        }

        public BrowserTabUserControl(string url)
        {
            InitializeComponent();
            InitUrl = url;
        }

        public BrowserTabUserControl(CoreWebView2NewWindowRequestedEventArgs e)
        {
            InitializeComponent();
            CoreWebView2Deferral deferral = e.GetDeferral();
            WebViewFirstInitialized += (s, args) =>
            {
                e.NewWindow = webView2.CoreWebView2;
                e.Handled = true;
                deferral.Complete();
            };
        }

        private bool IsScriptRunning = false;

        public void CompletedScript()
        {
            IsScriptRunning = false;
        }
        public async Task ExecuteScriptAsync(string script, CancellationTokenSource CancellationTokenSource)
        {
            IsScriptRunning = true;
            await Invoke(async () => await webView2.CoreWebView2.ExecuteScriptAsync(script));
            await Task.Run(() =>
            {
                while (IsScriptRunning)
                {
                    CancellationTokenSource.Token.ThrowIfCancellationRequested();
                    Task.Delay(10).Wait();
                }
            });
        }

        public bool LoadEnd { set; get; } = true;

        private static string loaded_script_contents = "";
        private static bool loaded_script_contents_is = false;
        private static void LoadScript()
        {
            loaded_script_contents_is = true;
            try
            {
                string filename = Path.GetFullPath(@".\script\loaded.js");
                if (File.Exists(filename))
                {
                    StreamReader sr = new(filename);
                    loaded_script_contents += sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch { }
        }

        private void WebView2_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            CoreWebView2 coreWebView2 = webView2.CoreWebView2;
            try
            {
            foreach (string name in Directory.GetDirectories(Path.GetFullPath(@".\contents")))
            {
                coreWebView2.SetVirtualHostNameToFolderMapping(Path.GetFileName(name), name, CoreWebView2HostResourceAccessKind.Allow);
            }
            }
            catch { }
            coreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
            coreWebView2.WindowCloseRequested += CoreWebView2_WindowCloseRequested;
            coreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
            coreWebView2.SourceChanged += CoreWebView2_SourceChanged;
            coreWebView2.StatusBarTextChanged += CoreWebView2_StatusBarTextChanged;
            coreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
            coreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            coreWebView2.FrameNavigationStarting += CoreWebView2_FrameNavigationStarting;
            coreWebView2.FrameNavigationCompleted += CoreWebView2_FrameNavigationCompleted;
            coreWebView2.HistoryChanged += CoreWebView2_HistoryChanged;
            coreWebView2.ContextMenuRequested += CoreWebView2_ContextMenuRequested;
            coreWebView2.Settings.AreHostObjectsAllowed = true;
            coreWebView2.Settings.IsPasswordAutosaveEnabled = true;
            coreWebView2.Settings.IsStatusBarEnabled = false;
            coreWebView2.Settings.IsWebMessageEnabled = true;
            CookieSessionStore cookieSessionStore = CookieSessionStore.Instance;
            cookieSessionStore.AddOrUpdateCookie(webView2.CoreWebView2.CookieManager);
            WebViewFirstInitialized?.Invoke(this, new EventArgs());
            if (!loaded_script_contents_is)
            {
                LoadScript();
            }
            if (webView2.CoreWebView2 is not null && loaded_script_contents.Length > 0)
            {
                _ = webView2.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(loaded_script_contents);
            }
            coreWebView2.AddHostObjectToScript("WebAuto", new WebAutoJavascriptObject(this));
        }

        private void CoreWebView2_ContextMenuRequested(object? sender, CoreWebView2ContextMenuRequestedEventArgs e)
        {
            CoreWebView2ContextMenuTargetKind context = e.ContextMenuTarget.Kind;
            if (e.ContextMenuTarget.HasLinkUri)
            {
                IList<CoreWebView2ContextMenuItem> menuList = e.MenuItems;
                int index = 0;
                for (; index < menuList.Count; index++)
                {
                    if (menuList[index].Name == "openLinkInNewWindow")
                    {
                        index++;
                        break;
                    }
                }
                CoreWebView2ContextMenuItem newItem =
                    webView2.CoreWebView2.Environment.CreateContextMenuItem("リンクを新しいバックグランドのタブで開く", null,
                    CoreWebView2ContextMenuItemKind.Command);
                newItem.CustomItemSelected += delegate (object? sender, object ex)
                {
                    string link = e.ContextMenuTarget.LinkUri;
                    Invoke(() =>
                    {
                        if (FindForm() is FormBrowser owner)
                        {
                            owner.AddTab(link, null, false);
                        }
                    });
                };
                menuList.Insert(index, newItem);
            }
        }

        private void CoreWebView2_HistoryChanged(object? sender, object e)
        {
            Invoke(() =>
            {
                if (FindForm() is FormBrowser owner)
                {
                    owner.SetCanGoBackAndForward(CanGoBack, CanGoForward);
                }
            });
        }

        private void CoreWebView2_SourceChanged(object? sender, CoreWebView2SourceChangedEventArgs e)
        {
            Invoke(() =>
            {
                if (FindForm() is FormBrowser owner)
                {
                    owner.SetUrlText(this, webView2.CoreWebView2.Source);
                }
            });
        }

        private void LoadStarting()
        {
            Invoke(() =>
            {
                if (FindForm() is FormBrowser owner)
                {
                    owner.SetIsLoading(this, true);
                }
                if (Parent is LoadingTabPage tabPage)
                {
                    tabPage.Loading = true;
                }
            });
        }
        private void LoadCompleted()
        {
            _ = Invoke(async () =>
            {
                if (FindForm() is FormBrowser owner)
                {
                    owner.SetCanGoBackAndForward(CanGoBack, CanGoForward);
                    owner.SetIsLoading(this, false);
                }
                if (Parent is LoadingTabPage tabPage)
                {
                    tabPage.Loading = false;
                }
                CompletedScript();
                CookieSessionStore cookieSessionStore = CookieSessionStore.Instance;
                _ = await cookieSessionStore.StoreCookies(webView2.CoreWebView2.CookieManager);
            });
        }

        private void CoreWebView2_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
                {
            LoadStarting();
                }
        private void CoreWebView2_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
                {
            LoadCompleted();
        }

        private void CoreWebView2_FrameNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
            {
            LoadStarting();
                }

        private void CoreWebView2_FrameNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
                {
            LoadCompleted();
            LoadEnd = true;
        }

        private void CoreWebView2_StatusBarTextChanged(object? sender, object e)
        {
            Invoke(() => 
            {
                if (FindForm() is FormBrowser owner)
                {
                    owner.SetStatusText(this, webView2.CoreWebView2.StatusBarText);
                }
            });
        }

        private void CoreWebView2_DocumentTitleChanged(object? sender, object e)
        {
            Invoke(() =>
            {
                if (Parent is TabPage tabPage)
                {
                    tabPage.ToolTipText = tabPage.Text = webView2.CoreWebView2.DocumentTitle;
                }
                if (FindForm() is FormBrowser owner)
                {
                    owner.TitleChanged(this, webView2.CoreWebView2.DocumentTitle);
                }
            });
        }

        private void CoreWebView2_WindowCloseRequested(object? sender, object e)
        {
            Invoke(() =>
            {
                if (FindForm() is FormBrowser owner && Parent is TabPage tabPage)
                {
                    owner.SelectPrevTab(tabPage);
                    owner.RemoveTab(tabPage);
                }
            });
        }

        private void CoreWebView2_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            Invoke(() =>
            {
                if (FindForm() is FormBrowser owner && Parent is TabPage tabPage)
                {
                    owner.AddTab(e, tabPage);
                }
            });
        }

        private async void BrowserTabUserControl_Load(object sender, EventArgs e)
        {
            CoreWebView2EnvironmentOptions options = new();
            Uri proxyjs_uri = new($"{Application.UserAppDataPath}\\..\\proxy\\proxy.js");
            string proxyjs_file = $"{proxyjs_uri.AbsolutePath}";
            if (File.Exists(proxyjs_file))
            {
                try
                {
                    StreamReader sr = new(proxyjs_file);
                    string ProxyPacScript = sr.ReadToEnd();
                    sr.Close();
                    byte[] bytes = Encoding.UTF8.GetBytes(ProxyPacScript);
                    string base64 = Convert.ToBase64String(bytes);
                    options.AdditionalBrowserArguments = $"--proxy-pac-url=\"data:application/x-ns-proxy-autoconfig;base64,{base64}\"";
                }
                catch { }
            }
            CoreWebView2Environment env = await CoreWebView2Environment.CreateAsync(null, null, options);
            try
            {
            await webView2.EnsureCoreWebView2Async(env);
            }
            catch { }
            if(InitUrl.Length > 0)
            {
                try
                {
                    webView2.Source = new Uri(InitUrl);
                }
                catch { }
            }
        }
        public void LoadUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                try
                {
                    webView2.Source = new Uri(url);
                }
                catch { }
            }
        }
        public void Reload()
        {
            webView2.CoreWebView2?.Reload();
        }

        public void Back()
        {
            webView2.CoreWebView2?.GoBack();
        }
        public void Forward()
        {
            webView2.CoreWebView2?.GoForward();
        }

        public bool CanGoBack => null != webView2.CoreWebView2 && webView2.CoreWebView2.CanGoBack;
        public bool CanGoForward => null != webView2.CoreWebView2 && webView2.CoreWebView2.CanGoForward;
        public string Address => null == webView2.CoreWebView2 ? InitUrl : webView2.CoreWebView2.Source;
    }
}
