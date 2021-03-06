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
        private static DateTime _startDateTime;

        static void Main(string[] args)
        {
            _startDateTime = DateTime.Now;

            try
            {
                InitSettings();

                if (GlobalSettings.VisitorSettings.Regions.Count == 0)
                {
                    HandleRegion(null);
                }
                else
                {
                    foreach (var region in GlobalSettings.VisitorSettings.Regions)
                    {
                        HandleRegion((RegionElement)region);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteError(ex.ToString());
            }

            PrintContacts();
            PrintPressAnyKey();

            Console.ReadKey();
        }

        private static void HandleRegion(RegionElement region)
        {
            if (GlobalSettings.VisitorSettings.WriteLogs)
            {
                ItitLogFile();
            }

            var reportDir = CreateReportFolder(region);

            var dataCollector = new DataCollector();

            List<YandexPage> yaPages;

            if (!GlobalSettings.VisitorSettings.DeserializeMode)
            {
                yaPages = dataCollector.CollectRequestResults(reportDir, region);
                Serialize(yaPages, Path.Combine(reportDir, "yaPages.xml"));
            }
            else
            {
                yaPages = DeSerialize();
            }

            var report = new RivalListReport(yaPages);
            var printer = new RivalListReportPrinter(report, Path.Combine(reportDir, "RivalListReport-{0}.html"));
            printer.Print(GlobalSettings.VisitorSettings.RivalsOnPage);

            var rivalReport = new RivalReport2(yaPages, reportDir, region);
            rivalReport.Print(reportDir);
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
            Console.ForegroundColor = ConsoleColor.Green;
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
            using (FileStream fs = new FileStream("yaPages.xml", FileMode.OpenOrCreate))
            {
                return (List<YandexPage>)formatter.Deserialize(fs);
            }
        }

        public static void Serialize(List<YandexPage> ypList, string path)
        {
            // передаем в конструктор тип класса
            XmlSerializer formatter = new XmlSerializer(typeof(List<YandexPage>));

            // получаем поток, куда будем записывать сериализованный объект
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, ypList);
            }
        }

        public static string CreateReportFolder(RegionElement region)
        {
            var path = 
                region == null ?
                Path.Combine("Reports", _startDateTime.ToString("yyyy-MM-dd_HH-mm-ss")):
                Path.Combine("Reports", _startDateTime.ToString("yyyy-MM-dd_HH-mm-ss"), region.Region);

            Directory.CreateDirectory(path);
            return path;
        }
    }

    public class GlobalSettings
    {
        // Глобальные переменные - зло. Я осознаю, что их лучше избегать. 
        public static string LoggerFilePath { get; set; }
        public static VisitorSettings VisitorSettings { get; set; }
    }
}
