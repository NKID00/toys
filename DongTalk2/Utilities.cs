using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace DongTalk
{
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
