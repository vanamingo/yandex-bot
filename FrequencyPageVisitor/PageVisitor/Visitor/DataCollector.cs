using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

        public List<YandexPage> CollectRequestResults(string reportDir, RegionElement region)
        {
            var queryCount = _settings.Queries.Count;

            if (region != null)
            {
                Logger.WriteGreen(region.Region + " - " + region.Code);
            }

            Logger.WriteWhite("Начало обработки запросов. Кол-во: " + queryCount);

            var delay = _settings.DelayInSeconds*1000;

            var yaPages = new List<YandexPage>();
           
            IWebDriver driver = null;
            //TODO. Управление временем жизни браузера более красиво. 
            if (!GlobalSettings.VisitorSettings.NewBrowserForQuery)
            {
                driver = WebDriverProvider.GetWebDriver();
            }
            for (int i = 0; i < _settings.Queries.Count; i++)
            {
                //TODO. Управление временем жизни браузера более красиво. 
                if (GlobalSettings.VisitorSettings.NewBrowserForQuery)
                {
                    driver = WebDriverProvider.GetWebDriver();
                }


                var queryElement = _settings.Queries[i];
                Logger.WriteWhite(string.Format("({0} из {1}){2}", i+1, queryCount, queryElement.Query));
                var query = queryElement;
                yaPages.Add(GetResultPage(query, driver, region));

                // TODO. Хотел реализовать скриншот экрана. 
                // Не получилось - хром делает скриншот только отображаемого экрана. Фаерфокс умеет делать скриншот всей страницы. 
                //MakeScreenshot("", driver, i, queryElement);

                Thread.Sleep(delay);
            }
            if (!GlobalSettings.VisitorSettings.NewBrowserForQuery)
            {
                //TODO. Управление временем жизни браузера более красиво.
                driver.Close();
            }
            return yaPages;
        }


        public void MakeScreenshot(string path, IWebDriver driver, int i, QueryElement queryElement)
        {
            var ss = ((ITakesScreenshot)driver).GetScreenshot();
            var screenshotAsByteArray = ss.AsByteArray;

            var screenshotName = Regex.Replace(queryElement.Query, "[^0-9a-zA-Z]+", "");
            screenshotName = string.Format("{0}-{1}.jpg",i, screenshotName);

            if (!Directory.Exists("Screenshots"))
            {
                Directory.CreateDirectory("Screenshots");
            }

            File.WriteAllBytes(Path.Combine(path,"Screenshots", screenshotName), screenshotAsByteArray);
        }

        private YandexPage GetResultPage(QueryElement query, IWebDriver driver, RegionElement region)
        {
            //var driver = WebDriverProvider.GetWebDriver();
            var yaPage = new YandexPage(driver, query, region);
            //driver.Close();
            return yaPage;
        }
    }
}
