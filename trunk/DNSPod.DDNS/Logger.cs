using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace DDNSPod.DNSPod.DDNS
{
    public class Logger
    {
        public static void WriteLog(string msg)
        {
            string path = Path.Combine(Application.StartupPath, "log");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string filename = DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            string fullfile = Path.Combine(path, filename);

            using (StreamWriter w = File.AppendText(fullfile))
            {
                Logger.Log(msg, w);
                w.Close();
            }
        }

        public static void Log(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            w.WriteLine("  :");
            w.WriteLine("  :{0}", logMessage);
            w.WriteLine("-------------------------------");
            // Update the underlying file.
            w.Flush();
        }
    }
}
