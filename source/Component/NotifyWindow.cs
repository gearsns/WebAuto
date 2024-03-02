namespace WebAuto.Resources
{
    public partial class NotifyWindow : Form
    {
        public enum EnmAction
        {
            wait,
            start,
            close
        }

        public enum EnmType
        {
            Success,
            Warning,
            Error,
            Info
        }
        private EnmAction action;
        private int x, y;

        public NotifyWindow()
        {
            InitializeComponent();
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            timer1.Interval = 1;
            action = EnmAction.close;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            switch (action)
            {
                case EnmAction.wait:
                    timer1.Interval = 5000;
                    action = EnmAction.close;
                    break;
                case EnmAction.start:
                    timer1.Interval = 1;
                    Opacity += 0.1;
                    if (x < Location.X)
                    {
                        Left--;
                    }
                    else if (Opacity == 1.0)
                    {
                        action = EnmAction.wait;
                    }
                    break;
                case EnmAction.close:
                    timer1.Interval = 1;
                    Opacity -= 0.1;

                    Left -= 3;
                    if (base.Opacity == 0.0)
                    {
                        base.Close();
                    }
                    break;
                default:
                    break;
            }
        }

        public void ShowAlert(Form form, string title, string msg, EnmType type)
        {
            Opacity = 0.0;
            StartPosition = FormStartPosition.Manual;
            Screen TargetScreen = Screen.FromControl(form);
            for (int i = 1; i < 10; i++)
            {
                string fname = $"alert{i}";
                if (Application.OpenForms[fname] is null)
                {
                    Name = fname;
                    x = TargetScreen.WorkingArea.Right - Width + 15;
                    y = TargetScreen.WorkingArea.Bottom - Height * i - 5 * i;
                    Location = new Point(x, y);
                    break;

                }

            }
            x = TargetScreen.WorkingArea.Right - base.Width - 5;

            switch (type)
            {
                case EnmType.Success:
                    pictureBox2.Image = Properties.Resources.NotifyWindowSuccess;
                    BackColor = Color.FromArgb(0x00, 0x6e, 0x99);//Color.SeaGreen;
                    break;
                case EnmType.Error:
                    pictureBox2.Image = Properties.Resources.NotifyWindowError;
                    BackColor = Color.FromArgb(0x66, 0x00, 0x00);//Color.DarkRed;
                    break;
                case EnmType.Info:
                    pictureBox2.Image = Properties.Resources.NotifyWindowInfo;
                    BackColor = Color.FromArgb(29, 54, 92);// Color.RoyalBlue;
                    break;
                case EnmType.Warning:
                    pictureBox2.Image = Properties.Resources.NotifyWindowWarning;
                    BackColor = Color.FromArgb(0x99, 0x54, 0x00);// Color.DarkOrange;
                    break;
            }

            lblTitle.Text = title;
            lblMsg.Text = msg;

            Show();
            action = EnmAction.start;
            timer1.Interval = 1;
            timer1.Start();
        }
    }
}
