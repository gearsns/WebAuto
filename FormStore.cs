using CastAuto;
using System.Text.Json.Nodes;

namespace WebAuto
{
    public partial class FormStore : Form
    {
        public FormStore()
        {
            InitializeComponent();
            MyDataGridViewInput.InitEvent();

            ItemPatternInfo.AddEvent += ItemPatternInfo_AddEvent;

            LoadData();
        }

        private async void FormStoreEdit_Save(string name)
        {
            if (comboBoxSheet.SelectedIndex >= 0
                && comboBoxSheet.Items[comboBoxSheet.SelectedIndex] is ItemPattern itemPattern)
            {
                if (name.Length == 0)
                {
                    _ = ItemPatternInfo.ItemPatterns.Remove(itemPattern);
                }
                else if (ItemPatternInfo.ItemPatterns.Find(n => n == itemPattern) is ItemPattern item)
                {
                    item.Name = name;
                    item.Items.Clear();
                    foreach (DataGridViewRow row in MyDataGridViewInput.Rows.Cast<DataGridViewRow>())
                    {
                        if (row.IsNewRow)
                        {
                            continue;
                        }
                        item.Items.Add(new ItemValue
                        {
                            Name = row.GetCellString("ColumnName"),
                            Key = row.GetCellString("ColumnKey"),
                            Value = row.GetCellString("ColumnValue"),
                        });
                    }
                }
                UpdateList();
                await ItemPatternInfo.Save();
            }
        }

        private void UpdateList()
        {
            comboBoxSheet.DataSource = null;
            comboBoxSheet.DataSource = ItemPatternInfo.ItemPatterns;
        }

        private void ItemPatternInfo_AddEvent(ItemPattern obj)
        {
            UpdateList();
            if (comboBoxSheet.Items.Count > 0)
            {
                comboBoxSheet.SelectedIndex = comboBoxSheet.Items.Count - 1;
            }
        }

        private void SwitchButton(bool bEnable)
        {
            buttonInput.Visible = bEnable;
            buttonEdit.Visible = bEnable;
            buttonExtract.Visible = bEnable;
            buttonClear.Visible = bEnable;
            buttonCancel.Visible = !bEnable;
            comboBoxSheet.Enabled = bEnable;
            MyDataGridViewInput.ReadOnly = !bEnable;
        }

        private void ButtonInput_Click(object sender, EventArgs e)
        {
            if (null != Parent && Parent.FindForm() is FormBrowser owner)
            {
                SwitchButton(false);
                owner.InputData(MyDataGridViewInput, () =>
                {
                    SwitchButton(true);
                });
            }
        }

        private async void LoadData()
        {
            await ItemPatternInfo.Load();
            UpdateList();
            if (comboBoxSheet.Items.Count > 0)
            {
                comboBoxSheet.SelectedIndex = 0;
            }
        }
        private void SetData()
        {
            buttonEdit.Enabled = false;
            if (comboBoxSheet.SelectedIndex >= 0
                && comboBoxSheet.Items[comboBoxSheet.SelectedIndex] is ItemPattern itemPattern)
            {
                SetData(itemPattern.Items);
                buttonEdit.Enabled = true;
            }
            else
            {
                SetData([]);
            }
        }

        private void ClearData()
        {
            MyDataGridViewInput.Rows.Clear();
            MyDataGridViewInput.UndoClear();
            MyDataGridViewInput.RedoClear();
            MyDataGridViewInput.Enabled = true;
        }

        private void SetData(List<ItemValue> items)
        {
            ClearData();
            List<DataGridViewRow> rows = new();
            foreach (ItemValue item in items)
            {
                DataGridViewRow dataGridViewRow = new();
                dataGridViewRow.CreateCells(MyDataGridViewInput, new string[3]
                {
                    item.Name, item.Key, item.Value
                });
                rows.Add(dataGridViewRow);
            }
            MyDataGridViewInput.Rows.AddRange(rows.ToArray());
        }
        private void ComboBoxSheet_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetData();
        }

        private void ButtonEdit_Click(object sender, EventArgs e)
        {
            if (comboBoxSheet.SelectedIndex >= 0
                && comboBoxSheet.Items[comboBoxSheet.SelectedIndex] is ItemPattern itemPattern)
            {
                FormStoreEdit formStoreEdit = new();
                formStoreEdit.Save += FormStoreEdit_Save;
                formStoreEdit.Show(this, itemPattern.Name);
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            if (null != Parent && Parent.FindForm() is FormBrowser owner)
            {
                owner.CancelInputData();
            }
        }

        private async void ButtonExtract_Click(object sender, EventArgs e)
        {
            if (null == Parent
                || Parent.FindForm() is not FormBrowser owner
                || owner.GetCurrentTabControl() is not BrowserTabUserControl control)
            {
                Program.ShowBalloonTip("WebAuto", "ページが読み込まれていません", ToolTipIcon.Warning);
                return;
            }
            if (control.WebView2 is not Microsoft.Web.WebView2.WinForms.WebView2 webView2)
            {
                Program.ShowBalloonTip("WebAuto", "対象ページが表示されていません。");
                return;
            }
            Dictionary<string, string> script_files = [];
            try
            {
                string filename = Path.GetFullPath(@".\script\extract.js");
                if (File.Exists(filename))
                {
                    StreamReader sr = new(filename);
                    string input_script_contents = sr.ReadToEnd();
                    sr.Close();
                    await Invoke(async () =>
                    {
                        _ = await control.WebView2.CoreWebView2.ExecuteScriptAsync(input_script_contents);
                    });
                }
            }
            catch { }
            try
            {
                List<DataGridViewRow> rows = [];
                int column_count = MyDataGridViewInput.ColumnCount;
                string[] table_header_name = new string[column_count];
                try
                {
                    string text = "";
                    await Invoke(async () =>
                    {
                        text = await control.WebView2.CoreWebView2.ExecuteScriptAsync("getWebAutoData()");
                    });
                    if (JsonNode.Parse(text) is JsonNode jsonNode)
                    {
                        string[] table_header_keys = new string[column_count];
                        if (jsonNode["header"] is JsonArray headerList)
                        {
                            int index = 0;
                            foreach (JsonNode? line in headerList)
                            {
                                if (line == null)
                                {
                                    continue;
                                }
                                table_header_name[index] = line.ToString("name");
                                table_header_keys[index] = line.ToString("key");
                                ++index;
                                if (index == column_count)
                                {
                                    break;
                                }
                            }
                        }
                        if (jsonNode["data"] is JsonArray dataList)
                        {
                            string[] data = new string[column_count];
                            foreach (JsonNode? line in dataList)
                            {
                                if (line == null)
                                {
                                    continue;
                                }
                                for (int i = 0; i < column_count; ++i)
                                {
                                    data[i] = "";
                                    try
                                    {
                                        if (line.GetType() == typeof(JsonArray))
                                        {
                                            data[i] = line.ToString(i);
                                        }
                                        else if (table_header_keys[i] != null)
                                        {
                                            data[i] = line.ToString(table_header_keys[i]);
                                        }
                                    }
                                    catch { }
                                }
                                DataGridViewRow dataGridViewRow = new();
                                dataGridViewRow.CreateCells(MyDataGridViewInput, data);
                                rows.Add(dataGridViewRow);
                            }
                        }
                    }
                }
                catch { }
                ClearData();
                for (int i = 0; i < column_count; ++i)
                {
                    MyDataGridViewInput.Columns[i].HeaderText = table_header_name[i];
                }
                MyDataGridViewInput.Rows.AddRange(rows.ToArray());

                ItemPattern itemPattern = new();
                foreach (DataGridViewRow item in MyDataGridViewInput.Rows)
                {
                    itemPattern.Items.Add(new ItemValue()
                    {
                        Name = item.GetCellString(0),
                        Key = item.GetCellString(1),
                        Value = item.GetCellString(2),
                    });
                }
                if (itemPattern.Items.Count > 0)
                {
                    ItemPattern? item = ItemPatternInfo.ItemPatterns.FirstOrDefault(item => item.Name == "一時保存");
                    if (null != item)
                    {
                        ItemPatternInfo.ItemPatterns.Remove(item);
                    }
                    itemPattern.Name = "一時保存";
                    ItemPatternInfo.ItemPatterns.Insert(0, itemPattern);
                    await ItemPatternInfo.Save();
                    UpdateList();
                    if (comboBoxSheet.Items.Count > 0)
                    {
                        comboBoxSheet.SelectedIndex = 0;
                    }
                }
            }
            catch { }
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            ClearData();
        }

        private void ToolStripMenuItemToday_Click(object sender, EventArgs e)
        {
            MyDataGridViewInput.SetSelectColumnCellsValue(ColumnValue.Index, DateTime.Now.ToString("yyyy-MM-dd"));
        }

        private void ToolStripMenuItemTime_Click(object sender, EventArgs e)
        {
            MyDataGridViewInput.SetSelectColumnCellsValue(ColumnValue.Index, DateTime.Now.ToString("HH:mm"));
        }

        private void ToolStripMenuItemMonthFirstDay_Click(object sender, EventArgs e)
        {
            MyDataGridViewInput.SetSelectColumnCellsValue(ColumnValue.Index, DateTime.Now.ToString("yyyy-MM-01"));
        }

        private void ToolStripMenuItemMonthLastDay_Click(object sender, EventArgs e)
        {
            DateTime dateTime = DateTime.Now;
            int days = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
            MyDataGridViewInput.SetSelectColumnCellsValue(ColumnValue.Index, dateTime.ToString($"yyyy-MM-{days}"));
        }

        private void ToolStripMenuItemPreMonthFirstDay_Click(object sender, EventArgs e)
        {
            DateTime dateTime = DateTime.Now.AddMonths(-1);
            MyDataGridViewInput.SetSelectColumnCellsValue(ColumnValue.Index, dateTime.ToString($"yyyy-MM-01"));
        }

        private void ToolStripMenuItemPreMonthLastDay_Click(object sender, EventArgs e)
        {
            DateTime dateTime = DateTime.Now.AddMonths(-1);
            int days = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
            MyDataGridViewInput.SetSelectColumnCellsValue(ColumnValue.Index, dateTime.ToString($"yyyy-MM-{days}"));
        }

        private void ToolStripMenuItemMonthCalendarCell_Click(object sender, EventArgs e)
        {
            FormMonthCalendar form = new();
            DialogResult ret = form.ShowDialog(this);
            if (ret == DialogResult.OK)
            {
                MyDataGridViewInput.SetSelectColumnCellsValue(ColumnValue.Index, form.SelectedDateTime.ToString($"yyyy-MM-dd"));
            }
        }
    }
}
