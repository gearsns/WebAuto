using System.Data;

namespace WebAuto
{
    public partial class DataGridViewMultiComboBoxColumn : DataGridViewColumn
    {
        public DataGridViewMultiComboBoxColumn() : base(new DataGridViewMultiComboBoxCell()) { }
        public DataGridViewMultiComboBoxColumn(DataView comboSource) : base(new DataGridViewMultiComboBoxCell())
        {
            DataSource = comboSource;
        }
        public DataView DataSource { get; set; } = null!;
        public int DropDownWidth { get; set; } = 0;
        public int DropDownColWidth { get; set; } = 0;
        public override object Clone()
        {
            DataGridViewMultiComboBoxColumn col =
                (DataGridViewMultiComboBoxColumn)base.Clone();
            col.DataSource = DataSource;
            col.DropDownWidth = DropDownWidth;
            col.DropDownColWidth = DropDownColWidth;
            return col;
        }
        public override DataGridViewCell CellTemplate
        {
            get => base.CellTemplate;
            set
            {
                if (value is not DataGridViewMultiComboBoxCell)
                {
                    throw new InvalidCastException(
                        "DataGridViewMultiComboBoxCellオブジェクトを" +
                        "指定してください。");
                }
                base.CellTemplate = value;
            }
        }
    }

}
