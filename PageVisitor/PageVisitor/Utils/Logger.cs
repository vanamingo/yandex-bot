using System;
using System.IO;

namespace PageVisitor.Utils
{
    public class Logger
    {
        public static void WriteRed(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
        }
        public static void WriteGreen(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(msg);
        }
        public static void WriteError(string msg)
        {
            //var path = @"E:\MAXON\MolotokParse\logCheck.txt";
            //msg = "\n --- " + DateTime.Now.ToLongTimeString() + "--------- \n" + msg;
            //File.AppendAllText(path, msg);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("*-------- Ошибка --------*");
            Console.WriteLine(msg);
        }
    }
}