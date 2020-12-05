using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DongTalk
{
    public enum AlignX
    {
        Left, Middle, Right
    }

    public enum AlignY
    {
        Top, Middle, Bottom
    }
    public class DongLib
    {
        private readonly IntPtr DongLibHandle;

        public delegate ulong DongSoundInit_t(IntPtr hWnd);
        public delegate IntPtr DongSoundLoad_t(byte[] path);
        public delegate ulong DongSoundPlay_t(IntPtr buffer);
        public delegate ulong DongSoundLoadAndPlay_t(byte[] path);
        public delegate void DongBSOD_t(long code);
        public DongSoundInit_t DongSoundInit;
        public DongSoundLoad_t DongSoundLoad;
        public DongSoundPlay_t DongSoundPlay;
        public DongSoundLoadAndPlay_t DongSoundLoadAndPlay;
        public DongBSOD_t DongBSOD;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string path);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr handle, string funcname);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr handle);

        public DongLib()
        {
            var DongLibPath = Path.Combine(Path.GetTempPath(), "DongLib.dll");
            var fs = new FileStream(DongLibPath, FileMode.Create);
            fs.Write(Properties.Resources.DongLib, 0, Properties.Resources.DongLib.Length);
            fs.Flush();
            fs.Close();
            DongLibHandle = LoadLibrary(DongLibPath);
            var address = GetProcAddress(DongLibHandle, "_DongSoundInit@4");
            DongSoundInit = (DongSoundInit_t)Marshal.GetDelegateForFunctionPointer(address, typeof(DongSoundInit_t));
            address = GetProcAddress(DongLibHandle, "_DongSoundLoad@4");
            DongSoundLoad = (DongSoundLoad_t)Marshal.GetDelegateForFunctionPointer(address, typeof(DongSoundLoad_t));
            address = GetProcAddress(DongLibHandle, "_DongSoundPlay@4");
            DongSoundPlay = (DongSoundPlay_t)Marshal.GetDelegateForFunctionPointer(address, typeof(DongSoundPlay_t));
            address = GetProcAddress(DongLibHandle, "_DongSoundLoadAndPlay@4");
            DongSoundLoadAndPlay = (DongSoundLoadAndPlay_t)Marshal.GetDelegateForFunctionPointer(address, typeof(DongSoundLoadAndPlay_t));
            address = GetProcAddress(DongLibHandle, "_DongBSOD@4");
            DongBSOD = (DongBSOD_t)Marshal.GetDelegateForFunctionPointer(address, typeof(DongBSOD_t));
        }

        ~DongLib()
        {
            FreeLibrary(DongLibHandle);
        }
    }

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
            Application.Run(new HiddenForm());
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

        private static readonly object image_lock_obj = new object();
        private static void ShowImage(Image img, int x, int y, AlignX alignx, AlignY aligny)
        {
            new Thread(new ThreadStart(() =>
            {
                var imgform = new ImageForm(img, x, y, alignx, aligny);
                var image_handle = imgform.Handle;
                new Thread(new ThreadStart(() =>
                {
                    lock (image_lock_obj) { }
                    PostMessage(image_handle, 0x10, IntPtr.Zero, IntPtr.Zero);
                }
                )).Start();
                Application.Run(imgform);
            }
            )).Start();
        }

        private static Random rand;

        private static void ShowImageRandom(Image img)
        {
            ShowImage(img, rand.Next(0, w - img.Width), rand.Next(0, h - img.Height), AlignX.Left, AlignY.Top);
        }

        private static ulong msgid = 0x1234;
        private static readonly object msg_lock_obj = new object();

        private static void ShowMessageBox(int x, int y, AlignX alignx, AlignY aligny, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            var tcaption = msgid.ToString();
            new Thread(new ThreadStart(() =>
            {
                var handle = IntPtr.Zero;
                new Thread(new ThreadStart(() =>
                {
                    MessageBox.Show(text, tcaption, buttons, icon);
                }
                )).Start();
                while ((handle = FindWindow(IntPtr.Zero, tcaption)) == IntPtr.Zero) ;
                SetWindowText(handle, caption);
                GetWindowRect(handle, out var r);
                var rw = r.Width - r.X;
                var rh = r.Height - r.Y;
                switch (alignx)
                {
                    case AlignX.Left:
                        break;
                    case AlignX.Middle:
                        x -= rw / 2;
                        break;
                    case AlignX.Right:
                        x -= rw;
                        break;
                }
                switch (aligny)
                {
                    case AlignY.Top:
                        break;
                    case AlignY.Middle:
                        y -= rh / 2;
                        break;
                    case AlignY.Bottom:
                        y -= rh;
                        break;
                }
                MoveWindow(handle, x, y, rw, rh, true);
                lock (msg_lock_obj) { }
                PostMessage(handle, 0x10, IntPtr.Zero, IntPtr.Zero);
            }
            )).Start();
            msgid++;
        }

        private static void WriteWaveFile(string path, UnmanagedMemoryStream wave)
        {
            var l = wave.Length;
            var buffer = new byte[l];
            wave.Read(buffer, 0, (int)l);
            var fs = new FileStream(path, FileMode.Create);
            fs.Write(buffer, 0, (int)l);
            fs.Flush();
            fs.Close();
        }

        private static int w;
        private static int h;

        private static byte[] Wave0;
        private static byte[] Wave1;
        private static DongLib lib;

        private static Image[] images;

        private static void Start_()
        {
            var rand = new Random();
            var buffer0 = lib.DongSoundLoad(Wave0);
            var buffer1 = lib.DongSoundLoad(Wave1);
            lock (msg_lock_obj)
            {
                ShowMessageBox(
                    w / 2, h / 2,
                    AlignX.Middle, AlignY.Middle,
                    "0xffffffff 指令引用的 0x00000000 内存。该内存不能为read。\r\n"
                    + "\r\n"
                    + "要终止程序，请单击“确定”。\r\n",
                    "DongTalk.exe - 应用程序错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );
                lib.DongSoundPlay(buffer1);
                Thread.Sleep(3500);
            }
            lock (image_lock_obj)
            {
                for (int i = 0; i < 3; i++)
                {
                    foreach (var item in images)
                    {
                        ShowImageRandom(item);
                        lib.DongSoundPlay(buffer0);
                        Thread.Sleep(600);
                    }
                }
                Thread.Sleep(1500);
            }
            Thread.Sleep(1000);
            lock (msg_lock_obj)
            {
                ShowMessageBox(
                    w / 2, h / 2,
                    AlignX.Middle, AlignY.Middle,
                    "ROUND 2!", "",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                    );
                Thread.Sleep(3500);
            }
            Thread.Sleep(1000);
            lock (image_lock_obj)
            {
                for (int i = 0; i < 5; i++)
                {
                    foreach (var item in images)
                    {
                        ShowImageRandom(item);
                        lib.DongSoundLoadAndPlay(Wave0);
                        Thread.Sleep(300);
                    }
                }
                Thread.Sleep(1500);
            }
            Thread.Sleep(1000);
            lock (msg_lock_obj)
            {
                ShowMessageBox(
                    w / 2, h / 2,
                    AlignX.Middle, AlignY.Middle,
                    "FINAL WAVE!!!", "",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                    );
                Thread.Sleep(3500);
            }
            Thread.Sleep(1000);
            lock (image_lock_obj)
            {
                for (int i = 0; i < 20; i++)
                {
                    foreach (var item in images)
                    {
                        try
                        {
                            ShowImageRandom(item);
                            lib.DongSoundLoadAndPlay(Wave0);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                Thread.Sleep(5000);
            }
            /*
            lock (msg_lock_obj)
            {
                for (int i = 50; i < h / 3; i += 30)
                {
                    ShowMessageBox(
                        (int)(i * 1.5), i,
                        AlignX.Left, AlignY.Top,
                        "如果保存此图片任何透明度将丢失。是否要继续？\r\n",
                        "画图",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning
                        );
                    Thread.Sleep(700);
                }
                for (int i = 50; i < h / 3; i += 30)
                {
                    ShowMessageBox(
                        w - (int)(i * 1.5), i,
                        AlignX.Right, AlignY.Top,
                        "如果保存此图片任何透明度将丢失。是否要继续？\r\n",
                        "画图",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning
                        );
                    Thread.Sleep(700);
                }
                Thread.Sleep(5000);
            }
            */
        }

        public static void Start(Object obj)
        {
            w = Screen.PrimaryScreen.Bounds.Width;
            h = Screen.PrimaryScreen.Bounds.Height;
            var Wave0Path = Path.Combine(Path.GetTempPath(), "Wave0.wav");
            var Wave1Path = Path.Combine(Path.GetTempPath(), "Wave1.wav");
            WriteWaveFile(Wave0Path, Properties.Resources.WAVE0);
            WriteWaveFile(Wave1Path, Properties.Resources.WAVE1);
            Wave0 = Encoding.ASCII.GetBytes(Wave0Path);
            Wave1 = Encoding.ASCII.GetBytes(Wave1Path);
            lib = new DongLib();
            var e = lib.DongSoundInit((IntPtr)obj);
            rand = new Random(Guid.NewGuid().ToString().GetHashCode());
            images = new Image[]
            {
                Properties.Resources.IMAGE0,
                Properties.Resources.IMAGE1,
                Properties.Resources.IMAGE2,
                Properties.Resources.IMAGE3,
                Properties.Resources.IMAGE4,
                Properties.Resources.IMAGE5,
                Properties.Resources.IMAGE6
            };
            Start_();
            Environment.Exit(0);
        }
    }
}
