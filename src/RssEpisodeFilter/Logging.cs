using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using MUMS.RssEpisodeFilter.Properties;

namespace MUMS.RssEpisodeFilter
{
    public static class Logging
    {
        private static object syncRoot = new object();
        private static StreamWriter m_logWriter = null;

        private static StreamWriter LogWriter
        {
            get
            {
                if (!Settings.Default.EnableLogger)
                    return null;

                if (m_logWriter == null)
                {
                    if (!Directory.Exists(Settings.Default.LogStore))
                        Directory.CreateDirectory(Settings.Default.LogStore);

                    string fileName = string.Format("{0:yyyy-MM-dd_HH.mm}.txt", DateTime.Now);
                    m_logWriter = new StreamWriter(Path.Combine(Settings.Default.LogStore, fileName));
                }

                return m_logWriter;
            }
        }
    
        public static void PrintDownloaded(string title)
        {
            PrintStatus(ConsoleColor.Green, "Download", title);
        }

        public static void PrintDateSkipped(string title)
        {
            PrintStatus(ConsoleColor.Gray, "DateSkip", title);
        }

        public static void PrintDuplicate(string title)
        {
            PrintStatus(ConsoleColor.Yellow, "Duplicate", title);
        }

        public static void PrintInvalid(string title)
        {
            PrintStatus(ConsoleColor.Red, "Invalid", title);
        }

        public static void PrintNewline()
        {
            if (Debugger.IsAttached)
                Console.WriteLine();
            else if(Settings.Default.EnableLogger)
                LogWriter.WriteLine();
        }

        public static void PrintStatus(ConsoleColor leftColor, string type, string value)
        {
            if (Debugger.IsAttached)
            {
                var current = Console.ForegroundColor;
                Console.ForegroundColor = leftColor;
                Console.Write("{0}:", type);
                Console.ForegroundColor = current;
                Console.WriteLine("\t{0}", value);
            }
            else if (Settings.Default.EnableLogger)
            {
                LogWriter.WriteLine("{0}:\t{1}", type, value);
            }
        }

        public static void End()
        {
            if (Debugger.IsAttached)
                Console.ReadLine();
            else if (Settings.Default.EnableLogger && LogWriter != null)
                LogWriter.Close();
        }
    }
}
