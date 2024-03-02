using ExcelDataReader;
using System.Data;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Unicode;

namespace WebAuto
{
    public partial class FormBulk : Form
    {
        public FormBulk()
        {
            InitializeComponent();
            MyDataGridViewInput.InitEvent();
            SetTemplateList();
        }
        private void SetTemplateList()
        {
            try
            {
                Uri uri = new($"{Application.StartupPath}\\Template");
                string[] dirs = Directory.GetFiles(uri.AbsolutePath.ToString(), "*.json");
                List<string> template_list = [];
                foreach (string f in dirs)
                {
                    try
                    {
                        template_list.Add(Path.GetFileNameWithoutExtension(f));
                    }
                    catch { }
                }
                comboBoxTemplate.Items.Clear();
                foreach (string item in template_list)
                {
                    _ = comboBoxTemplate.Items.Add(item);
                }
                comboBoxTemplate.SelectedIndex = 0;
            }
            catch { }
        }
        private void ButtonInput_Click(object sender, EventArgs e)
        {
            if (Parent?.FindForm() is FormBrowser owner)
            {
                buttonInput.Visible = false;
                buttonCancel.Visible = true;
                owner.InputData(MyDataGridViewInput, () =>
                {
                    buttonInput.Visible = true;
                    buttonCancel.Visible = false;
                });
            }
        }

        private class ItemValue
        {
            public string Name { get; set; } = "";
            public string Value { get; set; } = "";
        }
        private class Item
        {
            public List<ItemValue> Cond { get; set; } = [];
            public List<ItemValue> Field { get; set; } = [];
        }
        private class Template
        {
            public string? Filename { get; set; }
            public string Sheetname { get; set; } = "";
            public List<Item> Item { get; set; } = [];
        }

        private readonly JsonSerializerOptions options = new()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };
        private void ButtonExcel_Click(object sender, EventArgs e)
        {
            if (null == comboBoxTemplate.SelectedItem)
            {
                return;
            }
            List<string[]> list = [];
            try
            {
                string _filename = $"{Application.StartupPath}\\template\\{comboBoxTemplate.SelectedItem}.json";
                FileStream fs = new(_filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                Template? template = JsonSerializer.Deserialize<Template>(fs, options);
                fs.Close();
                fs.Dispose();

                if (null == template)
                {
                    return;
                }
                string sheetname = template.Sheetname;
                string file = template.Filename ?? Store.Instance.GetValue("TemplateDefaultExelFilename");
                if (!File.Exists(file))
                {
                    Program.ShowBalloonTip("WebAuto", $"EXCELファイル({file})が見つかりませんでした。", ToolTipIcon.Error);
                    return;
                }
                using FileStream stream = new(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream, new ExcelReaderConfiguration()
                {
                    //デフォルトのエンコードは西ヨーロッパ言語の為、日本語が文字化けする
                    //オプション設定でエンコードをシフトJISに変更する
                    FallbackEncoding = Encoding.GetEncoding("Shift_JIS")
                });
                for (int i = 0; i < reader.ResultsCount; i++)
                {
                    if (sheetname != reader.Name)
                    {
                        _ = reader.NextResult();
                        continue;
                    }
                    int maxnum = (int)numericUpDownMaxnum.Value;
                    List<string> header = [];
                    bool bHead = true;
                    while (reader.Read())
                    {
                        Dictionary<string, string> data = new()
                        {
                            { "UserName", Store.Instance.GetValue("TemplateUserName") }
                        };
                        for (int j = 0; j < reader.FieldCount; j++)
                        {
                            string str = reader.GetValue(j) is object v ? v.ToString() ?? "" : "";
                            if (bHead)
                            {
                                header.Add(str);
                            } else
                            {
                                data[header[j]] = str;
                            }
                        }
                        if (!bHead && !AddItem(template, ref maxnum, data, ref list))
                        {
                            break;
                        }
                        bHead = false;
                    }
                }

                reader.Close();
            }
            catch { }

            if (list.Count <= 0)
            {
                Program.ShowBalloonTip("WebAuto エラー", "データが見つかりませんでした。", ToolTipIcon.Error);
                return;
            }
            DataGridViewRow[] rows = new DataGridViewRow[list.Count];
            int index = 0;
            foreach ((string[] val, DataGridViewRow dataGridViewRow) in from string[] val in list
                                                                        let dataGridViewRow = new DataGridViewRow()
                                                                        select (val, dataGridViewRow))
            {
                dataGridViewRow.CreateCells(MyDataGridViewInput, val);
                rows[index] = dataGridViewRow;
                index += 1;
            }

            MyDataGridViewInput.Rows.Clear();
            MyDataGridViewInput.Rows.AddRange(rows);
        }

        private static bool AddItem(Template template, ref int maxnum, Dictionary<string, string> data, ref List<string[]> list)
        {
            foreach (Item item in template.Item)
            {
                bool m = item.Cond.All(element =>
                    data.TryGetValue(element.Name, out string? value) && value == element.Value
                );
                if(m)
                {
                    foreach (ItemValue element in item.Field)
                    {
                        string value = RegexName().Replace(element.Value, m0 =>
                            data.TryGetValue(m0.Groups[1].Value, out string? value) ? value : m0.Value);
                        list.Add([element.Name, element.Name, value]);
                    }
                    --maxnum;
                    if (maxnum <= 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            MyDataGridViewInput.Rows.Clear();
            MyDataGridViewInput.UndoClear();
            MyDataGridViewInput.RedoClear();
            MyDataGridViewInput.Enabled = true;
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            if (Parent?.FindForm() is FormBrowser owner)
            {
                owner.CancelInputData();
            }
        }

        [GeneratedRegex("#\\{(.*)\\}")]
        private static partial Regex RegexName();
    }
}
