using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using OpenQA.Selenium;

namespace FrequencyPageVisitor.PageModels
{
    public enum QResultType
    {
        TopAdvertisement,
        BottomAdvertisement,
        RegularItem
    }

    public class QueryResult
    {
        public QueryResult()
        {
            
        }

        private readonly IWebElement _webElement;

        public QueryResult(IWebElement webElement)
        {
            _webElement = webElement;
            TitleLink = GetTitleLink();
            TitleUrl = GetTitleUrl();
            TitleHref = GetTitleHref();
            TextAdvertisment = GetTextAdvertisment();
            FastLinks = GetFastLinks();
            GraySpecifications = GetGraySpecifications();
            YandexBuisenessCard = GetYandexBuisenessCard();
            GreenUrl = GetGreenUrl();
            ResultType = GetResultType();
            YandexMarket = GetYandexMarket();
        }

        public string YandexMarket { get; set; }

        private string GetYandexMarket()
        {
            var els = _webElement.FindElements(By.CssSelector(".rating2__stars"));

            if (els.Count == 0)
            {
                return "нет";
            }

            var cl = els[0].GetAttribute("class");
            var starsLength = cl.Replace("rating2__stars rating2__stars_width_", "").Trim();

            float count;
            if (float.TryParse(starsLength, out count))
            {
                return (count/20).ToString("F1");
            }

            return "ошибка в подсчете звезд.";
        }

        public string TitleHref { get; set; }
        [XmlIgnore]
        public string IsUtm {
            get
            {//yclid и utm
                if (TitleHref.ToLower().Contains("utm"))
                {
                    return "utm";
                }

                if (TitleHref.ToLower().Contains("yclid"))
                {
                    return "yclid";
                }

                return "нет";
            }
        }

        public QResultType ResultType { get; set; }
        [XmlIgnore]
        public int ResultNumber { get; set; }

        public bool GreenUrl { get; set; }

        public bool YandexBuisenessCard { get; set; }

        public List<string> GraySpecifications { get; set; }

        public List<string> FastLinks { get; set; }

        public string TextAdvertisment { get; set; }

        public string TitleUrl { get; set; }

        public string TitleLink { get; set; }

        [XmlIgnore]
        public string CompanySite
        {
            get
            {
                var site = TitleUrl.Replace("Реклама", "").Split('/')[0];
                return site;
            }
        }

        public string GetTitleLink()
        {
            return GetElementText(".organic__url");
        }
        public string GetTitleHref()
        {
            var element = _webElement.FindElements(By.CssSelector(".organic__url"));

            if (element.Count == 0)
            {
                return string.Empty;
            }

            return element[0].GetAttribute("href");
        }

        public string GetTitleUrl()
        {
            return GetElementText(".organic__subtitle");
        }

        private string GetElementText(string cssSelector)
        {
            var element = _webElement.FindElements(By.CssSelector(cssSelector));

            //if (element.Count > 1)
            //{
            //    throw new ArgumentException("Селектору соответствует несколько елементов: " + cssSelector);
            //}

            if (element.Count == 0)
            {
                return string.Empty;
            }

            return element[0].Text;
        }

        public string GetTextAdvertisment()
        {
            return GetElementText(".organic__content-wrapper");
        }

        public List<string> GetFastLinks()
        {
            var links = _webElement.FindElements(By.CssSelector(".sitelinks__item"));

            if (links.Count == 0)
            {
                return new List<string>();
            }

            return links.Select(l => l.Text).ToList();
        }

        public List<string> GetGraySpecifications()
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

        ///// <summary>
        ///// Наличие контактной информации
        ///// </summary>
        public bool GetYandexBuisenessCard()
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


        ///// <summary>
        ///// Наличие отображаемой ссылки
        ///// </summary>
        public bool GetGreenUrl()
        {
            return GetTitleUrl().Contains("/");
        }

        public QResultType GetResultType()
        {
            var cssClass = _webElement.GetAttribute("class");
            if (cssClass == null || !cssClass.Contains("serp-adv-item"))
            {
                return QResultType.RegularItem;
            }

            var itemNum = _webElement.GetAttribute("data-cid");

            switch (itemNum)
            {
                case "0":
                case "1":
                case "2":
                    return QResultType.TopAdvertisement;
            }

            return QResultType.BottomAdvertisement;
        }
    }
}