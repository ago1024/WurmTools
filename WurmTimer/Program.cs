/*
 * Erstellt mit SharpDevelop.
 * Benutzer: ago
 * Datum: 04.09.2010
 * Zeit: 00:09
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace WurmTimer
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
        }
    }
}
