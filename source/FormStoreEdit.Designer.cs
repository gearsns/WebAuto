namespace WebAuto
{
    partial class FormStoreEdit
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
            buttonClose = new Button();
            label1 = new Label();
            textBoxName = new TextBox();
            buttonSave = new Button();
            buttonDelete = new Button();
            labelError = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // buttonClose
            // 
            buttonClose.FlatAppearance.BorderSize = 0;
            buttonClose.FlatStyle = FlatStyle.Flat;
            buttonClose.Location = new Point(3, 3);
            buttonClose.Name = "buttonClose";
            buttonClose.Size = new Size(27, 27);
            buttonClose.TabIndex = 0;
            buttonClose.Text = "❮";
            buttonClose.UseVisualStyleBackColor = true;
            buttonClose.Click += ButtonClose_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 33);
            label1.Name = "label1";
            label1.Size = new Size(55, 15);
            label1.TabIndex = 1;
            label1.Text = "パターン名";
            // 
            // textBoxName
            // 
            textBoxName.Location = new Point(12, 51);
            textBoxName.Name = "textBoxName";
            textBoxName.Size = new Size(196, 23);
            textBoxName.TabIndex = 2;
            // 
            // buttonSave
            // 
            buttonSave.BackColor = Color.DodgerBlue;
            buttonSave.FlatAppearance.BorderSize = 0;
            buttonSave.FlatStyle = FlatStyle.Flat;
            buttonSave.ForeColor = Color.White;
            buttonSave.Location = new Point(113, 5);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(45, 25);
            buttonSave.TabIndex = 3;
            buttonSave.Text = "保存";
            buttonSave.UseVisualStyleBackColor = false;
            buttonSave.Click += ButtonSave_Click;
            // 
            // buttonDelete
            // 
            buttonDelete.BackColor = Color.Chocolate;
            buttonDelete.FlatAppearance.BorderSize = 0;
            buttonDelete.FlatStyle = FlatStyle.Flat;
            buttonDelete.ForeColor = Color.White;
            buttonDelete.Location = new Point(164, 5);
            buttonDelete.Name = "buttonDelete";
            buttonDelete.Size = new Size(44, 25);
            buttonDelete.TabIndex = 4;
            buttonDelete.Text = "削除";
            buttonDelete.UseVisualStyleBackColor = false;
            buttonDelete.Click += ButtonDelete_Click;
            // 
            // labelError
            // 
            labelError.AutoSize = true;
            labelError.ForeColor = Color.Firebrick;
            labelError.Location = new Point(15, 80);
            labelError.Name = "labelError";
            labelError.Size = new Size(0, 15);
            labelError.TabIndex = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(15, 80);
            label2.Name = "label2";
            label2.Size = new Size(286, 15);
            label2.TabIndex = 6;
            label2.Text = "「保存」すると項目名・キー・値の一覧も一緒に保存されます";
            // 
            // FormStoreEdit
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(385, 450);
            Controls.Add(label2);
            Controls.Add(labelError);
            Controls.Add(buttonDelete);
            Controls.Add(buttonSave);
            Controls.Add(textBoxName);
            Controls.Add(label1);
            Controls.Add(buttonClose);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormStoreEdit";
            Text = "FormStoreEdit";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonClose;
        private Label label1;
        private TextBox textBoxName;
        private Button buttonSave;
        private Button buttonDelete;
        private Label labelError;
        private Label label2;
    }
}