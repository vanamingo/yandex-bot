using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;

namespace FrequencyPageVisitor.PageModels
{
    public class YandexPage
    {
        private readonly IWebDriver _driver;

        public YandexPage(IWebDriver driver)
        {
            this._driver = driver;
        }

        public void SearchRequest(string requestText)
        {
            var url = "https://yandex.ru/search/?text=" + requestText; 
            _driver.Navigate().GoToUrl(url);
        }

        public List<QueryResult> ResultItems
        {
            get
            {
                var elements = _driver
                    .FindElements(By.CssSelector("li.serp-item"))
                    .ToList();

                return elements.Select(e => new QueryResult(e)).ToList();
            }
        }
    }
}
