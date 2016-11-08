using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FrequencyPageVisitor.PageModels;
using FrequencyPageVisitor.Settings;

namespace FrequencyPageVisitor.Reports
{
    public class RivalListReport
    {
        public List<CompanyAdverisment> Companies { get; private set; }

        public List<ReportRow> Rows { get; private set; }

        public RivalListReport(IList<YandexPage> pages)
        {
            Companies = GetCompanies(pages);
            Rows = BuildRows(pages, Companies);
        }

        private List<CompanyAdverisment> GetCompanies(IList<YandexPage> pages)
        {
            var companiesNames = new List<string>();
            var links = pages
                .Select(_ => _.AdvertisementResultItems)
                .ToList();

            links.ForEach(e => companiesNames.AddRange(e.Select(_ => _.CompanySite)));

            var companies = new Dictionary<string, CompanyAdverisment>();

            foreach (var page in pages)
            {
                foreach (var adv in page.AdvertisementResultItems)
                {
                    if (!companies.ContainsKey(adv.CompanySite))
                    {
                        companies[adv.CompanySite] = new CompanyAdverisment()
                        {
                            CompanyName = adv.CompanySite
                        };
                    }

                    companies[adv.CompanySite].Advertisments[page.Query] = adv;
                }
            }

            var r = companies
                .Select(kvp => kvp.Value)
                .ToList();
            r.Sort(new TopAdvCountComparer());
            r.Reverse();

            return r;
        }

        private class TopAdvCountComparer: IComparer<CompanyAdverisment> 
        {
            public int Compare(CompanyAdverisment x, CompanyAdverisment y)
            {
                if (x.TopAdvertismentsCount > y.TopAdvertismentsCount)
                {
                    return 1;
                }
                
                if (x.TopAdvertismentsCount < y.TopAdvertismentsCount)
                {
                    return -1;
                }

                if (x.BottomAdvertismentsCount > y.BottomAdvertismentsCount)
                {
                    return 1;
                }

                if (x.BottomAdvertismentsCount < y.BottomAdvertismentsCount)
                {
                    return -1;
                }

                return 0;
            }
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

        public class CompanyAdverisment
        {
            public CompanyAdverisment()
            {
                Advertisments = new Dictionary<string, QueryResult>();
            }

            public string CompanyName { get; set; }

            public Dictionary<string, QueryResult> Advertisments
            {
                private set;
                get;
            }

            public int TopAdvertismentsCount
            {
                get { return Advertisments.Count(_ => _.Value.ResultType == QResultType.TopAdvertisement); }
            }
            public int BottomAdvertismentsCount
            {
                get { return Advertisments.Count(_ => _.Value.ResultType == QResultType.BottomAdvertisement); }
            }

        }
    }
}