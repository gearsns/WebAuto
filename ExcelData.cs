using System.Text;
using ExcelDataReader;

namespace WebAuto
{
    internal class ExcelData
    {
        private readonly Button targetButton;
        private readonly ComboBox targetComboBox;
        private readonly Dictionary<string, List<List<string>>> Data = new();
        public ExcelData(ref Button button, ref ComboBox comboBox)
        {
            targetButton = button;
            targetComboBox = comboBox;
        }
        public List<List<string>> Get(string sheetName)
        {
            return Data.ContainsKey(sheetName) ? Data[sheetName] : new List<List<string>>();
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
                using FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
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
                    List<List<string>> lines = new();
                    while (reader.Read())
                    {
                        List<string> line = new();
                        for (int j = 0; j < reader.FieldCount; j++)
                        {
                            if (reader.GetValue(j) is object v)
                            {
                                line.Add(v.ToString() ?? "");
                            } else
                            {
                                line.Add("");
                            }
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
                for (int i = 0; i < files.Length; i++)
                {
                    if (Load(files[i]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
