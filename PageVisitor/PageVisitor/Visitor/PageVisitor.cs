using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using OpenQA.Selenium;

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

        private bool ElementContainsSiteAndWords(string itemHtml, string ourSite, string words)
        {
            itemHtml = itemHtml.ToLower().Trim();
            ourSite = ourSite.ToLower().Trim();
            words = words.ToLower().Trim();

            if (!itemHtml.Contains(ourSite))
                return false;

            return words
                .Split('|')
                .Any(w => itemHtml.Contains(w));
        }

        public List<IWebElement> GetElementsWithOurAdvertisement(string words, string ourSite)
        {
            var requestItems = _driver
                .FindElements(By.CssSelector(".serp-item"))
                .Where(i => ElementContainsSiteAndWords(i.Text, ourSite, words))
                .ToList();

            return requestItems;
        }

        public string MakeScreenshot()
        {
            Screenshot ss = ((ITakesScreenshot)_driver).GetScreenshot();
            byte[] screenshotAsByteArray = ss.AsByteArray;

            if (!Directory.Exists("ScreenShots"))
            {
                Directory.CreateDirectory("ScreenShots");
            }

            var path = "ScreenShots/" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".jpg";
            File.WriteAllBytes(path, screenshotAsByteArray);
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