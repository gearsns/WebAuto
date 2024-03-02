using System.Data;

namespace WebAuto
{
    public partial class DataGridViewMultiComboBoxCell : DataGridViewTextBoxCell
    {
        public override Type EditType => typeof(DataGridViewMultiComboBoxEditingControl);
        public DataGridViewMultiComboBoxCell()
            : base()
        {
        }
        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            if(DataGridView == null)
            {
                return;
            }
            if (DataGridView.EditingControl is DataGridViewMultiComboBoxEditingControl textBox)
            {
                if (OwningColumn is DataGridViewMultiComboBoxColumn column)
                {
                    textBox.SetDataSource(column.DataSource, column.DropDownColWidth, column.DropDownWidth);
                    int selectedIndex = textBox.FindStringExact(Convert.ToString(initialFormattedValue));
                    DataView dv = column.DataSource;
                    textBox.SelectedIndex = selectedIndex >= 0 && Convert.ToString(dv[selectedIndex][textBox.DisplayMember]) != Convert.ToString(initialFormattedValue)
                        ? -1
                        : selectedIndex;
                }
                textBox.Text = Convert.ToString(initialFormattedValue);
            }
        }
    }
    /// <summary>
    /// DataGridViewMaskedTextBoxCellでホストされる
    /// MaskedTextBoxコントロールを表します。
    /// </summary>
    public class DataGridViewMultiComboBoxEditingControl :
        ComboBox, IDataGridViewEditingControl
    {
        private int m_ColumnWidth = 0;

        //コンストラクタ
        public DataGridViewMultiComboBoxEditingControl()
        {
            TabStop = false;
            FlatStyle = FlatStyle.Flat;
            Margin = new Padding(0);
            DropDownStyle = ComboBoxStyle.DropDown;
        }
        public void SetDataSource(DataView source, int col, int width)
        {
            if (source != null && source.Table != null && source.Table.Columns.Count >= 2)
            {
                DataSource = source;
                DisplayMember = source.Table.Columns[0].ColumnName;
                ValueMember = source.Table.Columns[1].ColumnName;
                DrawMode = DrawMode.OwnerDrawFixed;
                DrawItem += ComboDrawItem;
                if (width <= 0)
                {
                    m_ColumnWidth = 10;
                    DropDownWidth = 10;
                    int dropdownwidth = 0;
                    Graphics g = CreateGraphics();
                    foreach (DataRow r in source.Table.Rows)
                    {
                        SizeF sf = g.MeasureString(r[DisplayMember].ToString(), Font);
                        if (m_ColumnWidth < (int)sf.Width)
                        {
                            m_ColumnWidth = (int)sf.Width;
                        }
                        sf = g.MeasureString(r[ValueMember].ToString(), Font);
                        if (dropdownwidth < (int)sf.Width)
                        {
                            dropdownwidth = (int)sf.Width;
                        }
                    }
                    m_ColumnWidth += 5;
                    DropDownWidth = dropdownwidth + m_ColumnWidth + 5 + SystemInformation.HorizontalScrollBarHeight;
                    g.Dispose();
                }
                else
                {
                    m_ColumnWidth = col;
                    DropDownWidth = width;
                }
            }
        }

        private void ComboDrawItem(object? sender, DrawItemEventArgs e)
        {
            if (sender is not ComboBox cb)
            {
                return;
            }
            using Pen p = new(Color.Gray);
            Brush b = new SolidBrush(e.ForeColor);

            if (cb.Items[e.Index] is not DataRowView drv) { return; }
            e.DrawBackground();
            if (e.Font != null)
            {
                e.Graphics.DrawString(Convert.ToString(drv.Row[DisplayMember]), e.Font, b, e.Bounds.X, e.Bounds.Y);
            }
            if (m_ColumnWidth < 0)
            {
                m_ColumnWidth = DropDownWidth;
            }
            float bLineX = m_ColumnWidth;// sf.Width;
            e.Graphics.DrawLine(p, bLineX, e.Bounds.Top, bLineX, e.Bounds.Bottom);

            if (e.Font != null)
            {
                e.Graphics.DrawString(Convert.ToString(drv.Row[ValueMember]), e.Font, b, bLineX, e.Bounds.Y);
            }

            if (Convert.ToBoolean(e.State & DrawItemState.Selected))
            {
                ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds);
            }
        }

        #region IDataGridViewEditingControl メンバ

        //編集コントロールで変更されたセルの値
        public object GetEditingControlFormattedValue(
            DataGridViewDataErrorContexts context) {
            return Text;
        }

        //編集コントロールで変更されたセルの値
        public object EditingControlFormattedValue
        {
            get => GetEditingControlFormattedValue(
                    DataGridViewDataErrorContexts.Formatting);
            set => Text = (string)value;
        }

        //セルスタイルを編集コントロールに適用する
        //編集コントロールの前景色、背景色、フォントなどをセルスタイルに合わせる
        public void ApplyCellStyleToEditingControl(
            DataGridViewCellStyle dataGridViewCellStyle) {
            Font = dataGridViewCellStyle.Font;
        }

        //編集するセルがあるDataGridView
        public DataGridView? EditingControlDataGridView { get; set; } = null!;

        //編集している行のインデックス
        public int EditingControlRowIndex { get; set; } = -1;

        //値が変更されたかどうか
        //編集コントロールの値とセルの値が違うかどうか
        public bool EditingControlValueChanged { get; set; }

        //指定されたキーをDataGridViewが処理するか、編集コントロールが処理するか
        //Trueを返すと、編集コントロールが処理する
        //dataGridViewWantsInputKeyがTrueの時は、DataGridViewが処理できる
        public bool EditingControlWantsInputKey(
            Keys keyData, bool dataGridViewWantsInputKey)
        {
            //Keys.Left、Right、Home、Endの時は、Trueを返す
            //このようにしないと、これらのキーで別のセルにフォーカスが移ってしまう
            return (keyData & Keys.KeyCode) switch
            {
                Keys.Right or Keys.End or Keys.Left or Keys.Home or Keys.Up or Keys.Down or Keys.PageUp or Keys.PageDown => true,
                _ => !dataGridViewWantsInputKey,
            };
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e) {
            base.OnPreviewKeyDown(e);
            switch (e.KeyData & Keys.KeyCode)
            {
                case Keys.Tab:
                case Keys.Enter:
                    EditingControlValueChanged = true;
                    EditingControlDataGridView?.NotifyCurrentCellDirty(true);
                    break;
                default:
                    break;
            }
        }

        //マウスカーソルがEditingPanel上にあるときのカーソルを指定する
        //EditingPanelは編集コントロールをホストするパネルで、
        //編集コントロールがセルより小さいとコントロール以外の部分がパネルとなる
        public Cursor EditingPanelCursor => base.Cursor;

        //コントロールで編集する準備をする
        //テキストを選択状態にしたり、挿入ポインタを末尾にしたりする
        public void PrepareEditingControlForEdit(bool selectAll) {
            if (selectAll) {
                //選択状態にする
                SelectAll();
            } else {
                //挿入ポインタを末尾にする
                SelectionStart = Text.Length;
            }
        }

        //値が変更した時に、セルの位置を変更するかどうか
        //値が変更された時に編集コントロールの大きさが変更される時はTrue
        public bool RepositionEditingControlOnValueChange => false;

        #endregion

        //値が変更された時
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            //値が変更されたことをDataGridViewに通知する
            EditingControlValueChanged = true;
            EditingControlDataGridView?.NotifyCurrentCellDirty(true);
        }
        //
        // 概要:
        //     System.Windows.Forms.ComboBox.TextUpdate イベントを発生させます。
        //
        // パラメーター:
        //   e:
        //     イベント データを格納している System.EventArgs。
        protected override void OnTextUpdate(EventArgs e)
        {
            base.OnTextUpdate(e);
            //値が変更されたことをDataGridViewに通知する
            EditingControlValueChanged = true;
            EditingControlDataGridView?.NotifyCurrentCellDirty(true);
        }
        //
        // 概要:
        //     System.Windows.Forms.ComboBox.SelectionChangeCommitted イベントを発生させます。
        //
        // パラメーター:
        //   e:
        //     イベント データを格納している System.EventArgs。
        protected override void OnSelectionChangeCommitted(EventArgs e)
        {
            base.OnSelectionChangeCommitted(e);
            //値が変更されたことをDataGridViewに通知する
            EditingControlValueChanged = true;
            EditingControlDataGridView?.NotifyCurrentCellDirty(true);
        }
    }
}
