using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
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

    public class MessageBoxes
    {
        private ulong msgid = (ulong)Guid.NewGuid().ToString().GetHashCode();
        private readonly Mutex mutex = new Mutex();
        private bool locked = false;

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(IntPtr classname, string title);
        [DllImport("user32.dll")]
        private static extern bool SetWindowText(IntPtr hWnd, string title);
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out Rectangle rect);
        [DllImport("user32.dll")]
        private static extern int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        private static extern void MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool repaint);

        public void MessageBoxShow(int x, int y, AlignX alignx, AlignY aligny, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            if (!locked)
            {
                mutex.WaitOne();
                locked = true;
            }
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
                mutex.WaitOne();
                mutex.ReleaseMutex();
                PostMessage(handle, 0x10, IntPtr.Zero, IntPtr.Zero);
            }
            )).Start();
            msgid++;
        }

        public void MessageboxShowRandom(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            if (!locked)
            {
                mutex.WaitOne();
                locked = true;
            }
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
                MoveWindow(
                    handle,
                    Utilities.rand.Next(0, Utilities.w - rw),
                    Utilities.rand.Next(0, Utilities.h - rh),
                    rw, rh,
                    true
                    );
                mutex.WaitOne();
                mutex.ReleaseMutex();
                PostMessage(handle, 0x10, IntPtr.Zero, IntPtr.Zero);
            }
            )).Start();
            msgid++;
        }

        public void MessageBoxCloseAll()
        {
            if (locked)
            {
                mutex.ReleaseMutex();
                locked = false;
            }
        }

        ~MessageBoxes()
        {
            MessageBoxCloseAll();
        }
    }

    public class Images
    {
        private readonly Mutex mutex = new Mutex();
        private bool locked = false;

        [DllImport("user32.dll")]
        private static extern int PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private void ImageShow(Image img, int x, int y, AlignX alignx, AlignY aligny)
        {
            if (!locked)
            {
                mutex.WaitOne();
                locked = true;
            }
            new Thread(new ThreadStart(() =>
            {
                var imgform = new ImageForm(img, x, y, alignx, aligny);
                var image_handle = imgform.Handle;
                new Thread(new ThreadStart(() =>
                {
                    mutex.WaitOne();
                    mutex.ReleaseMutex();
                    PostMessage(image_handle, 0x10, IntPtr.Zero, IntPtr.Zero);
                }
                )).Start();
                Application.Run(imgform);
            }
            )).Start();
        }

        public void ImageShowRandom(Image img)
        {
            ImageShow(
                img,
                Utilities.rand.Next(0, Utilities.w - img.Width),
                Utilities.rand.Next(0, Utilities.h - img.Height),
                AlignX.Left,
                AlignY.Top
                );
        }

        public void ImageCloseAll()
        {
            if (locked)
            {
                mutex.ReleaseMutex();
                locked = false;
            }
        }

        ~Images()
        {
            ImageCloseAll();
        }
    }

    public class Sounds
    {
        public static void WriteWaveFile(string path, UnmanagedMemoryStream wave)
        {
            var l = wave.Length;
            var buffer = new byte[l];
            wave.Read(buffer, 0, (int)l);
            using (var fs = new FileStream(path, FileMode.Create))
            {
                fs.Write(buffer, 0, (int)l);
            }
        }
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
            var pDongLib = Path.Combine(Path.GetTempPath(), "DongLib.dll");
            using (var fs = new FileStream(pDongLib, FileMode.Create))
            {
                fs.Write(Properties.Resources.DongLib, 0, (int)Properties.Resources.DongLib.Length);
            }
            DongLibHandle = LoadLibrary(pDongLib);
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

    public static class Utilities
    {
        public static int w = Screen.PrimaryScreen.Bounds.Width;
        public static int h = Screen.PrimaryScreen.Bounds.Height;
        public static Random rand = new Random(Guid.NewGuid().ToString().GetHashCode());
    }
}
