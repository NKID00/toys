using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DongTalk
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new DongLib().DongBSOD(0xffffffff);
        }

        private bool calculating = false;

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (calculating)
            {
                return;
            }
            calculating = true;
            if (double.TryParse(this.textBox1.Text, out var v))
            {
                textBox2.Text = (v*1.8+32).ToString();
            }
            calculating = false;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (calculating)
            {
                return;
            }
            calculating = true;
            if (double.TryParse(this.textBox2.Text, out var v))
            {
                textBox1.Text = ((v-32)/1.8).ToString();
            }
            calculating = false;
        }
    }
}
