using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace DongTalk
{
    public partial class HiddenForm : Form
    {
        public HiddenForm()
        {
            InitializeComponent();
            new Thread(new ParameterizedThreadStart(Program.Start)).Start(Handle);
        }

        private void HiddenForm_Hide(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void HiddenForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
