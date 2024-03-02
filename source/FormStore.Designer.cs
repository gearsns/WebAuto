namespace WebAuto
{
    partial class FormStore
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
            buttonInput = new Button();
            comboBoxSheet = new ComboBox();
            MyDataGridViewInput = new MyDataGridView();
            ColumnName = new DataGridViewTextBoxColumn();
            ColumnKey = new DataGridViewTextBoxColumn();
            ColumnValue = new DataGridViewTextBoxColumn();
            contextMenuStripValue = new ContextMenuStrip(components);
            ToolStripMenuItemToday = new ToolStripMenuItem();
            ToolStripMenuItemTime = new ToolStripMenuItem();
            ToolStripMenuItemMonthFirstDay = new ToolStripMenuItem();
            ToolStripMenuItemMonthLastDay = new ToolStripMenuItem();
            ToolStripMenuItemPreMonthFirstDay = new ToolStripMenuItem();
            ToolStripMenuItemPreMonthLastDay = new ToolStripMenuItem();
            ToolStripMenuItemMonthCalendarCell = new ToolStripMenuItem();
            buttonEdit = new Button();
            buttonCancel = new Button();
            buttonExtract = new Button();
            buttonClear = new Button();
            ((System.ComponentModel.ISupportInitialize)MyDataGridViewInput).BeginInit();
            contextMenuStripValue.SuspendLayout();
            SuspendLayout();
            // 
            // buttonInput
            // 
            buttonInput.BackColor = Color.DodgerBlue;
            buttonInput.FlatAppearance.BorderSize = 0;
            buttonInput.FlatStyle = FlatStyle.Flat;
            buttonInput.ForeColor = Color.White;
            buttonInput.Location = new Point(2, 2);
            buttonInput.Name = "buttonInput";
            buttonInput.Size = new Size(52, 39);
            buttonInput.TabIndex = 26;
            buttonInput.Text = "自動\r\n入力";
            buttonInput.UseVisualStyleBackColor = false;
            buttonInput.Click += ButtonInput_Click;
            // 
            // comboBoxSheet
            // 
            comboBoxSheet.DisplayMember = "Name";
            comboBoxSheet.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxSheet.FlatStyle = FlatStyle.Flat;
            comboBoxSheet.FormattingEnabled = true;
            comboBoxSheet.Location = new Point(2, 47);
            comboBoxSheet.Name = "comboBoxSheet";
            comboBoxSheet.Size = new Size(215, 23);
            comboBoxSheet.TabIndex = 27;
            comboBoxSheet.ValueMember = "Name";
            comboBoxSheet.SelectedIndexChanged += ComboBoxSheet_SelectedIndexChanged;
            // 
            // MyDataGridViewInput
            // 
            MyDataGridViewInput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            MyDataGridViewInput.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            MyDataGridViewInput.Columns.AddRange(new DataGridViewColumn[] { ColumnName, ColumnKey, ColumnValue });
            MyDataGridViewInput.Location = new Point(2, 76);
            MyDataGridViewInput.Name = "MyDataGridViewInput";
            MyDataGridViewInput.Size = new Size(722, 370);
            MyDataGridViewInput.TabIndex = 28;
            // 
            // ColumnName
            // 
            ColumnName.HeaderText = "項目名";
            ColumnName.Name = "ColumnName";
            // 
            // ColumnKey
            // 
            ColumnKey.HeaderText = "キー";
            ColumnKey.Name = "ColumnKey";
            // 
            // ColumnValue
            // 
            ColumnValue.ContextMenuStrip = contextMenuStripValue;
            ColumnValue.HeaderText = "値";
            ColumnValue.Name = "ColumnValue";
            ColumnValue.Width = 200;
            // 
            // contextMenuStripValue
            // 
            contextMenuStripValue.Items.AddRange(new ToolStripItem[] { ToolStripMenuItemToday, ToolStripMenuItemTime, ToolStripMenuItemMonthFirstDay, ToolStripMenuItemMonthLastDay, ToolStripMenuItemPreMonthFirstDay, ToolStripMenuItemPreMonthLastDay, ToolStripMenuItemMonthCalendarCell });
            contextMenuStripValue.Name = "contextMenuStripValue";
            contextMenuStripValue.Size = new Size(243, 158);
            // 
            // ToolStripMenuItemToday
            // 
            ToolStripMenuItemToday.Name = "ToolStripMenuItemToday";
            ToolStripMenuItemToday.Size = new Size(242, 22);
            ToolStripMenuItemToday.Text = "今日の日付(yyyy-mm-dd)";
            ToolStripMenuItemToday.Click += ToolStripMenuItemToday_Click;
            // 
            // ToolStripMenuItemTime
            // 
            ToolStripMenuItemTime.Name = "ToolStripMenuItemTime";
            ToolStripMenuItemTime.Size = new Size(242, 22);
            ToolStripMenuItemTime.Text = "今の時間(hh:mm)";
            ToolStripMenuItemTime.Click += ToolStripMenuItemTime_Click;
            // 
            // ToolStripMenuItemMonthFirstDay
            // 
            ToolStripMenuItemMonthFirstDay.Name = "ToolStripMenuItemMonthFirstDay";
            ToolStripMenuItemMonthFirstDay.Size = new Size(242, 22);
            ToolStripMenuItemMonthFirstDay.Text = "月初の日付(yyyy-mm-01)";
            ToolStripMenuItemMonthFirstDay.Click += ToolStripMenuItemMonthFirstDay_Click;
            // 
            // ToolStripMenuItemMonthLastDay
            // 
            ToolStripMenuItemMonthLastDay.Name = "ToolStripMenuItemMonthLastDay";
            ToolStripMenuItemMonthLastDay.Size = new Size(242, 22);
            ToolStripMenuItemMonthLastDay.Text = "月末の日付(yyyy-mm-最終日)";
            ToolStripMenuItemMonthLastDay.Click += ToolStripMenuItemMonthLastDay_Click;
            // 
            // ToolStripMenuItemPreMonthFirstDay
            // 
            ToolStripMenuItemPreMonthFirstDay.Name = "ToolStripMenuItemPreMonthFirstDay";
            ToolStripMenuItemPreMonthFirstDay.Size = new Size(242, 22);
            ToolStripMenuItemPreMonthFirstDay.Text = "前月初の日付(yyyy-mm-01)";
            ToolStripMenuItemPreMonthFirstDay.Click += ToolStripMenuItemPreMonthFirstDay_Click;
            // 
            // ToolStripMenuItemPreMonthLastDay
            // 
            ToolStripMenuItemPreMonthLastDay.Name = "ToolStripMenuItemPreMonthLastDay";
            ToolStripMenuItemPreMonthLastDay.Size = new Size(242, 22);
            ToolStripMenuItemPreMonthLastDay.Text = "前月末の日付(yyyy-mm-最終日)";
            ToolStripMenuItemPreMonthLastDay.Click += ToolStripMenuItemPreMonthLastDay_Click;
            // 
            // ToolStripMenuItemMonthCalendarCell
            // 
            ToolStripMenuItemMonthCalendarCell.Name = "ToolStripMenuItemMonthCalendarCell";
            ToolStripMenuItemMonthCalendarCell.Size = new Size(242, 22);
            ToolStripMenuItemMonthCalendarCell.Text = "カレンダーから日付を指定";
            ToolStripMenuItemMonthCalendarCell.Click += ToolStripMenuItemMonthCalendarCell_Click;
            // 
            // buttonEdit
            // 
            buttonEdit.BackColor = Color.Chocolate;
            buttonEdit.FlatAppearance.BorderSize = 0;
            buttonEdit.FlatStyle = FlatStyle.Flat;
            buttonEdit.ForeColor = Color.White;
            buttonEdit.Location = new Point(223, 47);
            buttonEdit.Name = "buttonEdit";
            buttonEdit.Size = new Size(64, 23);
            buttonEdit.TabIndex = 29;
            buttonEdit.Text = "設定";
            buttonEdit.UseVisualStyleBackColor = false;
            buttonEdit.Click += ButtonEdit_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.BackColor = Color.Crimson;
            buttonCancel.FlatAppearance.BorderSize = 0;
            buttonCancel.FlatStyle = FlatStyle.Flat;
            buttonCancel.ForeColor = Color.White;
            buttonCancel.Location = new Point(2, 2);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(52, 39);
            buttonCancel.TabIndex = 30;
            buttonCancel.Text = "中断";
            buttonCancel.UseVisualStyleBackColor = false;
            buttonCancel.Visible = false;
            buttonCancel.Click += ButtonCancel_Click;
            // 
            // buttonExtract
            // 
            buttonExtract.BackColor = Color.ForestGreen;
            buttonExtract.FlatAppearance.BorderSize = 0;
            buttonExtract.FlatStyle = FlatStyle.Flat;
            buttonExtract.ForeColor = Color.White;
            buttonExtract.Location = new Point(60, 2);
            buttonExtract.Name = "buttonExtract";
            buttonExtract.Size = new Size(100, 39);
            buttonExtract.TabIndex = 31;
            buttonExtract.Text = "Webページ\r\nから取り込む";
            buttonExtract.UseVisualStyleBackColor = false;
            buttonExtract.Click += ButtonExtract_Click;
            // 
            // buttonClear
            // 
            buttonClear.BackColor = Color.Goldenrod;
            buttonClear.FlatAppearance.BorderSize = 0;
            buttonClear.FlatStyle = FlatStyle.Flat;
            buttonClear.ForeColor = Color.WhiteSmoke;
            buttonClear.Location = new Point(166, 2);
            buttonClear.Name = "buttonClear";
            buttonClear.Size = new Size(51, 39);
            buttonClear.TabIndex = 32;
            buttonClear.Text = "クリア";
            buttonClear.UseVisualStyleBackColor = false;
            buttonClear.Click += ButtonClear_Click;
            // 
            // FormStore
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(727, 450);
            Controls.Add(buttonClear);
            Controls.Add(buttonExtract);
            Controls.Add(buttonCancel);
            Controls.Add(buttonEdit);
            Controls.Add(MyDataGridViewInput);
            Controls.Add(comboBoxSheet);
            Controls.Add(buttonInput);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormStore";
            Text = "登録パターン";
            ((System.ComponentModel.ISupportInitialize)MyDataGridViewInput).EndInit();
            contextMenuStripValue.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Button buttonInput;
        private ComboBox comboBoxSheet;
        private MyDataGridView MyDataGridViewInput;
        private Button buttonEdit;
        private Button buttonCancel;
        private Button buttonExtract;
        private Button buttonClear;
        private DataGridViewTextBoxColumn ColumnName;
        private DataGridViewTextBoxColumn ColumnKey;
        private DataGridViewTextBoxColumn ColumnValue;
        private ContextMenuStrip contextMenuStripValue;
        private ToolStripMenuItem ToolStripMenuItemToday;
        private ToolStripMenuItem ToolStripMenuItemTime;
        private ToolStripMenuItem ToolStripMenuItemMonthFirstDay;
        private ToolStripMenuItem ToolStripMenuItemMonthLastDay;
        private ToolStripMenuItem ToolStripMenuItemPreMonthFirstDay;
        private ToolStripMenuItem ToolStripMenuItemPreMonthLastDay;
        private ToolStripMenuItem ToolStripMenuItemMonthCalendarCell;
    }
}