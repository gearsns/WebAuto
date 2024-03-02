using System.Data;

namespace WebAuto
{
    public partial class FormExcel : Form
    {
        private readonly ExcelData ExcelDataWork;
        public FormExcel()
        {
            InitializeComponent();
            MyDataGridViewInput.InitEvent();
            ExcelDataWork = new(ref buttonExcel, ref comboBoxSheet);
        }
        private void ButtonInput_Click(object sender, EventArgs e)
        {
            if (Parent!.FindForm() is FormBrowser owner)
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

        private void ButtonExcel_Click(object sender, EventArgs e)
        {
            _ = ExcelDataWork.Load();
            SetSheetData();
        }

        private void FormExcel_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void FormExcel_DragDrop(object sender, DragEventArgs e)
        {
            _ = ExcelDataWork.Load(e);
            SetSheetData();
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            MyDataGridViewInput.Rows.Clear();
            MyDataGridViewInput.UndoClear();
            MyDataGridViewInput.RedoClear();
            MyDataGridViewInput.Enabled = true;
        }
        private void SetSheetData()
        {
            if (comboBoxSheet?.SelectedIndex >= 0)
            {
                string sheetname = comboBoxSheet.Items[comboBoxSheet.SelectedIndex]?.ToString() ?? "";
                SetSheetData(sheetname);
            }
        }

        private void SetSheetData(string sheetname)
        {
            List<List<string>> list = ExcelDataWork.Get(sheetname);
            if (list == null || list.Count <= 1)
            {
                return;
            }
            MyDataGridViewInput.Rows.Clear();
            MyDataGridViewInput.UndoClear();
            MyDataGridViewInput.RedoClear();
            MyDataGridViewInput.Enabled = true;

            bool bHeader = true;
            List<DataGridViewRow> rows = [];
            foreach ((List<string> val, DataGridViewRow dataGridViewRow)
                in from List<string> val in list
                   let dataGridViewRow = new DataGridViewRow()
                   select (val, dataGridViewRow))
            {
                if (bHeader)
                {
                    bHeader = false;
                    continue;
                }
                string[] data = new string[3];
                if (val.Count <= 1)
                {
                    continue;
                }
                else if (val.Count == 2)
                {
                    data[1] = val[0];
                    data[2] = val[1];
                }
                else
                {
                    data[0] = val[0];
                    data[1] = val[1];
                    data[2] = val[2];
                }
                if (data[1].Length == 0)
                {
                    continue;
                }
                dataGridViewRow.CreateCells(MyDataGridViewInput, data);
                rows.Add(dataGridViewRow);
            }

            MyDataGridViewInput.Rows.Clear();
            MyDataGridViewInput.Rows.AddRange(rows.ToArray());
        }
        private void ComboBoxSheet_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SetSheetData();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            if (Parent?.FindForm() is FormBrowser owner)
            {
                owner.CancelInputData();
            }
        }
    }
}
