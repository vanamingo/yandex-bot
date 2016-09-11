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
            Console.WriteLine("Для завершения нажмите любую кнопку");
        }
    }
}
