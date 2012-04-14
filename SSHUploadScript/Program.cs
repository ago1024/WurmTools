using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WurmUtils;

namespace SSHUploadScript
{
    static class Program
    {
        [STAThread]
        static void Main()
        {

            WurmDateTime dt = new WurmDateTime();
            WurmDateTimeStamp stamp = new WurmDateTimeStamp();

            String[] logFiles = Directory.GetFiles(DateExtract.getLogPath(), "_Event*.txt");
            Array.Sort(logFiles);
            Array.Reverse(logFiles);
            Array.Resize(ref logFiles, 1);

            foreach (String logFile in logFiles)
            {
                System.Diagnostics.Debug.WriteLine("Scanning file {0}", logFile);

                if (DateExtract.scanLogFile(logFile, ref dt, ref stamp, DateTime.Now))
                {
                    new SSHUpload().HandleWurmTimeStamp(stamp);
                }
            }
        }
    }
}
