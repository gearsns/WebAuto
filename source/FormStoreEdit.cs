namespace WebAuto
{
    public partial class FormStoreEdit : Form
    {
        public event Action<string>? Save;
        public string name = "";
        public FormStoreEdit()
        {
            InitializeComponent();
        }

        public void Show(Control control, string name)
        {
            TopLevel = false;
            control.Controls.Add(this);
            textBoxName.Text = name;
            Dock = DockStyle.Fill;
            Show();
            BringToFront();
            _ = textBoxName.Focus();
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            labelError.Text = "";
            if (textBoxName.Text.Length == 0)
            {
                labelError.Text = "パターン名を入力してください。";
                textBoxName.Focus();
                return;
            }
            Save?.Invoke(textBoxName.Text);
            Close();
        }

        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show("削除しますか？", "確認", MessageBoxButtons.YesNo))
            {
                Save?.Invoke("");
                Close();
            }
        }
    }
}
