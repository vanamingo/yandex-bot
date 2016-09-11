using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using PageVisitor.Settings;
using PageVisitor.Utils;
using PageVisitor.WebDriverWrapper;

namespace PageVisitor.Visitor
{
    public class QueryHandler
    {
        private readonly QueryElement _query;
        private PageVisitor _visitor;
        private IWebDriver _driver;

        public QueryHandler(QueryElement query)
        {
            _query = query;
        }

        private void InitPageVisitor()
        {
            _driver = WebDriverProvider.GetWebDriver();
            _visitor = new PageVisitor(_driver);
        }

        public void HandleQuery()
        {
            try
            {
                Logger.WriteWhite("Обработка: " + _query.Query);
                InitPageVisitor();

                var url = "https://yandex.ru/search/?text=" + _query.Query;
                _visitor.NavigateToUrl(url);

                var elementsWithOurAdvertisement =
                    _visitor.GetElementsWithOurAdvertisement(_query);

                if (elementsWithOurAdvertisement.Count != 0)
                {
                    elementsWithOurAdvertisement
                        .ForEach(_visitor.HightlightTheElement);

                    _visitor.MakeScreenshot(_query);

                    Logger.WriteGreen("Объявление найдено");
                }
                else
                {
                    var msg = String.Format("Объявление не найдено.");
                    Logger.WriteRed(msg);
                }

                Close();
            }
            catch (Exception ex)
            {
                Logger.WriteError(ex.ToString());
            }
        }
        public void Close()
        {
            _visitor.Close();
        }
    }
}