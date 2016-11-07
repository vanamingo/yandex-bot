﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using FrequencyPageVisitor.PageModels;
using FrequencyPageVisitor.Reports;
using FrequencyPageVisitor.Settings;
using FrequencyPageVisitor.Utils;
using FrequencyPageVisitor.Visitor;

namespace FrequencyPageVisitor
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                InitSettings();
                if (GlobalSettings.VisitorSettings.WriteLogs)
                {
                    ItitLogFile();
                }

                //var dataCollector = new DataCollector();
                //var searchResults = dataCollector.CollectRequestResults();
                //Serialize(searchResults);

                var yaPages = DeSerialize();

                var report = new RivalListReport(yaPages);


            }
            catch (Exception ex)
            {
                Logger.WriteError(ex.ToString());
            }

            PrintContacts();
            PrintPressAnyKey();

            Console.ReadKey();
        }

        private static void InitSettings()
        {
            Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            GlobalSettings.VisitorSettings = (VisitorSettings)cfg.Sections["VisitorSettings"];
        }

        private static void ItitLogFile()
        {
            var loggerDirectory = "Logs";
            if (!Directory.Exists(loggerDirectory))
            {
                Directory.CreateDirectory(loggerDirectory);
            }

            var loggerFilePath = Path.Combine(loggerDirectory, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + "-log.txt");
            //File.Create(loggerFilePath);

            GlobalSettings.LoggerFilePath = loggerFilePath;
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


        public static List<YandexPage> DeSerialize( )
        {
            XmlSerializer formatter = new XmlSerializer(typeof(List<YandexPage>));
            // десериализация
            using (FileStream fs = new FileStream("yaPage.xml", FileMode.OpenOrCreate))
            {
                return (List<YandexPage>)formatter.Deserialize(fs);
            }
        }

        public static void Serialize(List<YandexPage> ypList)
        {
            // передаем в конструктор тип класса
            XmlSerializer formatter = new XmlSerializer(typeof(List<YandexPage>));

            // получаем поток, куда будем записывать сериализованный объект
            using (FileStream fs = new FileStream("yaPage.xml", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, ypList);
            }
        }

        
    }

    public class GlobalSettings
    {
        // Глобальные переменные - зло. Я осознаю, что их лучше избегать. 
        public static string LoggerFilePath { get; set; }
        public static VisitorSettings VisitorSettings { get; set; }
    }
}
