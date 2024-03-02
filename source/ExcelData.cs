using System.Text;
using ExcelDataReader;

namespace WebAuto
{
    internal class ExcelData
    {
        private readonly Button targetButton;
        private readonly ComboBox targetComboBox;
        private readonly Dictionary<string, List<List<string>>> Data = [];
        public ExcelData(ref Button button, ref ComboBox comboBox)
        {
            targetButton = button;
            targetComboBox = comboBox;
        }
        public List<List<string>> Get(string sheetName)
        {
            return Data.TryGetValue(sheetName, out List<List<string>>? value) ? value : [];
        }

        public bool Load(string filename)
        {
            targetButton.Enabled = false;
            targetComboBox.Items.Clear();
            Data.Clear();
            //var sheetname = "Sheet1";
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            try
            {
                using FileStream stream = new(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                IExcelDataReader reader;

                //ファイルの拡張子を確認
                if (filename.EndsWith(".xls") || filename.EndsWith(".xlsx") || filename.EndsWith(".xlsb") || filename.EndsWith(".xlsm"))
                {
                    reader = ExcelReaderFactory.CreateReader(stream, new ExcelReaderConfiguration()
                    {
                        //デフォルトのエンコードは西ヨーロッパ言語の為、日本語が文字化けする
                        //オプション設定でエンコードをシフトJISに変更する
                        FallbackEncoding = Encoding.GetEncoding("Shift_JIS")
                    });
                }
                else if (filename.EndsWith(".csv"))
                {
                    reader = ExcelReaderFactory.CreateCsvReader(stream, new ExcelReaderConfiguration()
                    {
                        //デフォルトのエンコードは西ヨーロッパ言語の為、日本語が文字化けする
                        //オプション設定でエンコードをシフトJISに変更する
                        FallbackEncoding = Encoding.GetEncoding("Shift_JIS")
                    });
                }
                else
                {
                    _ = MessageBox.Show("サポート対象外の拡張子です。", "エラー");
                    targetButton.Enabled = true;
                    return false;
                }

                for (int i = 0; i < reader.ResultsCount; i++)
                {
                    string sheetName = reader.Name;
                    List<List<string>> lines = [];
                    while (reader.Read())
                    {
                        List<string> line = [];
                        for (int j = 0; j < reader.FieldCount; j++)
                        {
                            line.Add(reader.GetValue(j) is object v ? v.ToString() ?? "" : "");
                        }
                        lines.Add(line);
                    }
                    if (lines.Count > 0)
                    {
                        _ = targetComboBox.Items.Add(sheetName);
                        Data[sheetName] = lines;
                    }
                    reader.NextResult();
                }

                reader.Close();
            }
            catch { }
            targetButton.Enabled = true;
            if (Data.Count <= 0)
            {
                Program.ShowBalloonTip("WebAuto", "データを取得できませんでした", ToolTipIcon.Warning);
                return false;
            }
            targetComboBox.SelectedIndex = 0;
            return true;
        }
        public bool Load()
        {
            using OpenFileDialog openFileDialog = new()
            {
                Filter = "EXCELファイル(*.xlsx)|*.xlsx"
            };
            return openFileDialog.ShowDialog() == DialogResult.OK
                && Load(openFileDialog.FileName);
        }
        public bool Load(DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetData(DataFormats.FileDrop, false) is string[] files)
            {
                foreach (string v in files)
                {
                    if (Load(v))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
