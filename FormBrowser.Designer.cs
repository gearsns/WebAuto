namespace WebAuto
{
    partial class FormBrowser
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBrowser));
            tabControlBrowser = new VerticalTabControl();
            tabPageAdd = new TabPage();
            splitContainer1 = new SplitContainer();
            panelToolBody = new Panel();
            panel1 = new Panel();
            buttonCollapse = new Button();
            labelToolName = new Label();
            panelToolList = new Panel();
            buttonBulk = new Button();
            buttonExtract = new Button();
            buttonExcel = new Button();
            buttonLink = new Button();
            contextMenuStripLink = new ContextMenuStrip(components);
            statusStrip = new StatusStrip();
            toolStripStatusLabelStatusText = new ToolStripStatusLabel();
            panelToolTop = new Panel();
            panel2 = new Panel();
            textBoxURL = new TextBox();
            buttonReload = new Button();
            buttonHome = new Component.ImageButton();
            buttonForward = new Component.ImageButton();
            buttonBack = new Component.ImageButton();
            tabControlBrowser.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panel1.SuspendLayout();
            panelToolList.SuspendLayout();
            statusStrip.SuspendLayout();
            panelToolTop.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // tabControlBrowser
            // 
            tabControlBrowser.Alignment = TabAlignment.Right;
            tabControlBrowser.Controls.Add(tabPageAdd);
            tabControlBrowser.Dock = DockStyle.Fill;
            tabControlBrowser.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControlBrowser.ItemSize = new Size(28, 100);
            tabControlBrowser.Location = new Point(0, 0);
            tabControlBrowser.Margin = new Padding(0);
            tabControlBrowser.Multiline = true;
            tabControlBrowser.Name = "tabControlBrowser";
            tabControlBrowser.Padding = new Point(0, 0);
            tabControlBrowser.SelectedIndex = 0;
            tabControlBrowser.ShowToolTips = true;
            tabControlBrowser.Size = new Size(468, 401);
            tabControlBrowser.SizeMode = TabSizeMode.Fixed;
            tabControlBrowser.TabIndex = 0;
            tabControlBrowser.AddedNewPage += TabControlBrowser_AddedNewPage;
            tabControlBrowser.Selected += TabControlBrowser_Selected;
            // 
            // tabPageAdd
            // 
            tabPageAdd.Location = new Point(4, 4);
            tabPageAdd.Name = "tabPageAdd";
            tabPageAdd.Size = new Size(360, 393);
            tabPageAdd.TabIndex = 1;
            tabPageAdd.Text = "新しいタブを追加";
            tabPageAdd.ToolTipText = "新しいタブを追加";
            tabPageAdd.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.Location = new Point(28, 27);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(panelToolBody);
            splitContainer1.Panel1.Controls.Add(panel1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(tabControlBrowser);
            splitContainer1.Size = new Size(772, 401);
            splitContainer1.SplitterDistance = 300;
            splitContainer1.TabIndex = 1;
            // 
            // panelToolBody
            // 
            panelToolBody.BackColor = SystemColors.Control;
            panelToolBody.Dock = DockStyle.Fill;
            panelToolBody.Location = new Point(0, 29);
            panelToolBody.Margin = new Padding(3, 3, 3, 8);
            panelToolBody.Name = "panelToolBody";
            panelToolBody.Size = new Size(300, 372);
            panelToolBody.TabIndex = 1;
            // 
            // panel1
            // 
            panel1.AutoSize = true;
            panel1.Controls.Add(buttonCollapse);
            panel1.Controls.Add(labelToolName);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(300, 29);
            panel1.TabIndex = 2;
            // 
            // buttonCollapse
            // 
            buttonCollapse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonCollapse.FlatAppearance.BorderSize = 0;
            buttonCollapse.FlatStyle = FlatStyle.Flat;
            buttonCollapse.Location = new Point(274, 3);
            buttonCollapse.Name = "buttonCollapse";
            buttonCollapse.Size = new Size(26, 23);
            buttonCollapse.TabIndex = 1;
            buttonCollapse.Text = "✖";
            buttonCollapse.UseVisualStyleBackColor = true;
            buttonCollapse.Click += ButtonCollapse_Click;
            // 
            // labelToolName
            // 
            labelToolName.AutoSize = true;
            labelToolName.Location = new Point(6, 9);
            labelToolName.Name = "labelToolName";
            labelToolName.Size = new Size(101, 15);
            labelToolName.TabIndex = 0;
            labelToolName.Text = "EXCELから取り込み";
            // 
            // panelToolList
            // 
            panelToolList.Controls.Add(buttonBulk);
            panelToolList.Controls.Add(buttonExtract);
            panelToolList.Controls.Add(buttonExcel);
            panelToolList.Dock = DockStyle.Left;
            panelToolList.Location = new Point(0, 27);
            panelToolList.Name = "panelToolList";
            panelToolList.Size = new Size(28, 401);
            panelToolList.TabIndex = 0;
            // 
            // buttonBulk
            // 
            buttonBulk.BackgroundImage = Properties.Resources.bulk;
            buttonBulk.BackgroundImageLayout = ImageLayout.Stretch;
            buttonBulk.FlatAppearance.BorderSize = 0;
            buttonBulk.FlatStyle = FlatStyle.Flat;
            buttonBulk.Location = new Point(0, 26);
            buttonBulk.Name = "buttonBulk";
            buttonBulk.Size = new Size(26, 26);
            buttonBulk.TabIndex = 4;
            buttonBulk.UseVisualStyleBackColor = true;
            buttonBulk.Click += ButtonBulk_Click;
            // 
            // buttonExtract
            // 
            buttonExtract.BackgroundImage = Properties.Resources.extract;
            buttonExtract.BackgroundImageLayout = ImageLayout.Stretch;
            buttonExtract.FlatAppearance.BorderSize = 0;
            buttonExtract.FlatStyle = FlatStyle.Flat;
            buttonExtract.Location = new Point(0, 52);
            buttonExtract.Name = "buttonExtract";
            buttonExtract.Size = new Size(26, 26);
            buttonExtract.TabIndex = 2;
            buttonExtract.UseVisualStyleBackColor = true;
            buttonExtract.Click += ButtonExtract_Click;
            // 
            // buttonExcel
            // 
            buttonExcel.BackgroundImage = Properties.Resources.excel;
            buttonExcel.BackgroundImageLayout = ImageLayout.Stretch;
            buttonExcel.FlatAppearance.BorderSize = 0;
            buttonExcel.FlatStyle = FlatStyle.Flat;
            buttonExcel.Location = new Point(0, 0);
            buttonExcel.Name = "buttonExcel";
            buttonExcel.Size = new Size(26, 26);
            buttonExcel.TabIndex = 0;
            buttonExcel.UseVisualStyleBackColor = true;
            buttonExcel.Click += ButtonExcel_Click;
            // 
            // buttonLink
            // 
            buttonLink.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonLink.BackgroundImage = Properties.Resources.link;
            buttonLink.BackgroundImageLayout = ImageLayout.Stretch;
            buttonLink.FlatAppearance.BorderSize = 0;
            buttonLink.FlatStyle = FlatStyle.Flat;
            buttonLink.Location = new Point(771, 0);
            buttonLink.Name = "buttonLink";
            buttonLink.Size = new Size(26, 26);
            buttonLink.TabIndex = 1;
            buttonLink.UseVisualStyleBackColor = true;
            buttonLink.Click += ButtonLink_Click;
            // 
            // contextMenuStripLink
            // 
            contextMenuStripLink.Name = "contextMenuStripLink";
            contextMenuStripLink.ShowImageMargin = false;
            contextMenuStripLink.Size = new Size(36, 4);
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelStatusText });
            statusStrip.Location = new Point(0, 428);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(800, 22);
            statusStrip.TabIndex = 2;
            statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabelStatusText
            // 
            toolStripStatusLabelStatusText.Name = "toolStripStatusLabelStatusText";
            toolStripStatusLabelStatusText.Overflow = ToolStripItemOverflow.Never;
            toolStripStatusLabelStatusText.Size = new Size(0, 17);
            // 
            // panelToolTop
            // 
            panelToolTop.Controls.Add(panel2);
            panelToolTop.Controls.Add(buttonReload);
            panelToolTop.Controls.Add(buttonLink);
            panelToolTop.Controls.Add(buttonHome);
            panelToolTop.Controls.Add(buttonForward);
            panelToolTop.Controls.Add(buttonBack);
            panelToolTop.Dock = DockStyle.Top;
            panelToolTop.Location = new Point(0, 0);
            panelToolTop.Name = "panelToolTop";
            panelToolTop.Size = new Size(800, 27);
            panelToolTop.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel2.BackColor = SystemColors.Window;
            panel2.Controls.Add(textBoxURL);
            panel2.Location = new Point(124, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(641, 20);
            panel2.TabIndex = 6;
            // 
            // textBoxURL
            // 
            textBoxURL.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxURL.BorderStyle = BorderStyle.None;
            textBoxURL.Location = new Point(2, 2);
            textBoxURL.Name = "textBoxURL";
            textBoxURL.Size = new Size(636, 16);
            textBoxURL.TabIndex = 3;
            textBoxURL.Enter += TextBoxURL_Enter;
            textBoxURL.KeyUp += TextBoxURL_KeyUp;
            // 
            // buttonReload
            // 
            buttonReload.BackgroundImage = Properties.Resources.refresh;
            buttonReload.BackgroundImageLayout = ImageLayout.Stretch;
            buttonReload.FlatAppearance.BorderSize = 0;
            buttonReload.FlatStyle = FlatStyle.Flat;
            buttonReload.Location = new Point(60, 0);
            buttonReload.Name = "buttonReload";
            buttonReload.Size = new Size(26, 26);
            buttonReload.TabIndex = 5;
            buttonReload.UseVisualStyleBackColor = true;
            buttonReload.Click += buttonReload_Click;
            // 
            // buttonHome
            // 
            buttonHome.BackgroundImage = Properties.Resources.home;
            buttonHome.BackgroundImageLayout = ImageLayout.Stretch;
            buttonHome.FlatAppearance.BorderSize = 0;
            buttonHome.FlatStyle = FlatStyle.Flat;
            buttonHome.Location = new Point(92, 0);
            buttonHome.Name = "buttonHome";
            buttonHome.Size = new Size(26, 26);
            buttonHome.TabIndex = 2;
            buttonHome.UseVisualStyleBackColor = true;
            buttonHome.Click += ButtonHome_Click;
            // 
            // buttonForward
            // 
            buttonForward.BackgroundImage = Properties.Resources.right_line;
            buttonForward.BackgroundImageLayout = ImageLayout.Stretch;
            buttonForward.Enabled = false;
            buttonForward.FlatAppearance.BorderSize = 0;
            buttonForward.FlatStyle = FlatStyle.Flat;
            buttonForward.Location = new Point(28, 0);
            buttonForward.Name = "buttonForward";
            buttonForward.Size = new Size(26, 26);
            buttonForward.TabIndex = 1;
            buttonForward.UseVisualStyleBackColor = true;
            buttonForward.Click += ButtonForward_Click;
            // 
            // buttonBack
            // 
            buttonBack.BackgroundImage = Properties.Resources.left_line;
            buttonBack.BackgroundImageLayout = ImageLayout.Stretch;
            buttonBack.Enabled = false;
            buttonBack.FlatAppearance.BorderSize = 0;
            buttonBack.FlatStyle = FlatStyle.Flat;
            buttonBack.Location = new Point(0, 0);
            buttonBack.Name = "buttonBack";
            buttonBack.Size = new Size(26, 26);
            buttonBack.TabIndex = 0;
            buttonBack.UseVisualStyleBackColor = true;
            buttonBack.Click += ButtonBack_Click;
            // 
            // FormBrowser
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(splitContainer1);
            Controls.Add(panelToolList);
            Controls.Add(statusStrip);
            Controls.Add(panelToolTop);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormBrowser";
            Text = "データ入力支援ツール";
            FormClosing += FormBrowser_FormClosing;
            Load += FormBrowser_Load;
            tabControlBrowser.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panelToolList.ResumeLayout(false);
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            panelToolTop.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private VerticalTabControl tabControlBrowser;
        private SplitContainer splitContainer1;
        private TabPage tabPageAdd;
        private Panel panelToolBody;
        private Label labelToolName;
        private Panel panelToolList;
        private Button buttonExcel;
        private Button buttonLink;
        private Button buttonCollapse;
        private Panel panel1;
        private Button buttonExtract;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel toolStripStatusLabelStatusText;
        private Panel panelToolTop;
        private Component.ImageButton buttonBack;
        private Component.ImageButton buttonForward;
        private Component.ImageButton buttonHome;
        private TextBox textBoxURL;
        private Button buttonReload;
        private Button buttonBulk;
        private ContextMenuStrip contextMenuStripLink;
        private Panel panel2;
    }
}
