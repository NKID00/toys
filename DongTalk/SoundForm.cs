using System;
using System.Threading;
using System.Windows.Forms;

namespace DongTalk
{
    public partial class SoundForm : Form
    {
        public SoundForm()
        {
            InitializeComponent();
            new Thread(new ParameterizedThreadStart(Program.Start)).Start(Handle);
        }

        private void SoundForm_Hide(object sender, EventArgs e)
        {
            Hide();
        }

        private void SoundForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }
    }
}
