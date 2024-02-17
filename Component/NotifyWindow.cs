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

        private void NotifyWindow_Load(object sender, EventArgs e)
        {
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            switch (this.action)
            {
                case EnmAction.wait:
                    timer1.Interval = 5000;
                    action = EnmAction.close;
                    break;
                case NotifyWindow.EnmAction.start:
                    this.timer1.Interval = 1;
                    this.Opacity += 0.1;
                    if (this.x < this.Location.X)
                    {
                        this.Left--;
                    }
                    else
                    {
                        if (this.Opacity == 1.0)
                        {
                            action = NotifyWindow.EnmAction.wait;
                        }
                    }
                    break;
                case EnmAction.close:
                    timer1.Interval = 1;
                    this.Opacity -= 0.1;

                    this.Left -= 3;
                    if (base.Opacity == 0.0)
                    {
                        base.Close();
                    }
                    break;
            }
        }

        public void ShowAlert(Form form, string title, string msg, EnmType type)
        {
            this.Opacity = 0.0;
            this.StartPosition = FormStartPosition.Manual;
            string fname;

            //var TargetScreen = Screen.PrimaryScreen;
            Screen TargetScreen = Screen.FromControl(form);
            for (int i = 1; i < 10; i++)
            {
                fname = "alert" + i.ToString();
                NotifyWindow frm = (NotifyWindow)Application.OpenForms[fname];

                if (frm == null)
                {
                    this.Name = fname;
                    this.x = TargetScreen.WorkingArea.Right - this.Width + 15;
                    this.y = TargetScreen.WorkingArea.Bottom - this.Height * i - 5 * i;
                    this.Location = new Point(this.x, this.y);
                    break;

                }

            }
            this.x = TargetScreen.WorkingArea.Right - base.Width - 5;

            switch (type)
            {
                case EnmType.Success:
                    this.pictureBox2.Image = Properties.Resources.NotifyWindowSuccess;
                    this.BackColor = Color.FromArgb(0x00, 0x6e, 0x99);//Color.SeaGreen;
                    break;
                case EnmType.Error:
                    this.pictureBox2.Image = Properties.Resources.NotifyWindowError;
                    this.BackColor = Color.FromArgb(0x66, 0x00, 0x00);//Color.DarkRed;
                    break;
                case EnmType.Info:
                    this.pictureBox2.Image = Properties.Resources.NotifyWindowInfo;
                    this.BackColor = Color.FromArgb(29, 54, 92);// Color.RoyalBlue;
                    break;
                case EnmType.Warning:
                    this.pictureBox2.Image = Properties.Resources.NotifyWindowWarning;
                    this.BackColor = Color.FromArgb(0x99, 0x54, 0x00);// Color.DarkOrange;
                    break;
            }


            this.lblTitle.Text = title;
            this.lblMsg.Text = msg;

            this.Show();
            this.action = EnmAction.start;
            this.timer1.Interval = 1;
            this.timer1.Start();
        }
    }
}
