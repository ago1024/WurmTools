using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace WurmLaunch
{
    class Program
    {
        static void Main(string[] args)
        {
            string jnlp = "http://www.wurmonline.com/client/wurmclient.jnlp";

            foreach (string arg in args)
            {
                switch (arg)
                {
                    case "stable":
                        jnlp = "http://www.wurmonline.com/client/wurmclient.jnlp";
                        break;
                    case "unstable":
                        jnlp = "http://www.wurmonline.com/client/wurmclient_unstable.jnlp";
                        break;
                    case "testing":
                        jnlp = "http://www.wurmonline.com/client/wurmclient_test.jnlp";
                        break;
                    default:
                        jnlp = arg;
                        break;
                }
            }

            Process wurmProcess = new Process();
            wurmProcess.StartInfo.FileName = "javaws.exe";
            wurmProcess.StartInfo.Arguments = "-wait -Xnosplash " + jnlp;
            wurmProcess.Start();
            wurmProcess.WaitForExit();
        }
    }
}
