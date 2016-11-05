using System;
using System.Linq;
using System.Threading;
using FrequencyPageVisitor.PageModels;
using FrequencyPageVisitor.Settings;
using FrequencyPageVisitor.Utils;
using FrequencyPageVisitor.WebDriverWrapper;

namespace FrequencyPageVisitor.Visitor
{
    public class QueriesManager
    {
        private readonly VisitorSettings _settings;
        public QueriesManager()
        {
            _settings = GlobalSettings.VisitorSettings;
        }

        public void HandleQueries()
        {
            var delay = _settings.DelayInSeconds*1000;

            for (int i = 0; i < _settings.Queries.Count; i++)
            {
                var query = _settings.Queries[i];
                HandleQuery(query);
                Thread.Sleep(delay);
            }
        }

        private void HandleQuery(QueryElement query)
        {
            try
            {
                var driver = WebDriverProvider.GetWebDriver();
                var yaPage = new YandexPage(driver);
                yaPage.SearchRequest(query.Query);
                var items = yaPage.ResultItems;

                var l = items[items.Count - 1];
                var g = l.GraySpecifications;
                var i = l.YandexBuisenessCard;

                var handler = new QueryHandler(query);
                handler.HandleQuery();
            }
            catch (Exception ex)
            {
                Logger.WriteError(ex.ToString());
            }
        }
    }
}
