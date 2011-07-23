using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WOSUploader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

            //Uploader uploader = new Uploader();
            //uploader.Upload(@"D:\ago\Wurm\players\ago\dumps\skills.20110723.1802.txt");
        }
    }
}
