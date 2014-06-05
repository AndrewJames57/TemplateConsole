using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Template_Console
{
    public static class Common
    {
        private static Logger log = LogManager.GetCurrentClassLogger();
        private static readonly string MethodName = MethodBase.GetCurrentMethod().Name;
        private static readonly Random _rng = new Random();
        private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789"; //Added 1-9

        public static string RandomString(int size)
        {
            char[] buffer = new char[size];

            for (int i = 0; i < size; i++)
            {
                buffer[i] = _chars[_rng.Next(_chars.Length)];
            }
            return new string(buffer);
        }

        public static string RandomName()
        {
            string randomName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            return randomName;
        }

        public static void LogError(Exception ex, string logName, int optionalint = 0)
        {
            Logger logger = LogManager.GetLogger(logName);
            var st = new StackTrace(ex, true);
            string stackIndent = "";
            for (int i = 0; i < st.FrameCount; i++)
            {
                // Note that at this level, there are a number of stack frames, we want the last one.
                StackFrame sf = st.GetFrame(i);
                stackIndent = "Method: " + sf.GetMethod().ToString() + " File: " + sf.GetFileName() + " Line Number: " + sf.GetFileLineNumber() + " ";
            }
            // Using the last stackframe...
            string serial = RandomString(6);
            logger.Fatal(" Message: " + serial + " - " + ex.Message + " " + stackIndent + " " + (ex.InnerException != null ? " Inner Exception : " + ex.InnerException : string.Empty));
            if (optionalint == 1)
            {
                MessageBox.Show("An error occured. See log file for details: " + serial, "Error Message");
            }
            if (optionalint == 2)
            {
                Console.WriteLine("An error occured. See log file for details: " + serial);
            }
        }

        public static Dictionary<string, string> ParamConfig(string path)
        {
            string contents = String.Empty;
            try
            {
                using (FileStream fs = File.Open(path, FileMode.Open))
                using (StreamReader reader = new StreamReader(fs))
                {
                    contents = reader.ReadToEnd();
                }

                if (contents.Length > 0)
                {
                    string[] lines = contents.Split(new char[] { '\n' });
                    Dictionary<string, string> mysettings = new Dictionary<string, string>();
                    foreach (string line in lines)
                    {
                        string[] keyAndValue = line.Split(new char[] { '=' });
                        mysettings.Add(keyAndValue[0].Trim(), keyAndValue[1].Trim());
                    }

                    return mysettings;
                }
            }
            catch (Exception ex)
            {
                Common.LogError(ex, log.Name);
                return null;
            }

            return null;
        }
    }
}