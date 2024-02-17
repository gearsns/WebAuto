using CastAuto;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace WebAuto
{
    public partial class FormBrowser : Form
    {
        private const string DefaultUrlForAddedTabs = "about:blank";

        private readonly Dictionary<string, Form> formMap = [];
        private string _title;
        public FormBrowser()
        {
            InitializeComponent();
            SetForm("Excel", new FormExcel());
            SetForm("Store", new FormStore());
            SetForm("Bulk", new FormBulk());

            SwitchForm("Excel");

            string filename = Path.GetFullPath(@".\contents\gears.webauto.local\link.json");
            if (File.Exists(filename))
            {
                try
                {
                    FileStream fs = new(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    JsonSerializerOptions options = new()
                    {
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                    };
                    List<Item>? tmpList = JsonSerializer.Deserialize<List<Item>>(fs, options);
                    fs.Close();
                    fs.Dispose();
                    if (null != tmpList)
                    {
                        foreach (Item item in tmpList)
                        {
                            AddMenuItem(item.Name, item.Href);
                        }
                    }
                }
                catch { }
            }
            AddMenuItem("その他 リンク", "https://gears.webauto.local/link.html");
            AddMenuItem("WebAutoの設定", "https://gears.webauto.local/config.html");
            AddMenuItem("更新履歴", "https://gears.webauto.local/changelog.html");

            // バージョンを上げると設定ファイルの保存パスが変わるので、タイトルだけ変更する
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            _title = $"{asm.GetName().Name} - {asm.GetName().Version}";
            Text = _title;

            AddTab("https://gears.webauto.local/link.html");
        }

        private void SetForm(string name, Form form)
        {
            form.TopLevel = false;
            panelToolBody.Controls.Add(form);
            form.Size = panelToolBody.Size;
            form.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            formMap[name] = form;
        }
        private void SwitchForm(string name)
        {
            if (!formMap.TryGetValue(name, out Form? form) && form is null)
            {
                return;
            }
            splitContainer1.Panel1Collapsed = false;
            labelToolName.Text = form.Text;
            form.Show();
            foreach (KeyValuePair<string, Form> item in formMap.Where(item => item.Key != name && item.Value.Visible))
            {
                item.Value.Visible = false;
            }
        }
        public void AddTab(BrowserTabUserControl browser, int? insertIndex = null, bool bSelectdTab = true)
        {
            tabControlBrowser.SuspendLayout();

            LoadingTabPage tabPage = new(browser.Address)
            {
                Dock = DockStyle.Fill
            };
            tabPage.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;
            browser.Bounds = tabControlBrowser.Bounds;
            if (insertIndex == null)
            {
                tabControlBrowser.TabPages.Insert(tabControlBrowser.TabPages.Count - 1, tabPage);
            }
            else
            {
                tabControlBrowser.TabPages.Insert(insertIndex.Value, tabPage);
            }
            if (bSelectdTab)
            {
                tabControlBrowser.SelectedTab = tabPage;
            }
            tabControlBrowser.ResumeLayout(true);

        }
        public void AddTab(Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs e)
        {
            AddTab(new BrowserTabUserControl(e));
        }
        public void AddTab(string url, int? insertIndex = null, bool bSelectdTab = true)
        {
            AddTab(new BrowserTabUserControl(url), insertIndex, bSelectdTab);
        }
        public void RemoveTab(TabPage tabPage)
        {
            tabControlBrowser.RemoveTab(tabPage);
        }
        public void SelectTab(TabPage tabPage)
        {
            tabControlBrowser.SelectedTab = tabPage;
        }
        public BrowserTabUserControl? GetCurrentTabControl()
        {
            if (tabControlBrowser.SelectedIndex == -1)
            {
                return null;
            }
            Control tabPage = tabControlBrowser.Controls[tabControlBrowser.SelectedIndex];
            return tabPage.Controls.Count > 0
                && tabPage != tabPageAdd
                && tabPage.Controls[0] is BrowserTabUserControl control
                ? control
                : null;
        }
        private void TextBoxURL_Enter(object sender, EventArgs e)
        {
            _ = BeginInvoke(textBoxURL.SelectAll);
        }

        private void TextBoxURL_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && textBoxURL.Text.Length != 0)
            {
                if (e.Control || e.Shift || e.Alt)
                {
                    AddTab(textBoxURL.Text);
                }
                else
                {
                    LoadUrl(textBoxURL.Text);
                }
            }
        }

        private void ButtonCollapse_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel1Collapsed = true;
        }
        private void ButtonLink_Click(object sender, EventArgs e)
        {
            contextMenuStripLink.Show(PointToScreen(new Point(buttonLink.Right - contextMenuStripLink.Width, buttonLink.Bottom)));
        }
        private void ButtonExcel_Click(object sender, EventArgs e)
        {
            SwitchForm("Excel");
        }

        private void ButtonBulk_Click(object sender, EventArgs e)
        {
            SwitchForm("Bulk");
        }

        private void ButtonExtract_Click(object sender, EventArgs e)
        {
            SwitchForm("Store");
        }

        private void FormBrowser_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.WebAutoSize.Width == 0 || Properties.Settings.Default.WebAutoSize.Height == 0)
            {
                // 初回起動時にはここに来るので必要なら初期値を与えても良い。
                // 何も与えない場合には、デザイナーウインドウで指定されている大きさになる。
            }
            else
            {
                WindowState = (FormWindowState)Properties.Settings.Default.WebAutoWindowState;

                // もし前回終了時に最小化されていても、今回起動時にはNormal状態にしておく
                if (WindowState == FormWindowState.Minimized)
                {
                    WindowState = FormWindowState.Normal;
                }

                Location = Properties.Settings.Default.WebAutoLocation;
                Size = Properties.Settings.Default.WebAutoSize;
            }
        }

        private void FormBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.WebAutoLocation = Location;
                Properties.Settings.Default.WebAutoSize = Size;
            }
            else
            {
                Properties.Settings.Default.WebAutoLocation = RestoreBounds.Location;
                Properties.Settings.Default.WebAutoSize = RestoreBounds.Size;
            }
            if (WindowState == FormWindowState.Minimized)
            {
                Properties.Settings.Default.WebAutoWindowState = (ulong)FormWindowState.Normal;
            }
            else
            {
                Properties.Settings.Default.WebAutoWindowState = (ulong)WindowState;
            }
            Properties.Settings.Default.Save();
        }

        public delegate void InputDataCallback();
        private CancellationTokenSource _cancellationTokenSource = new();
        private uint _runningFrame;
        private BrowserTabUserControl? _runningControl;

        public void CancelInputData()
        {
            _cancellationTokenSource.Cancel();
        }
        public void InputData(DataGridView MyDataGridViewInput, InputDataCallback? inputDataCallback = null)
        {
            if (0 != _runningFrame)
            {
                Program.ShowBalloonTip("WebAuto", "入力中です", ToolTipIcon.Error);
                inputDataCallback?.Invoke();
                return;
            }
            if (GetCurrentTabControl() is not BrowserTabUserControl control)
            {
                Program.ShowBalloonTip("WebAuto", "対象ページが表示されていません。");
                inputDataCallback?.Invoke();
                return;
            }
            if (control.WebView2 is not Microsoft.Web.WebView2.WinForms.WebView2 webView2)
            {
                Program.ShowBalloonTip("WebAuto", "対象ページが表示されていません。");
                inputDataCallback?.Invoke();
                return;
            }
            if (MyDataGridViewInput.Rows.Count <= 1)
            {
                Program.ShowBalloonTip("WebAuto", "データが登録されていません");
                inputDataCallback?.Invoke();
                return;
            }
            if (control.Parent is not LoadingTabPage tabPage)
            {
                inputDataCallback?.Invoke();
                return;
            }
            _cancellationTokenSource = new();
            int inputPageWaitTime = 1000;
            try
            {
                inputPageWaitTime = Math.Max(int.Parse(Store.GetInstance().GetValue("InputPageWaitTime")), inputPageWaitTime);
            }
            catch { }
            MyDataGridViewInput.ReadOnly = true;
            try
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        if (null == control)
                        {
                            return;
                        }
                        _runningControl = control;
                        JsonSerializerOptions options = new()
                        {
                            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                        };
                        {
                            string filename = Path.GetFullPath(@".\script\input.js");
                            if (File.Exists(filename))
                            {
                                StreamReader sr = new(filename);
                                string input_script_contents = sr.ReadToEnd();
                                sr.Close();
                                await this.Invoke(async () =>
                                {
                                    _ = await control.WebView2.CoreWebView2.ExecuteScriptAsync(input_script_contents);
                                });
                            }
                        }
                        this.Invoke(() =>
                        {
                            _runningFrame = control.WebView2.CoreWebView2.FrameId;
                        });
                        int columnIndex = (null == MyDataGridViewInput.CurrentCell)
                        ? 0
                        : MyDataGridViewInput.CurrentCell.ColumnIndex;
                        foreach (DataGridViewRow row in MyDataGridViewInput.Rows.Cast<DataGridViewRow>())
                        {
                            _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                            if (row.IsNewRow)
                            {
                                continue;
                            }
                            Invoke(() =>
                            {
                                MyDataGridViewInput.CurrentCell = MyDataGridViewInput[columnIndex, row.Index];
                            });
                            string k = row.GetCellString("ColumnKey");
                            if (k.Length == 0)
                            {
                                continue;
                            }
                            string v = row.GetCellString("ColumnValue")
                                .Replace("\r", "")
                                .Replace("\n", "\r\n");
                            if (k == "(frame)")
                            {
                                //_runningFrame = frame = await control.GetFrame(v) ?? frame;
                            }
                            else if (k == "URL")
                            {
                                control.LoadEnd = false;
                                this.Invoke(() =>
                                {
                                    control.LoadUrl(v);
                                });
                                for (int i = 0; i < 100; ++i)
                                {
                                    await Task.Delay(inputPageWaitTime);
                                    if (control.LoadEnd)
                                    {
                                        break;
                                    }
                                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                                }
                                _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                                await Task.Delay(inputPageWaitTime);
                            }
                            else
                            {
                                for (int i = 0; i < 100; ++i)
                                {
                                    if (!tabPage.Loading)
                                    {
                                        break;
                                    }
                                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                                    await Task.Delay(100);
                                }
                                Dictionary<string, string> dataList = new()
                                {
                                    ["key"] = k,
                                    ["val"] = v
                                };
                                string script = "{" +
                                $@"const data = {JsonSerializer.Serialize(dataList, options)};" +
                                    "if(typeof WebAutoSetValue === 'undefined'){" +
                                    "const e=document.querySelector(`${data.key},*[name='${data.key}']`);" +
                                    "if(e.type == 'checkbox'){ e.checked=data.val.length > 0 }" +
                                    "else if(e.value !== undefined){ e.value=data.val }" +
                                    " e.focus(); e.blur(); e.onchange();" +
                                    "}else{ WebAutoSetValue(data.key, data.val) }}";
                                await control.ExecuteScriptAsync(script, _cancellationTokenSource);
                            }
                        }
                        Invoke(() =>
                        {
                            MyDataGridViewInput.ReadOnly = false;
                        });
                    }
                    catch (OperationCanceledException)
                    {
                        Invoke(() =>
                        {
                            _ = control.WebView2.CoreWebView2.ExecuteScriptAsync("WebAutoCancel()");
                            Program.ShowBalloonTip("WebAuto", "入力をキャンセルしました。");
                        });
                    }
                    catch { }
                    finally
                    {
                        Invoke(() =>
                        {
                            inputDataCallback?.Invoke();
                            _runningFrame = 0;
                            _runningControl = null;
                        });
                    }
                }, _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException) { }
        }
        private void LoadUrl(string url)
        {
            if (GetCurrentTabControl() is BrowserTabUserControl btc)
            {
                btc.LoadUrl(url);
            }
            else
            {
                AddTab(url);
            }
        }

        private void TabControlBrowser_AddedNewPage()
        {
            AddTab(DefaultUrlForAddedTabs);
        }

        private void EnabledBack(bool enabled)
        {
            buttonBack.Enabled = enabled;
        }
        private void EnabledForward(bool enabled)
        {
            buttonForward.Enabled = enabled;
        }

        private void TabControlBrowser_Selected(object sender, TabControlEventArgs e)
        {
            if (GetCurrentTabControl() is BrowserTabUserControl btc)
            {
                EnabledBack(btc.CanGoBack);
                EnabledForward(btc.CanGoForward);
                textBoxURL.Text = btc.Address;
                Text = $"{_title} - {e.TabPage?.Text}";
            }
            else
            {
                EnabledBack(false);
                EnabledForward(false);
                textBoxURL.Text = string.Empty;
                Text = _title;
            }
            toolStripStatusLabelStatusText.Text = string.Empty;
            buttonReload.BackgroundImage = Properties.Resources.refresh;
        }
        public void TitleChanged(BrowserTabUserControl btc, string text)
        {
            if (btc == GetCurrentTabControl())
            {
                Text = $"{_title} - {text}";
            }
        }
        public void SetStatusText(BrowserTabUserControl btc, string text)
        {
            if (btc == GetCurrentTabControl())
            {
                toolStripStatusLabelStatusText.Text = text;
            }
        }
        public void SetUrlText(BrowserTabUserControl btc, string text)
        {
            if (btc == GetCurrentTabControl())
            {
                textBoxURL.Text = text;
            }
        }
        public void SetIsLoading(BrowserTabUserControl btc, bool isLoading)
        {
            if (btc == GetCurrentTabControl())
            {
                buttonReload.BackgroundImage = isLoading ?
                Properties.Resources.cancel:
                Properties.Resources.refresh;
            }
        }
        public void SetCanGoBackAndForward(bool? CanGoBack, bool? CanGoForward)
        {
            if (null != CanGoBack)
            {
                EnabledBack((bool)CanGoBack);
            }
            if (null != CanGoForward)
            {
                EnabledForward((bool)CanGoForward);
            }
        }
        private void ButtonBack_Click(object sender, EventArgs e)
        {
            GetCurrentTabControl()?.Back();
        }

        private void ButtonForward_Click(object sender, EventArgs e)
        {
            GetCurrentTabControl()?.Forward();
        }

        private void buttonReload_Click(object sender, EventArgs e)
        {
            GetCurrentTabControl()?.Reload();
        }
        private void ButtonHome_Click(object sender, EventArgs e)
        {
            LoadUrl("https://gears.webauto.local/link.html");
        }
        private void ButtonGo_Click(object sender, EventArgs e)
        {
            LoadUrl(textBoxURL.Text);
        }
        //
        private class Item
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = "";
            [JsonPropertyName("href")]
            public string Href { get; set; } = "";
        }
        private void AddMenuItem(string name, string href)
        {
            ToolStripMenuItem button = new()
            {
                Name = name,
                Text = name,
                Tag = href
            };
            button.Click += MenuItem_Click;
            _ = contextMenuStripLink.Items.Add(button);
        }
        private void MenuItem_Click(object? sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem button
                && button.Tag is string href)
            {
                AddTab(href);
            }
        }
    }
}
