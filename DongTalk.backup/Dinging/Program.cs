using System;
using System.Drawing;
using System.Globalization;
using System.Media;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.DirectX.DirectSound;

namespace Dinging
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(IntPtr classname, string title);
        [DllImport("user32.dll")]
        private static extern void MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool repaint);
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out Rectangle rect);
        [DllImport("user32.dll")]
        private static extern bool SetWindowText(IntPtr hWnd, string title);
        [DllImport("user32.dll")]
        private static extern int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private static ulong msgid = 0;
        private static object lock_obj = new object();
        private static void Msg(int x, int y,string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            var tcaption = msgid.ToString();
            new Thread(new ThreadStart(() =>
            {
                new Thread(new ThreadStart(() =>
                {
                    MessageBox.Show(text, tcaption, buttons, icon);
                }
                )).Start();
                var handle = IntPtr.Zero;
                while ((handle = FindWindow(IntPtr.Zero, tcaption)) == IntPtr.Zero) ;
                GetWindowRect(handle, out var r);
                MoveWindow(handle, x, y, r.Width - r.X, r.Height - r.Y, true);
                SetWindowText(handle, caption);
                lock (lock_obj)
                {
                    PostMessage(handle, 0x10, IntPtr.Zero, IntPtr.Zero);
                }
            }
            )).Start();
            msgid++;
        }

        public static void Start()
        {
            msgid = ulong.Parse(Guid.NewGuid().ToString("N").Substring(24, 8), NumberStyles.HexNumber);
            var = new Device();
            lock(lock_obj)
            {
                for (var i = 30; i < 500; i += 30)
                {
                    Msg(i, i, "如果保存此图片任何透明度将丢失。是否要继续？", "画图", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    Thread.Sleep(500);
                }
                for (var i = 30; i < 500; i += 30)
                {
                    var rm = new ResourceManager("Dinging.Properties.Resources", Assembly.GetExecutingAssembly());
                    var sp = new SoundPlayer(rm.GetStream("wave0"));
                    sp.Play();
                    Thread.Sleep(500);
                }
                Thread.Sleep(1000);
            }
        }
    }
}
