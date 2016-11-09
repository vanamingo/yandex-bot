using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrequencyPageVisitor.PageModels;

namespace FrequencyPageVisitor.Reports.Helpers
{
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

    public class CompaniesProvider
    {
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

        public static List<CompanyAdverisment> GetCompanies(IList<YandexPage> pages)
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
    }
}
