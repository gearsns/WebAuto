namespace WebAuto
{
    partial class FormBulk
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            comboBoxTemplate = new ComboBox();
            buttonClear = new Button();
            buttonExcel = new Button();
            buttonInput = new Button();
            MyDataGridViewInput = new MyDataGridView();
            ColumnName = new DataGridViewTextBoxColumn();
            ColumnKey = new DataGridViewTextBoxColumn();
            ColumnValue = new DataGridViewTextBoxColumn();
            buttonCancel = new Button();
            numericUpDownMaxnum = new NumericUpDown();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)MyDataGridViewInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownMaxnum).BeginInit();
            SuspendLayout();
            // 
            // comboBoxTemplate
            // 
            comboBoxTemplate.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxTemplate.FlatStyle = FlatStyle.Flat;
            comboBoxTemplate.FormattingEnabled = true;
            comboBoxTemplate.Location = new Point(2, 47);
            comboBoxTemplate.Name = "comboBoxTemplate";
            comboBoxTemplate.Size = new Size(215, 23);
            comboBoxTemplate.TabIndex = 21;
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
            buttonClear.TabIndex = 20;
            buttonClear.Text = "クリア";
            buttonClear.UseVisualStyleBackColor = false;
            buttonClear.Click += ButtonClear_Click;
            // 
            // buttonExcel
            // 
            buttonExcel.BackColor = Color.ForestGreen;
            buttonExcel.FlatAppearance.BorderSize = 0;
            buttonExcel.FlatStyle = FlatStyle.Flat;
            buttonExcel.ForeColor = Color.WhiteSmoke;
            buttonExcel.Location = new Point(60, 2);
            buttonExcel.Name = "buttonExcel";
            buttonExcel.Size = new Size(100, 39);
            buttonExcel.TabIndex = 19;
            buttonExcel.Text = "テンプレート\r\nから取り込む";
            buttonExcel.UseVisualStyleBackColor = false;
            buttonExcel.Click += ButtonExcel_Click;
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
            buttonInput.TabIndex = 18;
            buttonInput.Text = "自動\r\n入力";
            buttonInput.UseVisualStyleBackColor = false;
            buttonInput.Click += ButtonInput_Click;
            // 
            // MyDataGridViewInput
            // 
            MyDataGridViewInput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            MyDataGridViewInput.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            MyDataGridViewInput.Columns.AddRange(new DataGridViewColumn[] { ColumnName, ColumnKey, ColumnValue });
            MyDataGridViewInput.Location = new Point(2, 76);
            MyDataGridViewInput.Name = "MyDataGridViewInput";
            MyDataGridViewInput.Size = new Size(722, 370);
            MyDataGridViewInput.TabIndex = 24;
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
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            ColumnValue.DefaultCellStyle = dataGridViewCellStyle1;
            ColumnValue.HeaderText = "値";
            ColumnValue.Name = "ColumnValue";
            ColumnValue.Width = 200;
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
            buttonCancel.TabIndex = 31;
            buttonCancel.Text = "中断";
            buttonCancel.UseVisualStyleBackColor = false;
            buttonCancel.Visible = false;
            buttonCancel.Click += ButtonCancel_Click;
            // 
            // numericUpDownMaxnum
            // 
            numericUpDownMaxnum.Location = new Point(223, 48);
            numericUpDownMaxnum.Name = "numericUpDownMaxnum";
            numericUpDownMaxnum.Size = new Size(62, 23);
            numericUpDownMaxnum.TabIndex = 32;
            numericUpDownMaxnum.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(224, 7);
            label1.Name = "label1";
            label1.Size = new Size(55, 30);
            label1.TabIndex = 33;
            label1.Text = "最大読込\r\n件数";
            // 
            // FormBulk
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(727, 450);
            Controls.Add(label1);
            Controls.Add(numericUpDownMaxnum);
            Controls.Add(buttonCancel);
            Controls.Add(MyDataGridViewInput);
            Controls.Add(comboBoxTemplate);
            Controls.Add(buttonClear);
            Controls.Add(buttonExcel);
            Controls.Add(buttonInput);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormBulk";
            Text = "テンプレートで登録";
            ((System.ComponentModel.ISupportInitialize)MyDataGridViewInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownMaxnum).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ComboBox comboBoxTemplate;
        private Button buttonClear;
        private Button buttonExcel;
        private Button buttonInput;
        private MyDataGridView MyDataGridViewInput;
        private Button buttonCancel;
        private NumericUpDown numericUpDownMaxnum;
        private Label label1;
        private DataGridViewTextBoxColumn ColumnName;
        private DataGridViewTextBoxColumn ColumnKey;
        private DataGridViewTextBoxColumn ColumnValue;
    }
}