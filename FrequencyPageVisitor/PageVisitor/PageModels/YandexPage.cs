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
            Query = query.Query;
            Frequency = query.Frequency;
            SearchRequest();
            ResultItems = GetResultItems();
        }

        private void SearchRequest()
        {
            var url = "https://yandex.ru/search/?text=" + Query; 
            _driver.Navigate().GoToUrl(url);
        }

        public string Query { get; set; }
        public string Frequency { get; set; }

        public List<QueryResult> ResultItems { get; set; }
        public List<QueryResult> AdvertisementResultItems {
            get
            {
                var resultItems = ResultItems.Where(i => i.ResultType != QResultType.RegularItem).ToList();
                var num = 1;
                for (int i = 0; i < resultItems.Count; i++)
                {
                    var item = resultItems[i];

                    if (i != 0 && resultItems[i - 1].ResultType != resultItems[i].ResultType)
                    {
                        num = 1;
                    }

                    item.ResultNumber = num;
                    num++;
                }
                

                return resultItems;
            }
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
