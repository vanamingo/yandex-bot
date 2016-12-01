using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FrequencyPageVisitor.Settings;
using OpenQA.Selenium;

namespace FrequencyPageVisitor.PageModels
{
    public class FatTaskPage
    {
        private readonly IWebDriver _driver;
        private readonly RegionElement _region;


        public string ContentText {
            get
            {
                var el = _driver.FindElement(By.CssSelector(".content"));
                return el.Text;
            }
        }

        public List<string> VideoUrls
        {
            get
            {
                var els = _driver.FindElements(By.CssSelector("video source"));
                return els.Select( _=>_.GetAttribute("src")).ToList();
            }
        }

        public string Title {
            get
            {
                var el = _driver.FindElement(By.CssSelector(".under_title"));
                return el.Text;
                
            }
        }

        public FatTaskPage(IWebDriver driver, string url)
        {
            this._driver = driver;
            SearchRequest(url);
        }

        public List<string> QueryGroup { get; set; }

        private void SearchRequest(string url)
        {
            _driver.Navigate().GoToUrl(url);
        }
    }
}
