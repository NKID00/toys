using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DongTalk
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SoundForm());
        }

        public static void Start(object obj)
        {
            if (DialogResult.Yes != MessageBox.Show(
                "DongTalk © 2020 NKID00\r\n"
                + "https://gitee.com/NKID00/DongTalk"
                + "DongLib © 2020 NKID00\r\n"
                + "https://gitee.com/NKID00/DongLib"
                + "注意：本程序可能导致直接或间接的损失。\r\n"
                + "NKID00 不对使用此程序造成的任何损失负责。\r\n"
                + "你要继续吗？",
                "DongTalk © 2020 NKID00",
                MessageBoxButtons.YesNo, MessageBoxIcon.Information
                ))
            {
                Utilities.Exit();
            }
            if (DialogResult.Yes != MessageBox.Show(
                "警告：请保存所有未保存的文件。\r\n"
                + "你仍要继续吗？（最后一次确认）",
                "DongTalk © 2020 NKID00",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning
                ))
            {
                Utilities.Exit();
            }

            var Wave0_p = Path.Combine(Path.GetTempPath(), "f95706dc.wav");
            var Wave1_p = Path.Combine(Path.GetTempPath(), "7ebab543.wav");
            var Wave2_p = Path.Combine(Path.GetTempPath(), "87efc27b.wav");
            Sounds.WriteWaveFile(Wave0_p, Properties.Resources.WAVE0);
            Sounds.WriteWaveFile(Wave1_p, Properties.Resources.WAVE1);
            Sounds.WriteWaveFile(Wave2_p, Properties.Resources.WAVE2);
            var Wave0 = Encoding.UTF8.GetBytes(Wave0_p);
            var Wave1 = Encoding.UTF8.GetBytes(Wave1_p);
            var Wave2 = Encoding.UTF8.GetBytes(Wave2_p);
            var lib = new DongLib();
            lib.DongSoundInit((IntPtr)obj);

            var msg = new MessageBoxes();
            msg.MessageBoxShow(
                Utilities.w / 2, Utilities.h / 2,
                AlignX.Middle, AlignY.Middle,
                "0xfffff233 指令引用的 0x00000233 内存。该内存不能为read。\r\n"
                + "\r\n"
                + "要终止程序，请单击“确定”。\r\n",
                "DongTalk.exe - 应用程序错误",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
                );
            Thread.Sleep(3000);
            for (int i = 0; i < 30; i++)
            {
                try
                {
                    msg.MessageboxShowRandom(
                        "0xfffff233 指令引用的 0x00000233 内存。该内存不能为read。\r\n"
                        + "\r\n"
                        + "要终止程序，请单击“确定”。\r\n",
                        "DongTalk.exe - 应用程序错误",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                        );
                    lib.DongSoundLoadAndPlay(Utilities.RandChoice(Wave0, Wave1, Wave2));
                }
                catch (Exception)
                {

                }
                Thread.Sleep(100);
            }
            Thread.Sleep(1000);

            var images_raw = new Image[]
            {
                Properties.Resources.IMAGE0,
                Properties.Resources.IMAGE1,
                Properties.Resources.IMAGE2,
                Properties.Resources.IMAGE3,
                Properties.Resources.IMAGE4,
                Properties.Resources.IMAGE5,
                Properties.Resources.IMAGE6
            };
            var img = new Images();
            for (int i = 0; i < 10; i++)
            {
                var images = Utilities.Shuffle(images_raw);
                foreach (var item in images)
                {
                    try
                    {
                        img.ImageShowRandom(item);
                        lib.DongSoundLoadAndPlay(Utilities.RandChoice(Wave0, Wave1, Wave2));
                    }
                    catch (Exception)
                    {

                    }
                    Thread.Sleep(100);
                }
            }
            // lib.DongBSOD(0xfffff233);
            
            msg.MessageBoxCloseAll();
            img.ImageCloseAll();
            Thread.Sleep(1000);
            Utilities.Exit();
            
        }
    }
}
