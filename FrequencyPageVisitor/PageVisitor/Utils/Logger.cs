using System;
using System.IO;

namespace PageVisitor.Utils
{
    public class Logger
    {
        public static void WriteWhite(string msg)
        {
            Console.ForegroundColor = ConsoleColor.White;
            WriteLog(msg);
        }
        public static void WriteRed(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLog(msg);
        }
        public static void WriteGreen(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            WriteLog(msg);
        }

        private static void WriteLog(string msg)
        {
            Console.WriteLine(msg);

            if (GlobalSettings.VisitorSettings.WriteLogs)
            {
                try
                {
                    File.AppendAllText(GlobalSettings.LoggerFilePath, "\n" + msg);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Запись в файл лога не доступна");
                }
                
            }
        }

        public static void WriteError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLog("*-------- Ошибка --------*\n" + msg);
        }
    }
}