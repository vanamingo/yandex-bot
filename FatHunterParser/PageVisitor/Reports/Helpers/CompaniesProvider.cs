using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrequencyPageVisitor.PageModels;

namespace FrequencyPageVisitor.Reports.Helpers
{
    public class CompanyAdverisment
    {
        private readonly int _totalQueriesCount;

        public CompanyAdverisment(int totalQueriesCount)
        {
            _totalQueriesCount = totalQueriesCount;
            Advertisments = new Dictionary<string, QueryResult>();
        }

        public string CompanyName { get; set; }

        public Dictionary<string, QueryResult> Advertisments { private set; get; }

        public int TopAdvertismentsCount
        {
            get { return Advertisments.Count(_ => _.Value.ResultType == QResultType.TopAdvertisement); }
        }

        public int BottomAdvertismentsCount
        {
            get { return Advertisments.Count(_ => _.Value.ResultType == QResultType.BottomAdvertisement); }
        }

        public double GetCompanyOrder()
        {
            var SP1 = Advertisments.Count(_=> _.Value.GetQResultTypeExtended() == QResultTypeExtended.SP1 );
            var SP2 = Advertisments.Count(_=> _.Value.GetQResultTypeExtended() == QResultTypeExtended.SP2 );
            var SP3 = Advertisments.Count(_=> _.Value.GetQResultTypeExtended() == QResultTypeExtended.SP3 );
            var G1 = Advertisments.Count(_ => _.Value.GetQResultTypeExtended() == QResultTypeExtended.G1);
            var G2 = Advertisments.Count(_ => _.Value.GetQResultTypeExtended() == QResultTypeExtended.G2);
            var G3 = Advertisments.Count(_ => _.Value.GetQResultTypeExtended() == QResultTypeExtended.G3);
            var G4 = Advertisments.Count(_ => _.Value.GetQResultTypeExtended() == QResultTypeExtended.G4);
            var G5 = Advertisments.Count(_ => _.Value.GetQResultTypeExtended() == QResultTypeExtended.G5);

            var advCount = SP1 + SP2 + SP3 + G1 + G2 + G3 + G4 + G5;
            var notCoveredQueries = _totalQueriesCount - advCount;
            var advRaiting = 
                SP1 * 1.6 + 
                SP2 * 1.5 + 
                SP3 * 1.4 +
                G1 * 0.7 + 
                G2 * 0.6 + 
                G3 * 0.5 +
                G4 * 0.4 +
                G5 * 0.3 +
                notCoveredQueries * 0.3;
            /*
             * 0,3	Пустых
1,6	СР1
1,5	СР2
1,4	СР3
0,7	Г1
0,6	Г2
0,5	Г3
0,4	Г4
0,3	Г5*/
            return advRaiting;
        }

    }

    public class CompaniesProvider
    {
        private class TopAdvCountComparer: IComparer<CompanyAdverisment> 
        {
            public int Compare(CompanyAdverisment x, CompanyAdverisment y)
            {
                var xOrder = x.GetCompanyOrder();
                var yOrder = y.GetCompanyOrder();

                if (xOrder > yOrder)
                {
                    return 1;
                }

                if (xOrder < yOrder)
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
                        companies[adv.CompanySite] = new CompanyAdverisment(pages.Count)
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
