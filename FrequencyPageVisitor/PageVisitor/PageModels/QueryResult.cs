using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace FrequencyPageVisitor.PageModels
{
    public class QueryResult
    {
        private readonly IWebElement _webElement;

        public QueryResult(IWebElement webElement)
        {
            _webElement = webElement;
        }

        public string TitleLink
        {
            get { return GetElementText(".organic__url"); }
        }

        public string TitleUrl
        {
            get { return GetElementText(".organic__subtitle"); }
        }

        private string GetElementText(string cssSelector)
        {
            var element = _webElement.FindElements(By.CssSelector(cssSelector));

            if (element.Count > 1)
            {
                throw new ArgumentException("Селектору соответствует несколько елементов: " + cssSelector);
            }

            if (element.Count == 0)
            {
                return string.Empty;
            }

            return element[0].Text;
        }

        public string TextAdvertisment
        {
            get { return GetElementText(".organic__content-wrapper"); }
        }

        public List<string> FastLinks
        {
            get
            {
                var links = _webElement.FindElements(By.CssSelector(".sitelinks__item"));

                if (links.Count == 0)
                {
                    return new List<string>();
                }

                return links.Select(l => l.Text).ToList();
            }
        }

        public List<string> GraySpecifications
        {
            get
            {
                // Под данный селектор попадают как уточнения, так и контактная инфо. 
                var grayBlocks = _webElement.FindElements(By.CssSelector(".serp-meta2_type_gray"));

                if (grayBlocks.Count == 0)
                {
                    return new List<string>();
                }

                var graySpecifications = grayBlocks
                    .FirstOrDefault(b => !b.Text.Contains("Контактная информация"));

                if (graySpecifications == null)
                {
                    return new List<string>();
                }

                var items = graySpecifications.FindElements(By.CssSelector(".serp-meta2__item"));

                if (items.Count == 0)
                {
                    return new List<string>();
                }

                return items.Select(l => l.Text).ToList();
            }
        }

        ///// <summary>
        ///// Наличие контактной информации
        ///// </summary>
        public bool YandexBuisenessCard
        {
            get
            {
                var grayBlocks = _webElement.FindElements(By.CssSelector(".serp-meta2_type_gray"));

                if (grayBlocks.Count == 0)
                {
                    return false;
                }

                var graySpecifications = grayBlocks
                    .FirstOrDefault(b => b.Text.Contains("Контактная информация"));

                return graySpecifications != null;
            }
        }
    

    ///// <summary>
        ///// Наличие отображаемой ссылки
        ///// </summary>
        public bool GreenUrl {
            get { return TitleUrl.Contains("/"); }
        }

        public enum ItemType
        {
            TopAdvertisement, BottomAdvertisement, RegularItem
        }

        public ItemType Type
        {
            get
            {
                var cssClass = _webElement.GetAttribute("class");
                if (cssClass == null || !cssClass.Contains("serp-adv-item"))
                {
                    return ItemType.RegularItem;
                }

                var itemNum = _webElement.GetAttribute("data-cid");

                switch (itemNum)
                {
                    case "0":
                    case "1":
                    case "2":
                        return ItemType.TopAdvertisement;
                }

                return ItemType.BottomAdvertisement;
            }
            
        }
    }
}