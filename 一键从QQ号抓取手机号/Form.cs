using System;
using System.Threading;
using System.Windows.Forms;
using wnexe;

namespace 一键从QQ号抓取手机号
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = comboBox2.SelectedIndex = 0;
        }

        private void button_Click(object sender, EventArgs e)
        {
            Thread.Sleep(2200);
            MessageBox.Show("服务器连接失败。\r\nERR_CONNECTION_TIMED_OUT", "抓取失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

       static public int closed = 0;

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (closed == 0)
            {
                Hide();
                Form1 f = new Form1();
                f.Show();
                e.Cancel = true;
                closed = 1;
            }
            else if (closed == 1)
            {
                e.Cancel = true;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (closed == 2)
            {
                Close();
            }
        }
    }
}
