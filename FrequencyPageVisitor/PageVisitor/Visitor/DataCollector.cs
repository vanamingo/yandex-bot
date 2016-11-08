using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FrequencyPageVisitor.PageModels;
using FrequencyPageVisitor.Settings;
using FrequencyPageVisitor.Utils;
using FrequencyPageVisitor.WebDriverWrapper;
using OpenQA.Selenium;

namespace FrequencyPageVisitor.Visitor
{
    public class DataCollector
    {
        private readonly VisitorSettings _settings;
        public DataCollector()
        {
            _settings = GlobalSettings.VisitorSettings;
        }

        public List<YandexPage> CollectRequestResults()
        {
            var queryCount = _settings.Queries.Count;
            Logger.WriteWhite("Начало обработки запросов. Кол-во: " + queryCount);

            var delay = _settings.DelayInSeconds*1000;

            var yaPages = new List<YandexPage>();
            var driver = WebDriverProvider.GetWebDriver();
            for (int i = 0; i < _settings.Queries.Count; i++)
            {
                var queryElement = _settings.Queries[i];
                Logger.WriteWhite(string.Format("({0} из {1}){2}", i+1, queryCount, queryElement.Query));
                var query = queryElement;
                yaPages.Add(GetResultPage(query, driver));
                Thread.Sleep(delay);
            }
            driver.Close();
            return yaPages;
        }

        private YandexPage GetResultPage(QueryElement query, IWebDriver driver)
        {
            //var driver = WebDriverProvider.GetWebDriver();
            var yaPage = new YandexPage(driver, query);
            //driver.Close();
            return yaPage;
        }
    }
}
