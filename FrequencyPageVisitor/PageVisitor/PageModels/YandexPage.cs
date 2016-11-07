using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrequencyPageVisitor.Settings;
using OpenQA.Selenium;

namespace FrequencyPageVisitor.PageModels
{
    public class YandexPage
    {
        private readonly IWebDriver _driver;

        public YandexPage()
        {
        }

        public YandexPage(IWebDriver driver, QueryElement query)
        {
            this._driver = driver;
            Query = query;
            SearchRequest();
            ResultItems = GetResultItems();
        }

        private void SearchRequest()
        {
            var url = "https://yandex.ru/search/?text=" + Query.Query; 
            _driver.Navigate().GoToUrl(url);
        }

        public QueryElement Query { get; set; }

        public List<QueryResult> ResultItems { get; set; }
        public List<QueryResult> AdvertisementResultItems {
            get { return ResultItems.Where(i => i.ResultType != QResultType.RegularItem).ToList(); }
        }
        private List<QueryResult> GetResultItems()
        {
            var elements = _driver
                .FindElements(By.CssSelector("li.serp-item"))
                .ToList();

            return elements.Select(e => new QueryResult(e)).ToList();

        }
    }
}
