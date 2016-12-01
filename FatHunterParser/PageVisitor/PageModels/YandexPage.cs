using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FrequencyPageVisitor.Settings;
using OpenQA.Selenium;

namespace FrequencyPageVisitor.PageModels
{
    public class YandexPage
    {
        private readonly IWebDriver _driver;
        private readonly RegionElement _region;

        public YandexPage()
        {
        }

        public YandexPage(IWebDriver driver, QueryElement query, RegionElement region)
        {
            this._driver = driver;
            _region = region;
            Query = query.Query;
            Frequency = query.Frequency;
            QueryGroup = query.Group;
            SearchRequest();
            ResultItems = GetResultItems();
        }

        public List<string> QueryGroup { get; set; }

        private void SearchRequest()
        {
            var regionParameter = _region != null ? "&rstr=-" + _region.Code : "";
            var url = "https://yandex.ru/search/?text=" + Query + regionParameter; 
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
                //.FindElements(By.CssSelector("li.serp-adv-item"))
                .ToList();

            return elements.Select(e => new QueryResult(e)).ToList();

        }
    }
}
