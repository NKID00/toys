﻿using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace DongTalk
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
            Application.Run(new SoundForm());
        }

        public static void Start(object obj)
        {

        }
    }
}
