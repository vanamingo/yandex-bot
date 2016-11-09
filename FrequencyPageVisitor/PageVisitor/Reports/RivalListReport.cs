using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FrequencyPageVisitor.PageModels;
using FrequencyPageVisitor.Reports.Helpers;
using FrequencyPageVisitor.Settings;

namespace FrequencyPageVisitor.Reports
{
    public class RivalListReport
    {
        public List<CompanyAdverisment> Companies { get; private set; }

        public List<ReportRow> Rows { get; private set; }

        public RivalListReport(IList<YandexPage> pages)
        {
            Companies = CompaniesProvider.GetCompanies(pages);
            Rows = BuildRows(pages, Companies);
        }

        

        

        private List<ReportRow> BuildRows(IList<YandexPage> pages, List<CompanyAdverisment> companies)
        {
            //for

            return pages.Select(p => GetRow(p, companies)).ToList();

        }

        private ReportRow GetRow(YandexPage yandexPage, List<CompanyAdverisment> companies)
        {
            return new ReportRow()
            {
                QueryName = yandexPage.Query,
                Frequency = yandexPage.Frequency,
                QueryGroup = yandexPage.QueryGroup,
                AdvertismentCount = yandexPage.AdvertisementResultItems.Count,
                Companies = companies.Where(_ => _.Advertisments.ContainsKey(yandexPage.Query)).ToList()
            };
        }

        public class ReportRow
        {
            public string QueryName { get; set; }
            public string Frequency { get; set; }
            public int AdvertismentCount { get; set; }
            public List<CompanyAdverisment> Companies { get; set; }
            public List<string> QueryGroup { get; set; }
        }

        
    }
}