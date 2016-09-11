using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using OpenQA.Selenium;
using PageVisitor.Settings;

namespace PageVisitor.Visitor
{
    public class PageVisitor
    {
        private readonly IWebDriver _driver;

        public PageVisitor(IWebDriver driver)
        {
            _driver = driver;
        }

        public void NavigateToUrl(string url)
        {
            _driver.Navigate().GoToUrl(url);
        }

        private bool ElementContainsSiteAndWords(string itemHtml, QueryElement query)
        {
            itemHtml = itemHtml.ToLower().Trim();
            var ourSite = query.OurSite.ToLower().Trim();

            return itemHtml.Contains(ourSite);
        }

        public List<IWebElement> GetElementsWithOurAdvertisement(QueryElement query)
        {
            var requestItems = _driver
                .FindElements(By.CssSelector(".serp-item"))
                .Where(i => ElementContainsSiteAndWords(i.Text, query))
                .ToList();

            return requestItems;
        }

        public string MakeScreenshot(QueryElement query)
        {
            var ss = ((ITakesScreenshot)_driver).GetScreenshot();
            var screenshotAsByteArray = ss.AsByteArray;

            File.WriteAllBytes(GetFilePath(query), screenshotAsByteArray);
            return GetFilePath(query);
        }

        private static string GetFilePath(QueryElement query)
        {
            var directory = Path.Combine("ScreenShots", query.Folder);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var path = Path.Combine(directory, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".jpg");
            return path;
        }

        public void HightlightTheElement(IWebElement elementWithOurAdvertisement)
        {
            var js = (IJavaScriptExecutor)_driver;
            js.ExecuteScript("arguments[0].setAttribute('style', 'border:2px solid red')", elementWithOurAdvertisement);
        }

        public void Close()
        {
            _driver.Close();
        }
    }
}