using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PageVisitor.Visitor;

namespace PageVisitor
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager = new VisitorManager();
            manager.Start();

            Console.ForegroundColor = ConsoleColor.White;

            if (ShoudCloseBrowser(args))
            {
                manager.Close();
                
            }

            PrintContacts();
            PrintPressAnyKey();
            
            Console.ReadKey();
            
        }

        static bool ShoudCloseBrowser(string[] args)
        {
            return args.Contains("-close");
        }

        static void PrintContacts()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Контакты разработчика \n Максим Максимов \n+79506113772\n fokusfm@yandex.ru \nhttps://vk.com/maksimov.maksim");
        }

        static void PrintPressAnyKey()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n\n\nДля завершения нажмите любую кнопку");
        }
    }
}
