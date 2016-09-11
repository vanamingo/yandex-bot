using System.Configuration;
using System.Threading;
using PageVisitor.Settings;

namespace PageVisitor.Visitor
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
            var handler = new QueryHandler(query);
            handler.HandleQuery();
        }
    }
}
