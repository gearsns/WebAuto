namespace WebAuto
{
    public partial class FormMonthCalendar : Form
    {
        public FormMonthCalendar()
        {
            InitializeComponent();
        }

        private void FormMonthCalendar_Load(object sender, EventArgs e)
        {
            ClientSize = monthCalendar.Size;
            Left = Cursor.Position.X - ClientSize.Width / 2;
            Top = Cursor.Position.Y - ClientSize.Height / 2;
            Rectangle rect = Screen.FromControl(this).WorkingArea;
            if(rect.Bottom < Bottom)
            {
                Top = rect.Bottom - Height;
            }
        }

        public DateTime SelectedDateTime => monthCalendar.SelectionStart;

        private void MonthCalendar_DateSelected(object sender, DateRangeEventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void MonthCalendar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}
