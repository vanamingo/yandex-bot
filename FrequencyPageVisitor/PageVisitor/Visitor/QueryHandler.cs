using System;
using FrequencyPageVisitor.Settings;
using FrequencyPageVisitor.Utils;
using FrequencyPageVisitor.WebDriverWrapper;
using OpenQA.Selenium;

namespace FrequencyPageVisitor.Visitor
{
    public class QueryHandler
    {
        private readonly QueryElement _query;
        private FrequencyPageVisitor.Visitor.PageVisitor _visitor;
        private IWebDriver _driver;

        public QueryHandler(QueryElement query)
        {
            _query = query;
        }

        private void InitPageVisitor()
        {
            _driver = WebDriverProvider.GetWebDriver();
            _visitor = new FrequencyPageVisitor.Visitor.PageVisitor(_driver);
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

                    _visitor.AddTimeInfoToThePage();
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