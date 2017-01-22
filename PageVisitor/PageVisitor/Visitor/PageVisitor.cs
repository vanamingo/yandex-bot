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

        private IJavaScriptExecutor JavaScriptExecutor
        {
            get { return (IJavaScriptExecutor) _driver; }
        }

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
            JavaScriptExecutor.ExecuteScript("arguments[0].setAttribute('style', 'border:2px solid red')", elementWithOurAdvertisement);
        }

        public void AddTimeInfoToThePage()
        {
            var timeInfo = String.Format("Время запроса: {0}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
            var script = string.Format(@"
var myDiv = document.createElement('div');
myDiv.innerText = '{0}';

myDiv.style.color = 'red';
myDiv.style['font-size'] = '17px';
myDiv.style['margin-top'] = '45px';


var body = document.getElementsByTagName('body')[0];
var firstChild = body.children[0];

body.insertBefore(myDiv, firstChild);

", timeInfo);


            var js = (IJavaScriptExecutor)_driver;
            js.ExecuteScript(script);

        }
        public void Close()
        {
            _driver.Close();
        }
    }
}