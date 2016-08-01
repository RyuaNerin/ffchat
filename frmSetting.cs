using System;
using System.Windows.Forms;

namespace FFChat
{
    public partial class frmSetting : Form
    {
        private readonly frmMain m_parent;

        public frmSetting(frmMain parent)
        {
            InitializeComponent();

            this.m_parent = parent;
        }

        private void frmSetting_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void button_ResetProcess_Click(object sender, EventArgs e)
        {
            this.m_parent.ResetProcess();
        }

        private void button_SelectProcess_Click(object sender, EventArgs e)
        {
            this.m_parent.SelectProcess(int.Parse(((string)this.comboBox_Process.SelectedItem).Split(':')[1]));            
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            var ctl = (CheckBox)sender;
            this.m_parent.SetLog(int.Parse((string)ctl.Tag), ctl.Checked);
        }
    }
}
