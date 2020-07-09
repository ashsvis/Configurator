using ModelHolder.Common;
using System;
using System.Windows.Forms;

namespace BaseEditor
{
    public partial class PropertyEditorForm : Form
    {
        public PropertyEditorForm()
        {
            InitializeComponent();
        }

        private void PropertyEditorForm_Load(object sender, EventArgs e)
        {
            tbName.Text = "Prop";
            foreach (var item in Helper.GetPropTypes())
                cbDataKind.Items.Add(item);
            cbDataKind.SelectedItem = cbDataKind.Items[0];
        }

        private void tbName_TextChanged(object sender, EventArgs e)
        {
            btnOk.Enabled = !string.IsNullOrWhiteSpace(tbName.Text);
        }
    }
}
