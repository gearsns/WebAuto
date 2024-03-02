namespace WebAuto.Resources
{
    partial class NotifyWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NotifyWindow));
            pictureBox1 = new PictureBox();
            timer1 = new System.Windows.Forms.Timer(components);
            pictureBox2 = new PictureBox();
            lblMsg = new Label();
            lblTitle = new Label();
            pictureBox3 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            pictureBox1.Image = Properties.Resources.NotifyWindowCancel;
            pictureBox1.Location = new Point(488, 39);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(28, 28);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Click += PictureBox1_Click;
            // 
            // timer1
            // 
            timer1.Tick += Timer1_Tick;
            // 
            // pictureBox2
            // 
            pictureBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            pictureBox2.Image = Properties.Resources.NotifyWindowInfo;
            pictureBox2.Location = new Point(4, 42);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(48, 43);
            pictureBox2.TabIndex = 1;
            pictureBox2.TabStop = false;
            // 
            // lblMsg
            // 
            lblMsg.AutoSize = true;
            lblMsg.BackColor = Color.Transparent;
            lblMsg.Font = new Font("Yu Gothic UI", 12F);
            lblMsg.ForeColor = Color.White;
            lblMsg.Location = new Point(58, 42);
            lblMsg.Name = "lblMsg";
            lblMsg.Size = new Size(52, 21);
            lblMsg.TabIndex = 2;
            lblMsg.Text = "label1";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Yu Gothic UI", 12F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(26, 4);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(51, 21);
            lblTitle.TabIndex = 3;
            lblTitle.Text = "label1";
            // 
            // pictureBox3
            // 
            pictureBox3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            pictureBox3.BackColor = Color.Transparent;
            pictureBox3.Image = (Image)resources.GetObject("pictureBox3.Image");
            pictureBox3.Location = new Point(4, 6);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(18, 18);
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.TabIndex = 4;
            pictureBox3.TabStop = false;
            // 
            // NotifyWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DarkOrange;
            ClientSize = new Size(528, 105);
            Controls.Add(pictureBox3);
            Controls.Add(lblTitle);
            Controls.Add(lblMsg);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "NotifyWindow";
            ShowInTaskbar = false;
            Text = "NotifyWindow";
            TopMost = true;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private System.Windows.Forms.Timer timer1;
        private PictureBox pictureBox2;
        private Label lblMsg;
        private Label lblTitle;
        private PictureBox pictureBox3;
    }
}