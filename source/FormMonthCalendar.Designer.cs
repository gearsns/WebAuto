namespace WebAuto
{
    partial class FormMonthCalendar
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
            monthCalendar = new MonthCalendar();
            SuspendLayout();
            // 
            // monthCalendar
            // 
            monthCalendar.Location = new Point(0, 0);
            monthCalendar.Name = "monthCalendar";
            monthCalendar.TabIndex = 0;
            monthCalendar.DateSelected += MonthCalendar_DateSelected;
            monthCalendar.KeyDown += MonthCalendar_KeyDown;
            // 
            // FormMonthCalendar
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(198, 163);
            Controls.Add(monthCalendar);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormMonthCalendar";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Text = "日付を選択";
            Load += FormMonthCalendar_Load;
            ResumeLayout(false);
        }

        #endregion

        private MonthCalendar monthCalendar;
    }
}