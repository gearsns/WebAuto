﻿using System.Data;
using System.Reflection;
using TinyCsvParser;
using TinyCsvParser.Mapping;
using TinyCsvParser.Ranges;

namespace WebAuto
{
    internal class MyDataGridView : DataGridView
    {
        public delegate void MyDataGridViewCellValueChangedHandler(object sender, MyDataGridViewCellValueChangedEventArgs e);
        public delegate void MyDataGridViewBeginPasteFromClipboardHandler(object sender, MyDataGridViewBeginPasteFromClipboardEventArgs e);
        public event MyDataGridViewCellValueChangedHandler? MyDataGridViewCellValueChanged;
        public event MyDataGridViewBeginPasteFromClipboardHandler? MyDataGridViewBeginPasteFromClipboard;

        private readonly Stack<ICommandPattern> m_undo = new();
        private readonly Stack<ICommandPattern> m_redo = new();
        public void InitEvent() {
            ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            CellBeginEdit += new DataGridViewCellCancelEventHandler(MyDataGridView_CellBeginEdit);
            CellEndEdit += new DataGridViewCellEventHandler(MyDataGridView_CellEndEdit);
            CurrentCellChanged += new EventHandler(DataGridView_CurrentCellChanged);
            UserDeletingRow += new DataGridViewRowCancelEventHandler(MyDataGridView_UserDeletingRow);
            KeyDown += new KeyEventHandler(MyDataGridView_KeyDown);
            MouseDown += new MouseEventHandler(MyDataGridView_MouseDown);

            BorderStyle = BorderStyle.None;
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            EnableHeadersVisualStyles = false;
            RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            int l = Columns.Count;
            for (int i = 0; i < l; ++i)
            {
                Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            typeof(DataGridView).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(this, true, null);

            CellMouseEnter += MyDataGridView_CellMouseEnter;
        }

        private DataGridViewCellEventArgs mouseLocation = new(-1, -1);
        private void MyDataGridView_CellMouseEnter(object? sender, DataGridViewCellEventArgs e)
        {
            mouseLocation = e;
            if(e.ColumnIndex >= 0)
            {
                if(Columns[e.ColumnIndex].ContextMenuStrip is ContextMenuStrip contextMenuStrip)
                {
                    contextMenuStrip.Opened += ContextMenu_Opened;
                }
            }
        }

        private void ContextMenu_Opened(object? sender, EventArgs e)
        {
            if(sender  is ContextMenuStrip contextMenuStrip)
            {
                contextMenuStrip.Opened -= ContextMenu_Opened;
                if(RowCount > mouseLocation.RowIndex)
                {
                    DataGridViewCell cell = this[mouseLocation.ColumnIndex, mouseLocation.RowIndex];
                    if (!cell.Selected)
                    {
                        CurrentCell = cell;
                    }
                }
            }
        }

        public void SetSelectColumnCellsValue(int columnIndex, string value)
        {
            bool bFirst = true;
            foreach (DataGridViewCell c in from DataGridViewCell c in SelectedCells
                                           where c.ColumnIndex == columnIndex
                                           select c)
            {
                if (c.Visible && !c.ReadOnly)
                {
                    if (bFirst)
                    {
                        bFirst = false;
                        SetBeginCommand(c.RowIndex, c.ColumnIndex);
                    }
                    SetCellValue(c.RowIndex, c.ColumnIndex, value);
                }
            }
            if (!bFirst)
            {
                SetEndCommand();
            }
        }

        // DataGridViewの幅が100を超えるとうまくスクロールしないので強制的にスクロールするようにする
        public delegate void DelegateDataGridViewScrollFix(DataGridView dgv);
        private void DataGridView_CurrentCellChanged(object? sender, EventArgs e)
        {
            if (sender is DataGridView dgv && dgv.CurrentCell is DataGridViewCell currentCell)
            {
                if (!currentCell.Displayed || currentCell.ColumnIndex == dgv.ColumnCount - 1)
                {
                    dgv.FirstDisplayedScrollingColumnIndex = currentCell.ColumnIndex;
                }
            }
        }

        private bool bDeletingRow;
        private void MyDataGridView_UserDeletingRow(object? sender, DataGridViewRowCancelEventArgs e)
        {
            RedoClear();
            if (!bDeletingRow)
            {
                if(e.Row != null)
                {
                    SetBeginCommand(e.Row.Index, 0);
                }
                bDeletingRow = true;
            }
            if (e.Row != null)
            {
                m_undo.Push(new DeleteLineCommand(this, e.Row.Index));
            }
            if (SelectedRows.Count == 1)
            {
                SetEndCommand();
                bDeletingRow = false;
            }
        }
        private void MyDataGridView_KeyDown(object? sender, KeyEventArgs e)
        {
            if (sender is not DataGridView dgv)
            {
                return;
            }
            int x = dgv.CurrentCellAddress.X;
            int y = dgv.CurrentCellAddress.Y;
            if ((e.Modifiers & Keys.Control) == Keys.Control && e.KeyCode == Keys.Z)
            {
                if (!dgv.ReadOnly)
                {
                    Undo();
                }
            }
            else if ((e.Modifiers & Keys.Control) == Keys.Control && e.KeyCode == Keys.Y)
            {
                if (!dgv.ReadOnly)
                {
                    Redo();
                }
            }
            else if (e.KeyCode is Keys.Delete or Keys.Back)
            {
                if (dgv.ReadOnly || !dgv.AllowUserToDeleteRows || dgv.CurrentRow.Selected)
                {
                    return;
                }
                try
                {
                    // セルの内容を消去
                    int newrowindex = dgv.NewRowIndex;
                    RedoClear();
                    m_undo.Push(new BeginCommand(dgv, y, x));
                    IEnumerable<(DataGridViewCell c, string? value)> enumerable()
                    {
                        foreach (DataGridViewCell c in dgv.SelectedCells)
                        {
                            if (c.RowIndex != newrowindex && !dgv[c.ColumnIndex, c.RowIndex].ReadOnly)
                            {
                                yield return (c, Convert.ToString(c.Value));
                            }
                        }
                    }

                    foreach ((DataGridViewCell c, string? value) in enumerable())
                    {
                        SetCellValue(c.RowIndex, c.ColumnIndex, "");
                        MyDataGridViewCellValueChanged?.Invoke(this, new MyDataGridViewCellValueChangedEventArgs(e, value, "", c.RowIndex, c.ColumnIndex, m_is_new_row));
                    }

                    m_undo.Push(new EndCommand());
                }
                catch { }
            }
            else if ((e.Modifiers & Keys.Control) == Keys.Control && (e.Modifiers & Keys.Shift) == Keys.Shift && (e.KeyCode == Keys.Oemplus || e.KeyCode == Keys.Add))
            {
                if (dgv.ReadOnly || !dgv.AllowUserToAddRows)
                {
                    return;
                }
                try
                {
                    int index = dgv.NewRowIndex;
                    foreach (DataGridViewRow r in dgv.SelectedRows)
                    {
                        if (index > r.Index)
                        {
                            index = r.Index;
                        }
                    }
                    RedoClear();
                    m_undo.Push(new BeginCommand(dgv, y, x));
                    string[] values = new string[dgv.ColumnCount];
                    int num = Math.Max(dgv.SelectedRows.Count, 1);
                    for (int i = 0; i < num; ++i)
                    {
                        dgv.Rows.Insert(index, values);
                        m_undo.Push(new InsertLineCommand(dgv, index));
                        MyDataGridViewCellValueChanged?.Invoke(this, new MyDataGridViewCellValueChangedEventArgs(e, "", "", index, -1, m_is_new_row));
                    }
                    m_undo.Push(new EndCommand());
                }
                catch { }
            }
            else if ((e.Modifiers & Keys.Control) == Keys.Control && e.KeyCode == Keys.C)
            {
                CopyToClipboard(dgv, e);
            }
            else if ((e.Modifiers & Keys.Control) == Keys.Control && e.KeyCode == Keys.V)
            {
                PasteFromClipboard(dgv, e);
            }
        }

        private static void CopyToClipboard(DataGridView dgv, KeyEventArgs e)
        {
            try
            {
                e.SuppressKeyPress = true;
                int newrowindex = dgv.NewRowIndex;
                List<Point> select_cell = [];
                foreach (DataGridViewCell c in from DataGridViewCell c in dgv.SelectedCells
                                               where c.RowIndex != newrowindex && c.Visible
                                               select c)
                {
                        select_cell.Add(new Point(c.ColumnIndex, c.RowIndex));
                    }
                if (select_cell.Count == 0)
                {
                    return;
                }
                int preRow = -1;
                List<string> data = [];
                List<string> rows = [];
                foreach (Point pos in select_cell.OrderBy(p => p.Y).ThenBy(p => p.X))
                {
                    if (pos.Y != preRow)
                    {
                        preRow = pos.Y;
                        if (rows.Count > 0)
                        {
                            data.Add(string.Join("\t", [.. rows]));
                            rows.Clear();
                        }
                    }
                    string? value = Convert.ToString(dgv[pos.X, pos.Y].Value);
                    switch (value)
                    {
                        case null:
                        rows.Add("\"\"");
                            break;
                        default:
                            if (value.Contains('\n') || value.Contains('"') || value.Contains('\t'))
                    {
                        rows.Add(string.Format("\"{0}\"", value.Replace("\"", "\"\"")));
                    }
                    else if (value.Length == 0)
                    {
                        rows.Add("\"\"");
                    }
                    else
                    {
                        rows.Add(value);
                    }
                            break;
                    }
                }
                if (rows.Count > 0)
                {
                    data.Add(string.Join("\t", [.. rows]));
                }
                string cp = string.Join("\r\n", [.. data]);
                if (string.IsNullOrEmpty(cp))
                {
                    Clipboard.SetText("\r\n", TextDataFormat.Text);
                }
                else
                {
                    Clipboard.SetText(cp, TextDataFormat.Text);
                }
            }
            catch { }
        }
        private void PasteFromClipboard(DataGridView dgv, KeyEventArgs e)
        {
            PasteFromText(dgv, e, Clipboard.GetText());
        }
        private void PasteFromText(DataGridView dgv, EventArgs e, string text)
        {
            if (dgv.ReadOnly)
            {
                return;
            }
            int x = dgv.CurrentCellAddress.X;
            int y = dgv.CurrentCellAddress.Y;
            try
            {
                bool[,] bSelectedCells = new bool[dgv.Columns.Count + 1, dgv.Rows.Count + 1];
                int iLeft = -1, iTop = -1, iRight = -1, iBottom = -1;
                int newrowindex = dgv.NewRowIndex;
                int iSelectNum = 0;
                // 連続している範囲かチェック
                foreach (DataGridViewCell c in
                from DataGridViewCell c in dgv.SelectedCells
                where c.ColumnIndex >= 0 && (c.RowIndex == newrowindex || (c.RowIndex != newrowindex/* && !dgv[c.ColumnIndex, c.RowIndex].ReadOnly*/))
                select c)
                {
                    bSelectedCells[c.ColumnIndex, c.RowIndex] = true;
                    ++iSelectNum;
                    if (iLeft < 0 || iLeft > c.ColumnIndex)
                    {
                        iLeft = c.ColumnIndex;
                    }

                    if (iRight < 0 || iRight < c.ColumnIndex)
                    {
                        iRight = c.ColumnIndex;
                    }

                    if (iTop < 0 || iTop > c.RowIndex)
                    {
                        iTop = c.RowIndex;
                    }

                    if (iBottom < 0 || iBottom < c.RowIndex)
                    {
                        iBottom = c.RowIndex;
                    }
                }
                // 複数の選択範囲が有ったらNGに
                if (iTop < 0)
                {
                    iTop = iBottom = y;
                    iLeft = iRight = x;
                }
                if (iTop < 0 || iBottom < 0 || iLeft < 0 || iRight < 0)
                {
                    _ = MessageBox.Show("エラー");
                    return;
                }
                for (int iRow = iTop; iRow <= iBottom; ++iRow)
                {
                    for (int iCol = iLeft; iCol <= iRight; ++iCol)
                    {
                        if (!bSelectedCells[iCol, iRow])
                        {
                            _ = MessageBox.Show("複数の選択範囲への貼り付けはできません。");
                            return;
                        }
                    }
                }
                // 改行を変換
                text = text.Replace("\r\n", Environment.NewLine);
                text = text.Replace("\r", Environment.NewLine);
                // 改行で分割
                CsvParserOptions csvParserOptions = new(false, '\t');
                CsvReaderOptions csvReaderOptions = new([Environment.NewLine, "\n", "\r\n", "\n"]);
                CsvDataMapping csvMapper = new();
                CsvParser<CsvData> csvParser = new(csvParserOptions, csvMapper);
                List<string[]> lines = [];
                foreach (CsvMappingResult<CsvData>? line in csvParser.ReadFromString(csvReaderOptions, text))
                {
                    if (null == line)
                    {
                        lines.Add([]);
                    }
                    else
                    {
                        lines.Add(line.Result.Values ?? []);
                    }
                }
                int iLineNum = lines.Count;
                if (null != MyDataGridViewBeginPasteFromClipboard)
                {
                    MyDataGridViewBeginPasteFromClipboardEventArgs eEegin = new(e, lines);
                    MyDataGridViewBeginPasteFromClipboard?.Invoke(this, eEegin);
                    if (eEegin.ModifyRequested)
                    {
                        lines = eEegin.Lines;
                    }
                }
                //
                if (iLineNum == 1)
                {
                    // lines １行の場合は、選択範囲すべてにコピーするようにする
                    // タブで分割
                    string[] vals = lines[0];
                    if (iSelectNum == 1 && vals != null)
                    {
                        iRight = dgv.Columns.Count - 1;
                    }
                    RedoClear();
                    m_undo.Push(new BeginCommand(dgv, y, x));
                    for (int r = iTop; r <= iBottom; ++r)
                    {
                        bool b_is_new_row = false;
                        // 行追加モード＆最終行の時は行追加
                        if (r == dgv.RowCount - 1 && dgv.AllowUserToAddRows == true)
                        {
                            dgv.RowCount += 1;
                            m_undo.Push(new NewLineCommand(dgv));
                            b_is_new_row = true;
                        }
                        for (int c = iLeft, ci = 0; c <= iRight; ++c, ++ci)
                        {
                            if (vals == null || ci >= vals.Length)
                            {
                                break;
                            }
                            // 貼り付け
                            DataGridViewCell cell_data = dgv[c, r];
                            if (!cell_data.Visible)
                            {
                                --ci;
                            }
                            else if (!cell_data.ReadOnly)
                            {
                                string? value = Convert.ToString(cell_data.Value);
                                if (vals != null && value != vals[ci])
                                {
                                    m_undo.Push(new PasteCommand(dgv, r, c, value));
                                    dgv[c, r].Value = vals[ci];
                                    MyDataGridViewCellValueChanged?.Invoke(this, new MyDataGridViewCellValueChangedEventArgs(e, value, vals[ci], r, c, m_is_new_row));
                                }
                            }
                        }
                        MyDataGridViewCellValueChanged?.Invoke(this, new MyDataGridViewCellValueChangedEventArgs(e, null, null, r, -1, b_is_new_row));
                    }
                    m_undo.Push(new EndCommand());
                }
                else
                {
                    RedoClear();
                    m_undo.Push(new BeginCommand(dgv, y, x));
                    if (iSelectNum == 1)
                    {
                        iRight = dgv.Columns.Count - 1;
                        iBottom = iTop + iLineNum - 1;
                    }
                    else
                    {
                        if (iBottom >= dgv.RowCount - 1 && dgv.AllowUserToAddRows == true)
                    {
                        iBottom = iTop + iLineNum - 1;
                    }
                    }
                    for (int r = iTop, ri = 0; r <= iBottom; ++r, ++ri)
                    {
                        // タブで分割
                        if (lines[ri] is not string[] vals)
                        {
                            continue;
                        }
                        int iRightMax = iRight;
                        bool b_is_new_row = false;
                        // 行追加モード＆最終行の時は行追加
                        if (r == dgv.RowCount - 1 && dgv.AllowUserToAddRows == true)
                        {
                            // ×ブランクのみデータで行の追加は行わない
                            if (!vals.Any(v => v.Length > 0))
                            {
                                continue;
                            }
                            dgv.RowCount += 1;
                            m_undo.Push(new NewLineCommand(dgv));
                            b_is_new_row = true;
                        }
                        for (int c = iLeft, ci = 0; c <= iRightMax; ++c, ++ci)
                        {
                            if (ci >= vals.Length)
                            {
                                break;
                            }
                            // 貼り付け
                            DataGridViewCell cell_data = dgv[c, r];
                            if (!cell_data.Visible)
                            {
                                --ci;
                            }
                            else if (!cell_data.ReadOnly)
                            {
                                string? value = Convert.ToString(cell_data.Value);
                                if (value != vals[ci])
                                {
                                    m_undo.Push(new PasteCommand(dgv, r, c, value));
                                    dgv[c, r].Value = vals[ci];
                                    MyDataGridViewCellValueChanged?.Invoke(this, new MyDataGridViewCellValueChangedEventArgs(e, value, vals[ci], r, c, m_is_new_row));
                                }
                            }
                        }
                        MyDataGridViewCellValueChanged?.Invoke(this, new MyDataGridViewCellValueChangedEventArgs(e, null, null, r, -1, b_is_new_row));
                    }
                    m_undo.Push(new EndCommand());
                }
            }
            catch { }
        }

        private partial class CsvData
        {
            public string[]? Values { get; set; }
        };

        private partial class CsvDataMapping : CsvMapping<CsvData>
        {
            public CsvDataMapping()
                : base()
            {
                _ = MapProperty(new RangeDefinition(0, 100), x => x.Values);
            }
        }

        public void SetCellValue(int row, int col, object value)
        {
            RedoClear();
            // 行追加モード＆最終行の時は行追加
            if (row == RowCount - 1 && AllowUserToAddRows == true)
            {
                RowCount += 1;
                m_undo.Push(new NewLineCommand(this));
            }
            m_undo.Push(new PasteCommand(this, row, col, this[col, row].Value));
            this[col, row].Value = value;
        }
        public void SetEventCellValue(int row, int col, object value)
        {
            RedoClear();
            // 行追加モード＆最終行の時は行追加
            if (row == RowCount - 1 && AllowUserToAddRows == true)
            {
                RowCount += 1;
                m_undo.Push(new NewLineCommand(this));
            }
            m_undo.Push(new PasteCommand(this, row, col, this[col, row].Value));
            var from_value = this[col, row].Value;
            this[col, row].Value = value;
            MyDataGridViewCellValueChanged?.Invoke(this, new MyDataGridViewCellValueChangedEventArgs(null, from_value, value, row, col, false));
        }
        public void SortRow(System.Collections.IComparer comp)
        {
            if (RowCount > 0)
            {
                RedoClear();
                m_undo.Push(new SortCommand(this, comp));
            }
        }
        public void DeleteRow(int row)
        {
            RedoClear();
            m_undo.Push(new DeleteLineCommand(this, row));
            Rows.RemoveAt(row);
        }
        public void SetBeginCommand(int row, int col)
        {
            RedoClear();
            m_undo.Push(new BeginCommand(this, row, col));
        }
        public void SetEndCommand()
        {
            RedoClear();
            m_undo.Push(new EndCommand());
        }
        public void Undo(int cnt = 0)
        {
            try
            {
                ICommandPattern command = m_undo.Pop();
                if (command is EndCommand)
                {
                    cnt += 1;
                }
                else if (command is BeginCommand)
                {
                    cnt -= 1;
                }
                command.UnDo();
                m_redo.Push(command);
                if (cnt > 0)
                {
                    Undo(cnt);
                }
            }
            catch { }
            if (cnt <= 0)
            {
                MyDataGridViewCellValueChanged?.Invoke(this, new MyDataGridViewCellValueChangedEventArgs(null, null, null, -1, -1, m_is_new_row));
            }
        }
        public void Redo(int cnt = 0)
        {
            try
            {
                ICommandPattern command = m_redo.Pop();
                if (command is BeginCommand)
                {
                    cnt += 1;
                }
                else if (command is EndCommand)
                {
                    cnt -= 1;
                }
                command.ReDo();
                m_undo.Push(command);
                if (cnt > 0)
                {
                    Redo(cnt);
                }
            }
            catch { }
            if (cnt <= 0)
            {
                MyDataGridViewCellValueChanged?.Invoke(this, new MyDataGridViewCellValueChangedEventArgs(null, null, null, -1, -1, m_is_new_row));
            }
        }

        private int m_iCurrentPattern = 0;
        public void SetDirty()
        {
            m_iCurrentPattern = -1;
        }
        public void SetCurrentUndo()
        {
            m_iCurrentPattern = m_undo.Count;
        }
        public bool IsModify => m_iCurrentPattern != m_undo.Count;
        public void UndoClear() { m_iCurrentPattern = 0; m_undo.Clear(); }
        public void RedoClear()
        {
            m_redo.Clear();
            if (m_iCurrentPattern > m_undo.Count)
            {
                m_iCurrentPattern = -1;
            }
        }

        private object? m_pre_cell_value = null;
        private bool m_is_new_row = false;
        private void MyDataGridView_CellBeginEdit(object? sender, DataGridViewCellCancelEventArgs e)
        {
            if (sender is DataGridView dgv)
            {
                m_pre_cell_value = dgv[e.ColumnIndex, e.RowIndex].Value;
                m_is_new_row = dgv.Rows[e.RowIndex].IsNewRow;
            }
        }

        private void MyDataGridView_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            if (sender is not DataGridView dgv)
            {
                return;
            }
            if (dgv[e.ColumnIndex, e.RowIndex].Value != m_pre_cell_value)
            {
                RedoClear();
                m_undo.Push(new BeginCommand(dgv, e.RowIndex, e.ColumnIndex));
                if (m_is_new_row)
                {
                    m_undo.Push(new NewLineCommand(dgv));
                }
                string str = dgv[e.ColumnIndex, e.RowIndex].Value?.ToString() ?? "";
                if (str.Contains('\t'))
                {
                    PasteFromText(dgv, e, str);
                }
                else
                {
                    m_undo.Push(new PasteCommand(dgv, e.RowIndex, e.ColumnIndex, m_pre_cell_value));
                    MyDataGridViewCellValueChanged?.Invoke(this, new MyDataGridViewCellValueChangedEventArgs(e, m_pre_cell_value, dgv[e.ColumnIndex, e.RowIndex].Value, e.RowIndex, e.ColumnIndex, m_is_new_row));
                }
                m_undo.Push(new EndCommand());
            }
        }

        private void MyDataGridView_MouseDown(object? sender, MouseEventArgs e)
        {
            if (sender is not DataGridView dgv)
            {
                return;
            }
            HitTestInfo hti = dgv.HitTest(e.X, e.Y);
            if (hti.ColumnIndex == -1 && hti.RowIndex == -1)
            {
                if (dgv.SelectionMode != DataGridViewSelectionMode.RowHeaderSelect)
                {
                    dgv.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
                }
            }
            else if (hti.ColumnIndex == -1 && hti.RowIndex >= 0)
            {
                // row header click
                if (dgv.SelectionMode != DataGridViewSelectionMode.RowHeaderSelect)
                {
                    dgv.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
                }
            }
            else if (hti.RowIndex == -1 && hti.ColumnIndex >= 0)
            {
                // column header click
                if (Columns.Count > 0 && Columns[0].SortMode != DataGridViewColumnSortMode.NotSortable)
                {
                    return;
                }
                if (dgv.SelectionMode != DataGridViewSelectionMode.ColumnHeaderSelect)
                {
                    dgv.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
                }
            }
        }
    }
    internal interface ICommandPattern
    {
        void UnDo();
        void ReDo();
    }
    public class BeginCommand(DataGridView d, int r, int c) : ICommandPattern
    {
        public void UnDo()
        {
            if (c >= 0 && r >= 0)
            {
                d.CurrentCell = d[c, r];
            }
        }
        public void ReDo()
        {
            if (c >= 0 && r >= 0)
            {
                d.CurrentCell = d[c, r];
            }
        }
    }
    public class EndCommand : ICommandPattern
    {
        public EndCommand() { }
        public void UnDo() { }
        public void ReDo() { }
    }
    public class RowOriginComparer(Dictionary<DataGridViewRow, int> l) : System.Collections.IComparer
    {
        public int Compare(object? x, object? y)
        {
            int i1 = -1;
            if (x is DataGridViewRow DataGridViewRow1 && l.TryGetValue(DataGridViewRow1, out int value1))
            {
                i1 = value1;
            }
            int i2 = -1;
            if (y is DataGridViewRow DataGridViewRow2 && l.TryGetValue(DataGridViewRow2, out int value2))
            {
                i2 = value2;
            }
            return i1 - i2;
        }
    }
    public class SortCommand : ICommandPattern
    {
        private readonly DataGridView dataGridView;
        private readonly Dictionary<DataGridViewRow, int> row_list = [];
        private System.Collections.IComparer RowComparer;
        public SortCommand(DataGridView d, System.Collections.IComparer comp)
        {
            dataGridView = d;
            RowComparer = comp;
            Sort();
        }

        public void Sort()
        {
            foreach (DataGridViewRow row in dataGridView.Rows.Cast<DataGridViewRow>())
            {
                if (row.IsNewRow)
                {
                    continue;
                }
                row_list[row] = row.Index;
            }
            dataGridView.Sort(RowComparer);
        }
        public void UnDo()
        {
            dataGridView.Sort(new RowOriginComparer(row_list));
        }
        public void ReDo()
        {
            Sort();
        }
    }

    public class PasteCommand(DataGridView d, int r, int c, object? v) : ICommandPattern
    {
        private object? val = v;
        private Color color = d[c, r].Style.BackColor;

        public void UnDo()
        {
            object v = d[c, r].Value;
            d[c, r].Value = val;
            Color now_color = d[c, r].Style.BackColor;
            d[c, r].Style.BackColor = color;
            val = v;
            color = now_color;
        }
        public void ReDo()
        {
            object v = d[c, r].Value;
            d[c, r].Value = val;
            Color now_color = d[c, r].Style.BackColor;
            d[c, r].Style.BackColor = color;
            val = v;
            color = now_color;
        }
    }
    public class DeleteLineCommand : ICommandPattern
    {
        private readonly int row;
        private readonly DataGridViewRow clonedRow;
        private readonly DataGridView dataGridView;
        public DeleteLineCommand(DataGridView d, int r)
        {
            dataGridView = d;
            row = r;
            DataGridViewRow dgv_row = d.Rows[r];
            clonedRow = (DataGridViewRow)dgv_row.Clone();
            for (int index = 0; index < dgv_row.Cells.Count; index++)
            {
                clonedRow.Cells[index].Value = dgv_row.Cells[index].Value;
            }
        }
        public void UnDo()
        {
            dataGridView.Rows.Insert(row, clonedRow);
        }
        public void ReDo()
        {
            dataGridView.Rows.RemoveAt(row);
        }
    }
    public class InsertLineCommand : ICommandPattern
    {
        private readonly int row;
        private readonly DataGridViewRow clonedRow;
        private readonly DataGridView dataGridView;
        public InsertLineCommand(DataGridView d, int r)
        {
            dataGridView = d;
            row = r;
            DataGridViewRow dgv_row = d.Rows[r];
            clonedRow = (DataGridViewRow)dgv_row.Clone();
            for (int index = 0; index < dgv_row.Cells.Count; index++)
            {
                clonedRow.Cells[index].Value = dgv_row.Cells[index].Value;
            }
        }
        public void UnDo()
        {
            dataGridView.Rows.RemoveAt(row);
        }
        public void ReDo()
        {
            dataGridView.Rows.Insert(row, clonedRow);
        }
    }
    public class NewLineCommand(DataGridView d) : ICommandPattern
    {
        public void UnDo()
        {
            d.RowCount -= 1;
        }
        public void ReDo()
        {
            d.RowCount += 1;
        }
    }
    public class MyDataGridViewCellValueChangedEventArgs(EventArgs? ea, object? f, object? t, int r, int c, bool n) : EventArgs
    {
        public EventArgs? FromEventArgs => ea;
        public object? FromValue { get; } = f;

        public object? ToValue { get; } = t;

        public int ColumnIndex { get; } = c;

        public int RowIndex { get; } = r;

        public bool IsNewRow { get; } = n;
    }
    public class MyDataGridViewBeginPasteFromClipboardEventArgs(EventArgs? ea, List<string[]> f) : EventArgs
    {
        public EventArgs? FromEventArgs => ea;
        public List<string[]> Lines { get; set; } = f;
        public bool ModifyRequested { get; set; }
    }
}
