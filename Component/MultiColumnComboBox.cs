using System.Data;

namespace WebAuto
{
    public partial class MultiColumnComboBox : ComboBox
    {
        int m_ColumnWidth = 0;
        public MultiColumnComboBox()
        {
            InitializeComponent();
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
        private void ComboDrawItem(object ?sender, DrawItemEventArgs e)
        {
            if (DataSource == null)
            {
                return;
            }
            using (DataView dv = (DataView)DataSource)
            {
                if (dv.Table == null)
                {
                    return;
                }
                DataTable dt = dv.Table;
                Brush b = new SolidBrush(e.ForeColor);

                e.DrawBackground();
                if (e.Font != null)
                {
                    e.Graphics.DrawString(Convert.ToString(dt.Rows[e.Index][DisplayMember]), e.Font, b, e.Bounds.X, e.Bounds.Y);
                }

                if (m_ColumnWidth < 0)
                {
                    m_ColumnWidth = DropDownWidth;
                }
                float bLineX = m_ColumnWidth;// sf.Width;
                Pen p = new(Color.Gray);
                e.Graphics.DrawLine(p, bLineX, e.Bounds.Top, bLineX, e.Bounds.Bottom);

                if (e.Font != null)
                {
                    e.Graphics.DrawString(Convert.ToString(dt.Rows[e.Index][ValueMember]), e.Font, b, bLineX, e.Bounds.Y);
                }
            }

            if (Convert.ToBoolean(e.State & DrawItemState.Selected))
            {
                ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds);
            }
        }
    }
}
