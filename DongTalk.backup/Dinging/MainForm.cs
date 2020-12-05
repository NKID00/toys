using System;
using System.Windows.Forms;

namespace Dinging
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void StartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Hide();
            Program.Start();
        }
    }
}
